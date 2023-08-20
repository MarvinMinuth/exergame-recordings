using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;

[System.Serializable]
public class HapticFeedback
{
    public GameObject leftController, rightController;
    private XRBaseController leftXRController, rightXRController;
    [Range(0f, 1f)]
    public float intensity = 0.5f;
    [Range(0.01f, 0.2f)]
    public float duration = 0.1f;

    public void Initialize()
    {
        if (leftController != null)
        {
            leftXRController = leftController.GetComponent<XRBaseController>();
        }
        if (rightController != null)
        {
            rightXRController = rightController.GetComponent<XRBaseController>();
        }
    }

    public void TriggerHaptic()
    {
        if (intensity > 0 && leftXRController != null && rightXRController != null)
        {
            leftXRController.SendHapticImpulse(intensity, duration);
            rightXRController.SendHapticImpulse(intensity, duration);
        }
    }
}

[System.Serializable]
public class AudioFeedback
{
    private AudioSource audioSource;
    public AudioClip lubClip, dubClip;
    

    public void Initialize(GameObject obj)
    {
        audioSource = obj.GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    public void PlayLub()
    {
        audioSource.clip = lubClip;
        audioSource.Play();
    }

    public void PlayDub()
    {
        audioSource.clip = dubClip;
        audioSource.Play();
    }
}

[System.Serializable]
public class VisualFeedback
{
    private Vector3 startScale;
    private Vector3 endScale;
    private GameObject obj;

    public float scaleFactor = 1.5f;
    //private bool scalingUp = true;
    public float scaleSpeed = 25f;
    //public float scaleRate;
    //private float scaleTimer;

    public void Initilialize(GameObject obj)
    {
        this.obj = obj;
        startScale = obj.transform.localScale;
        endScale = new Vector3(startScale.x * scaleFactor, startScale.y * scaleFactor, startScale.z * scaleFactor);
    }

    public void ScaleUp()
    {
        obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, endScale, scaleSpeed * Time.deltaTime);
    }

    public void ScaleDown()
    {
        obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, startScale, scaleSpeed * Time.deltaTime);
    }
}

public enum Position
{
    FighterHead,
    FighterBody,
    Dummy
}

public class AnimationScript : MonoBehaviour {

    private ReplayManager replayManager;

    private float bpm = 80f;
    public Position position;
    private Position currentPosition;

    [Header("Feedback Options")]
    public bool useVisualFeedback = true;
    public bool useAudioFeedback = true;
    public bool useHapticFeedback = true;

    [Header("Interval Length")]
    public float shortPause = 2f;
    public float longPause = 4f;
    public float lubPhase = 1f;
    public float dubPhase = 1f;

    private GameObject heart;
    private GameObject anchor;

    [Header("Fighter Anchor")]
    public GameObject fighter;

    private float beatInterval;
    private float timeSinceLastInterval = 0.0f;
    private bool isLubPhaseActive = true;
    private bool isShortPauseActive = false;

    public VisualFeedback visualFeedback;
    public AudioFeedback audioFeedback;
    public HapticFeedback hapticFeedback;

    // Use this for initialization
    void Start () 
    {
        replayManager = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayManager>();
        currentPosition = position;
        SetAnchorAndHeart();
        visualFeedback.Initilialize(heart);
        audioFeedback.Initialize(heart);
        hapticFeedback.Initialize();
    }
    /*
	// Update is called once per frame
	void Update () {

        bpm = replayManager.GetCurrentHeartRate();
        if(bpm < 0) { bpm = 80; }
        
        if(isScaling)
        {
            scaleTimer += Time.deltaTime;
            endScale = new Vector3(startScale.x * scaleFactor, startScale.y * scaleFactor, startScale.z * scaleFactor);

            if (scalingUp)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
            }
            else if (!scalingUp)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
            }

            if(scaleTimer >= scaleRate / GetBPMValue(bpm))
            {
                if (scalingUp) { scalingUp = false; }
                else if (!scalingUp) { scalingUp = true; }
                scaleTimer = 0;
            }
        }

        if(isPlayingSound)
        {
            if (!audioSource.isPlaying) { audioSource.Play(); }
            audioSource.pitch = GetBPMValue(bpm);
        }
        if(!isPlayingSound && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
	}
    */

    private void Update()
    {
        bpm = replayManager.GetCurrentHeartRate();

        if(currentPosition != position)
        {
            anchor.SetActive(false);

            SetAnchorAndHeart();
            visualFeedback.Initilialize(heart);
            audioFeedback.Initialize(heart);
        }
        
        RepositionHeartSymbol();

        if (bpm <= 0f) { return; }

        float intervalLength = lubPhase + dubPhase + shortPause + longPause;
        beatInterval = 60.0f / bpm / intervalLength;

        timeSinceLastInterval += Time.deltaTime;

        if (isLubPhaseActive) //start (lub-phase)
        {
                Lub();
                isLubPhaseActive = false;
                isShortPauseActive = true;
        }
        else if (isShortPauseActive) //after lub-phase (short pause)
        {
            ShortPause();
            if (timeSinceLastInterval >= beatInterval * (lubPhase + shortPause)) //dub phase
            {
                Dub();
                isShortPauseActive = false;
            }
        }
        else //after dub-phase (long pause)
        {
            LongPause();
            if (timeSinceLastInterval >= beatInterval * intervalLength) //end of interval
            {
                timeSinceLastInterval = 0;
                isLubPhaseActive = true;
            }
        }
    }

    void SetAnchorAndHeart()
    {
        switch (position)
        {
            case Position.Dummy:
                anchor = transform.Find("Dummy Anchor").gameObject;
                break;
            case Position.FighterHead:
                if (fighter != null)
                {
                    anchor = fighter.transform.Find("Head").Find("Fighter Anchor").gameObject;
                }
                break;
            case Position.FighterBody:
                if (fighter != null)
                {
                    anchor = fighter.transform.Find("Head").Find("Body").Find("Fighter Anchor").gameObject;
                }
                break;
        }

        anchor.SetActive(true);
        heart = anchor.transform.Find("Heart Symbol").gameObject;
        currentPosition = position;
    }

    private void Lub()
    {
        if(useVisualFeedback) visualFeedback.ScaleUp();
        if(useHapticFeedback) hapticFeedback.TriggerHaptic();
        if(useAudioFeedback) audioFeedback.PlayLub();

    }

    private void Dub()
    {
        if (useVisualFeedback) visualFeedback.ScaleUp();
        if (useHapticFeedback) hapticFeedback.TriggerHaptic();
        if (useAudioFeedback) audioFeedback.PlayDub();
    }

    private void ShortPause()
    {
        if (useVisualFeedback) visualFeedback.ScaleDown();
    }

    private void LongPause()
    {
        if (useVisualFeedback) visualFeedback.ScaleDown();
    }

    private void RepositionHeartSymbol()
    {
        if (anchor == null) return;
        anchor.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
        anchor.transform.localEulerAngles = new Vector3(0, anchor.transform.eulerAngles.y, 0);
    }


        public float GetBPMValue(float bpm)
    {
        return (bpm / 80);
    }

    public float GetBPM()
    {
        return bpm;
    }
}
