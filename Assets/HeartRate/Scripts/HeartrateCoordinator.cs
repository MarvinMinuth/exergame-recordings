using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using TMPro;

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

    public void End()
    {
        audioSource.Stop();
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

    public void End()
    {
        obj.transform.localScale = startScale;
    }
}

public enum Position
{
    FighterHead,
    FighterBody,
    Dummy
}

public class HeartrateCoordinator : MonoBehaviour {

    [SerializeField] private ReplayManager replayManager;
    [SerializeField] private TMP_Text heartrateDisplay;

    private float bpm = 80f;
    public Position position;
    private Position currentPosition;

    [Header("Feedback Options")]
    public bool useVisualFeedback = true;
    public bool useAudioFeedback = false;
    public bool useHapticFeedback = false;

    [Header("Interval Length")]
    public float shortPause = 2f;
    public float longPause = 4f;
    public float lubPhase = 1f;
    public float dubPhase = 1f;

    [Header("Dummy Anchor")]
    public GameObject dummyHeart;
    public HeartAnchor dummyAnchor;
    //public HeartMenu dummyHeartMenu;

    [Header("Fighter Anchor")]
    public HeartAnchor headFighterAnchor;
    public GameObject headFighterHeart;
    public HeartAnchor bodyFighterAnchor;
    public GameObject bodyFighterHeart;
    //public HeartMenu fighterHeartMenu;

    private float beatInterval;
    private float timeSinceLastInterval = 0.0f;
    private bool isLubPhaseActive = true;
    private bool isShortPauseActive = false;

    public VisualFeedback visualFeedback;
    public AudioFeedback audioFeedback;
    public HapticFeedback hapticFeedback;

    private bool isInitialized = false;
    private HeartAnchor activeAnchor;
    private GameObject activeHeart;

    void Start () 
    {
        replayManager = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayManager>();
        currentPosition = position;       
    }

    public void Setup()
    {
        SetAnchorAndHeart();
        visualFeedback.Initilialize(activeHeart);
        audioFeedback.Initialize(activeHeart);
        hapticFeedback.Initialize();
        isInitialized = true;
    }

    public void ResetCoordinator()
    {
        isInitialized = false;
        visualFeedback.End();
        audioFeedback.End();
        position = Position.Dummy;
        currentPosition = position;
        bpm = 80;
        timeSinceLastInterval = 0.0f;
        isLubPhaseActive = true;
        isShortPauseActive = false;
    }

    private void Update()
    {
        if(!isInitialized) { return; }
        bpm = replayManager.GetCurrentHeartRate();
        heartrateDisplay.text = bpm.ToString();

        if(currentPosition != position)
        {
            visualFeedback.End();
            audioFeedback.End();
            SetAnchorAndHeart();
            visualFeedback.Initilialize(activeHeart);
            audioFeedback.Initialize(activeHeart);
        }

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
        if(activeAnchor != null) { activeAnchor.DeactivateSymbol(); }
        if (!replayManager.FileIsLoaded()) return;
        switch (position)
        {
            case Position.Dummy:
                activeAnchor = dummyAnchor;
                break;
            case Position.FighterHead:
                if (headFighterAnchor != null)
                {
                    activeAnchor = headFighterAnchor;
                }
                break;
            case Position.FighterBody:
                if (bodyFighterAnchor != null)
                {
                    activeAnchor = bodyFighterAnchor;
                }
                break;
        }
        activeHeart = activeAnchor.GetHeartSymbol();
        activeAnchor.ActivateSymbol();
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
    public float GetBPMValue(float bpm)
    {
        return (bpm / 80);
    }
    public float GetBPM()
    {
        return bpm;
    }
}
