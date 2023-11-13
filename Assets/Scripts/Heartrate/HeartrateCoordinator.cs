using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections.Generic;
using System;

[System.Serializable]
public class HapticFeedback
{
    [SerializeField] private XRBaseController leftXRController, rightXRController;
    [Range(0f, 1f)]
    public float intensity = 0.5f;
    [Range(0.01f, 0.2f)]
    public float duration = 0.1f;

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
        audioSource.clip = null;
    }
}

[System.Serializable]
public class VisualFeedback
{
    private Vector3 startScale;
    private Vector3 endScale;
    private GameObject obj;

    public float scaleFactor = 1.5f;
    public float scaleSpeed = 25f;

    public void Initialize(GameObject obj)
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
    
    public static HeartrateCoordinator Instance {  get; private set; }

    public event EventHandler OnVisualFeedbackChanged;
    public event EventHandler OnAudioFeedbackChanged;
    public event EventHandler OnHapticFeedbackChanged;
    public event EventHandler OnPositionChanged;

    private ReplayManager replayManager;
    [SerializeField] private TMP_Text heartrateDisplay;

    private float newBPM = 80f;
    private float lastBPM = 80f;
    public Position position;
    [SerializeField] private Position currentPosition;

    [Header("Feedback Options")]
    public bool useVisualFeedback = true;
    public bool useAudioFeedback = false;
    public bool useHapticFeedback = false;

    [Header("Interval Length")]
    [SerializeField] private float shortPause = 2f;
    [SerializeField] private float longPause = 4f;
    [SerializeField] private float lubPhase = 1f;
    [SerializeField] private float dubPhase = 1f;

    [Header("Dummy Heart")]
    [SerializeField] private HeartAnchor dummyAnchor;

    [Header("Body Heart")]
    [SerializeField] private HeartAnchor headFighterAnchor;

    [Header("Head Heart")]
    [SerializeField] private HeartAnchor bodyFighterAnchor;

    private float beatInterval;

    public AudioFeedback audioFeedback;
    public HapticFeedback hapticFeedback;
    public VisualFeedback visualFeedback;

    private HeartAnchor activeAnchor;

    private List<IHeartrateObserver> observers = new List<IHeartrateObserver>();

    private IHeartbeatState currentState;

    public float CurrentLubLength { get; private set; }
    public float CurrentDubLength { get; private set; }
    public float CurrentShortPauseLength { get; private set; }
    public float CurrentLongPauseLength { get; private set; }

    private bool visualFeedbackActivated;
    private bool audioFeedbackActivated;
    private bool hapticFeedbackActivated;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one HeartrateCoordinator found");
        }

        visualFeedbackActivated = useVisualFeedback;
        audioFeedbackActivated = useAudioFeedback;
        hapticFeedbackActivated = useHapticFeedback;
    }
    void Start () 
    {
        replayManager = ReplayManager.Instance;
        if(replayManager == null)
        {
            Debug.LogError("No ReplayManager found");
        }

        replayManager.OnReplayLoaded += ReplayManager_OnReplayLoaded;
        replayManager.OnReplayUnloaded += ReplayManager_OnReplayUnloaded;
        
        currentPosition = position;
        RegisterObserver(dummyAnchor);
        RegisterObserver(bodyFighterAnchor);
        RegisterObserver(headFighterAnchor);
        //SetActiveAnchorByPosition(position);
        //hapticFeedback.Initialize();
        SetState(new WaitingHeartbeatState());
    }

    private void ReplayManager_OnReplayUnloaded(object sender, EventArgs e)
    {
        visualFeedback.End();
        audioFeedback.End();
        SetState(new WaitingHeartbeatState());
        newBPM = lastBPM = 80;
        activeAnchor.DeactivateSymbol();
        activeAnchor.DeactivateMenu();
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        SetActiveAnchorByPosition(position);
        SetState(new IdleHeartbeatState());
    }

    public void SetState(IHeartbeatState newState)
    {
        currentState = newState;
        newState.Enter(this);
    }

    public void ResetCoordinator()
    {
        visualFeedback.End();
        audioFeedback.End();
        visualFeedbackActivated = useVisualFeedback;
        audioFeedbackActivated = useAudioFeedback;
        hapticFeedbackActivated = useHapticFeedback;
        SetState(new WaitingHeartbeatState());
        position = Position.Dummy;
        ChangeActiveAnchor();
        newBPM = lastBPM = 80;
    }

    private void Update()
    {
        HandleBPM();

        if(currentPosition != position)
        {
            ChangeActiveAnchor();
        }
        currentState.Update(Instance);
        NotifyObservers();
    }

    private void HandleBPM()
    {      
        newBPM = replayManager.GetCurrentHeartRate();
        if (newBPM < 0f) { return; }
        heartrateDisplay.text = newBPM.ToString();
        if (newBPM != lastBPM)
        {
            UpdatePhaseLengths();
            lastBPM = newBPM;
        }
    }
    void ChangeActiveAnchor()
    {
        if (activeAnchor != null) 
        { 
            ResetAnchor(activeAnchor); 
        }
        SetActiveAnchorByPosition(position);
    }
    private void ResetAnchor(HeartAnchor anchor)
    {
        if (anchor == null)
        {
            return;
        }
        activeAnchor.DeactivateSymbol();
    }
    private void SetActiveAnchorByPosition(Position position)
    {
        // if (!replayManager.FileIsLoaded()) return;
        switch (position)
        {
            case Position.Dummy:
                if (dummyAnchor != null)
                {
                    activeAnchor = dummyAnchor;
                    currentPosition = position;
                }
                break;
            case Position.FighterHead:
                if (headFighterAnchor != null)
                {
                    activeAnchor = headFighterAnchor;
                    currentPosition = position;
                }
                break;
            case Position.FighterBody:
                if (bodyFighterAnchor != null)
                {
                    activeAnchor = bodyFighterAnchor;
                    currentPosition = position;
                }
                break;
        }
        activeAnchor.ActivateSymbol();
        NotifyObservers();

        OnPositionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdatePhaseLengths()
    {
        float intervalLength = lubPhase + dubPhase + shortPause + longPause;
        beatInterval = 60.0f / newBPM / intervalLength;

        CurrentLubLength = beatInterval * lubPhase;
        CurrentDubLength = beatInterval * dubPhase;
        CurrentShortPauseLength = beatInterval * shortPause;
        CurrentLongPauseLength = beatInterval * longPause;
    }

    public void RegisterObserver(IHeartrateObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
            observer.Initialize(audioFeedback, visualFeedback);
        }
    }
    public void RemoveObserver(IHeartrateObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }
    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.UpdateHeartbeat(currentState, audioFeedbackActivated, hapticFeedbackActivated, visualFeedbackActivated);
        }
    }

    public bool ChangeAudioFeedback()
    {
        audioFeedbackActivated = !audioFeedbackActivated;

        OnAudioFeedbackChanged?.Invoke(this, EventArgs.Empty);
        return audioFeedbackActivated;
    }

    public bool ChangeVisualFeedback()
    {
        visualFeedbackActivated = !visualFeedbackActivated;

        OnVisualFeedbackChanged?.Invoke(this, EventArgs.Empty);
        return visualFeedbackActivated;
    }

    public bool ChangeHapticFeedback()
    {
        hapticFeedbackActivated = !hapticFeedbackActivated;

        OnHapticFeedbackChanged?.Invoke(this, EventArgs.Empty);
        return hapticFeedbackActivated;
    }

    public bool IsAudioFeedbackActivated()
    {
        return audioFeedbackActivated;
    }
    public bool IsVisualFeedbackActivated()
    {
        return visualFeedbackActivated;
    }
    public bool IsHapticFeedbackActivated()
    {
        return hapticFeedbackActivated;
    }

    public Position GetCurrentPosition()
    {
        return currentPosition;
    }
}
