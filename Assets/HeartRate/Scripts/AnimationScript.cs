using UnityEngine;
using System.Collections;

public class AnimationScript : MonoBehaviour {

    public GameObject replayManagerObject;
    private ReplayManager replayManager;

    public bool isAnimated = false;

    public bool isRotating = false;
    public bool isFloating = false;
    public bool isScaling = false;
    public bool isPlayingSound = false;

    public Vector3 rotationAngle;
    public float rotationSpeed;

    public float floatSpeed;
    private bool goingUp = true;
    public float floatRate;
    private float floatTimer;
   
    public Vector3 startScale;
    public Vector3 endScale;
    public float scaleFactor = 1.5f;

    private bool scalingUp = true;
    public float scaleSpeed;
    public float scaleRate;
    private float scaleTimer;

    private AudioSource audioSource;
    public float bpm = 80f;

    // Use this for initialization
    void Start () 
    {
        replayManager = replayManagerObject.GetComponent<ReplayManager>();
        startScale = transform.localScale;
        endScale = new Vector3(startScale.x * scaleFactor, startScale.y * scaleFactor, startScale.z * scaleFactor);
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.loop = true;
    }
	
	// Update is called once per frame
	void Update () {

        bpm = replayManager.GetCurrentHeartRate();
        if(bpm < 0) { bpm = 80; }
        
        if(isAnimated)
        {
            if(isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            if(isFloating)
            {
                floatTimer += Time.deltaTime;
                Vector3 moveDir = new Vector3(0.0f, 0.0f, floatSpeed);
                transform.Translate(moveDir);

                if (goingUp && floatTimer >= floatRate)
                {
                    goingUp = false;
                    floatTimer = 0;
                    floatSpeed = -floatSpeed;
                }

                else if(!goingUp && floatTimer >= floatRate)
                {
                    goingUp = true;
                    floatTimer = 0;
                    floatSpeed = +floatSpeed;
                }
            }

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

    public float GetBPMValue(float bpm)
    {
        return (bpm / 80);
    }
}
