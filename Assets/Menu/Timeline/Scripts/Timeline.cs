using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public class Timeline : MonoBehaviour
{
    public float threshold = 60f;
     
    private Slider timeline;
    private Button pauseButton;
    private Button playButton;
    private Button backwardsButton;
    private Button fastPlayButton;
    private Button stopButton;
    private ReplayManager replayManager;

    //Heartrate
    private AnimationScript heartrateAnimator;
    private Button audioFeedbackButton;
    private Button visualFeedbackButton;
    private Button hapticFeedbackButton;
    private TMP_Dropdown heartPositionDropdown;
    private TMP_Text heartrateInfo;

    private GameObject activeObject;


    // Markers
    [Header("Marker")]
    public GameObject marker;
    public Color markerColor = Color.white;
    public float highlightThreshold = 120f;
    private List<int> shownHighlights = new List<int>();
    private Dictionary<int, GameObject> shownMarkers = new Dictionary<int, GameObject>();
    public bool showSuccsessfulArmCollisionHighlights;
    public bool showUnsuccsessfulArmCollisionHighlights;
    public bool showSuccsessfulFightCollisionHighlights;
    public bool showUnsuccsessfulFightCollisionHighlights;
    private Button sacButton, sfcButton, uacButton, ufcButton;


    private bool isDragged = false;
    private bool wasRunning;

    private int frame;
    void Start()
    {
        timeline = transform.Find("Slider").GetComponent<Slider>();
        pauseButton = transform.Find("Buttons").Find("PauseButton").GetComponent<Button>();
        playButton = transform.Find("Buttons").Find("PlayButton").GetComponent<Button>();
        backwardsButton = transform.Find("Buttons").Find("Backwards").GetComponent<Button>();
        fastPlayButton = transform.Find("Buttons").Find("FastForward").GetComponent<Button>();
        stopButton = transform.Find("Buttons").Find("StopButton").GetComponent<Button>();
        replayManager = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayManager>();
        timeline.maxValue = (replayManager.GetReplayLength()) - 2;

        pauseButton.onClick.AddListener(OnPauseButtonClick);
        playButton.onClick.AddListener(OnPlayButtonClick);
        stopButton.onClick.AddListener(OnStopButtonClick);
        backwardsButton.onClick.AddListener(OnBackwardsButtonClick);
        fastPlayButton.onClick.AddListener(OnFastPlayButtonPressed);

        heartrateAnimator = GameObject.FindGameObjectWithTag("Heartrate").GetComponent<AnimationScript>();
        audioFeedbackButton = transform.Find("Heartrate Buttons").Find("Audio Feedback Button").GetComponent<Button>();
        visualFeedbackButton = transform.Find("Heartrate Buttons").Find("Visual Feedback Button").GetComponent<Button>();
        hapticFeedbackButton = transform.Find("Heartrate Buttons").Find("Haptic Feedback Button").GetComponent<Button>();
        heartPositionDropdown = transform.Find("Heartrate Buttons").Find("Heart Position Dropdown").GetComponent<TMP_Dropdown>();
        heartrateInfo = transform.Find("Heartrate Info").Find("Info").GetComponent<TMP_Text>();

        audioFeedbackButton.onClick.AddListener(OnAudioFeedbackButtonPressed);
        visualFeedbackButton.onClick.AddListener(OnVisualFeedbackButtonPressed);
        hapticFeedbackButton.onClick.AddListener(OnHapticFeedbackButtonPressed);
        heartPositionDropdown.onValueChanged.AddListener(OnHeartPositionChanged);

        sacButton = transform.Find("Marker Options").Find("SAC").GetComponent<Button>();
        sfcButton = transform.Find("Marker Options").Find("SFC").GetComponent<Button>();
        uacButton = transform.Find("Marker Options").Find("UAC").GetComponent<Button>();
        ufcButton = transform.Find("Marker Options").Find("UFC").GetComponent<Button>();

        sacButton.onClick.AddListener(OnSACButtonClick);
        sfcButton.onClick.AddListener(OnSFCButtonClick);
        uacButton.onClick.AddListener(OnUACButtonClick);
        ufcButton.onClick.AddListener(OnUFCButtonClick);

        SetButtonActive(playButton);
        frame = 6;
        replayManager.LoadFrame(6);
        SetupHeartrateGraph();

        SwitchSuccsessfulArmCollisionHighlights();
        SwitchUnsuccsessfulArmCollisionHighlights();
        SwitchSuccsessfulFightCollisionHighlights();
        SwitchUnsuccsessfulFightCollisionHighlights();
        
    }

    /*
    public void CreateHighlights()
    {
        if(showArmCollisionHighlights && !armCollisionHighlightsShown)
        {
            foreach(KeyValuePair<int, ArmCollisionLog[]> log in replayManager.GetArmCollisionDic())
            {
                SetMark(log.Key, markerColor);
            }
            armCollisionHighlightsShown = true;
        }

        if(showFightCollisionHighlights && !fightCollisionHighlightsShown)
        {
            foreach (KeyValuePair<int, FightCollisionLog[]> log in replayManager.GetFightCollisionDic())
            {
                SetMark(log.Key, markerColor);
            }
            fightCollisionHighlightsShown= true;
        }
    }

    public void RemoveHighlights()
    {
        if(armCollisionHighlightsShown && !showArmCollisionHighlights)
        {
            foreach (KeyValuePair<int, ArmCollisionLog[]> log in replayManager.GetArmCollisionDic())
            {
                RemoveMark(log.Key);
            }
            armCollisionHighlightsShown= false;
        }

        if(fightCollisionHighlightsShown && !showFightCollisionHighlights)
        {
            foreach (KeyValuePair<int, FightCollisionLog[]> log in replayManager.GetFightCollisionDic())
            {
                RemoveMark(log.Key);
            }
            fightCollisionHighlightsShown= false;
        }
    }
    */

    public void CreateArmCollisionHighlights(Dictionary<int, ArmCollisionLog[]> armCollisionDic)
    {
        foreach (KeyValuePair<int, ArmCollisionLog[]> log in armCollisionDic)
        {
            SetMark(log.Key, markerColor);
        }
    }
    public void SwitchSuccsessfulArmCollisionHighlights() 
    {
        if(showSuccsessfulArmCollisionHighlights)
        {
            CreateArmCollisionHighlights(replayManager.GetSuccsessfulArmCollisionDic());
            SetButtonActive(sacButton);
        }
        else
        {
            RemoveArmCollisionHighlights(replayManager.GetSuccsessfulArmCollisionDic() );
            SetButtonInactive(sacButton);
        }
        
    }
    public void SwitchUnsuccsessfulArmCollisionHighlights() 
    {
        if (showUnsuccsessfulArmCollisionHighlights)
        {
            CreateArmCollisionHighlights(replayManager.GetUnsuccsessfulArmCollisionDic());
            SetButtonActive(uacButton);
        }
        else
        {
            RemoveArmCollisionHighlights(replayManager.GetUnsuccsessfulArmCollisionDic());
            SetButtonInactive(uacButton);
        }
        
    }

    public void RemoveArmCollisionHighlights(Dictionary<int, ArmCollisionLog[]> armCollisionDic)
    {
        foreach (KeyValuePair<int, ArmCollisionLog[]> log in armCollisionDic)
        {
            RemoveMark(log.Key);
        }
    }

    public void CreateFightCollisionHighlights(Dictionary<int, FightCollisionLog[]> fightCollisionDic)
    {
        foreach (KeyValuePair<int, FightCollisionLog[]> log in fightCollisionDic)
        {
            SetMark(log.Key, markerColor);
        }
    }
    public void SwitchSuccsessfulFightCollisionHighlights()
    {
        if (showSuccsessfulFightCollisionHighlights)
        {
            CreateFightCollisionHighlights(replayManager.GetSuccsessfulFightCollisionDic());
            SetButtonActive(sfcButton);
        }
        else
        {
            RemoveFightCollisionHighlights(replayManager.GetSuccsessfulFightCollisionDic());
            SetButtonInactive(sfcButton);
        }

    }
    public void SwitchUnsuccsessfulFightCollisionHighlights()
    {
        if (showUnsuccsessfulFightCollisionHighlights)
        {
            CreateFightCollisionHighlights(replayManager.GetUnsuccsessfulFightCollisionDic());
            SetButtonActive(ufcButton);
        }
        else
        {
            RemoveFightCollisionHighlights(replayManager.GetUnsuccsessfulFightCollisionDic());
            SetButtonInactive(ufcButton);
        }

    }

    public void OnSACButtonClick()
    {
        if (showSuccsessfulArmCollisionHighlights)
        {
            showSuccsessfulArmCollisionHighlights = false;
        }
        else
        {
            showSuccsessfulArmCollisionHighlights= true;
        }
        SwitchSuccsessfulArmCollisionHighlights();
    }

    public void OnUACButtonClick()
    {
        if (showUnsuccsessfulArmCollisionHighlights)
        {
            showUnsuccsessfulArmCollisionHighlights = false;
        }
        else
        {
            showUnsuccsessfulArmCollisionHighlights = true;
        }
        SwitchUnsuccsessfulArmCollisionHighlights();
    }

    public void OnSFCButtonClick()
    {
        if (showSuccsessfulFightCollisionHighlights)
        {
            showSuccsessfulFightCollisionHighlights = false;
        }
        else
        {
            showSuccsessfulFightCollisionHighlights = true;
        }
        SwitchSuccsessfulFightCollisionHighlights();
    }
    public void OnUFCButtonClick()
    {
        if (showUnsuccsessfulFightCollisionHighlights)
        {
            showUnsuccsessfulFightCollisionHighlights = false;
        }
        else
        {
            showUnsuccsessfulFightCollisionHighlights = true;
        }
        SwitchUnsuccsessfulFightCollisionHighlights();
    }


    public void RemoveFightCollisionHighlights(Dictionary<int, FightCollisionLog[]> fightCollisionDic)
    {
        foreach (KeyValuePair<int, FightCollisionLog[]> log in fightCollisionDic)
        {
            RemoveMark(log.Key);
        }
    }

    public void OnAudioFeedbackButtonPressed()
    {
        if (isDragged) { return; }
        if (heartrateAnimator.useAudioFeedback) { heartrateAnimator.useAudioFeedback = false; }
        else { heartrateAnimator.useAudioFeedback = true; }
    }
    public void OnVisualFeedbackButtonPressed() 
    {
        if (isDragged) { return; }
        if (heartrateAnimator.useVisualFeedback) { heartrateAnimator.useVisualFeedback = false; }
        else { heartrateAnimator.useVisualFeedback = true; }
    }
    public void OnHapticFeedbackButtonPressed() 
    {
        if (isDragged) { return; }
        if (heartrateAnimator.useHapticFeedback) { heartrateAnimator.useHapticFeedback = false; }
        else { heartrateAnimator.useHapticFeedback = true; }
    }
    public void OnHeartPositionChanged(int value) 
    {
        if (isDragged) { return ; }
        switch (value)
        {
            case 0:
                heartrateAnimator.position = Position.Dummy; break;
            case 1:
                heartrateAnimator.position = Position.FighterHead; break;
            case 2:
                heartrateAnimator.position = Position.FighterBody; break;
        }
    }

    public void SetButtonActive(Button button)
    {
        button.GetComponent<Image>().color = Color.grey;
    }

    public void SetButtonInactive(Button button)
    {
        button.GetComponent<Image>().color = Color.white;
    }

    public void OnPauseButtonClick()
    {
        if (isDragged)
        {
            return;
        }
        if (replayManager.IsRunning())
        {
            replayManager.Pause();
        }
    }

    public void OnPlayButtonClick()
    {
        if (isDragged)
        {
            return;
        }
        if (!replayManager.IsRunning())
        {
            replayManager.Play();
        }
        
    }

    public void OnStopButtonClick()
    {
        if (isDragged)
        {
            return;
        }
        replayManager.Stop();
    }

    public void OnFastPlayButtonPressed()
    {
        if (isDragged) { return; }
        if(replayManager.GetPlaySpeed() == 1)
        {
            replayManager.SetPlaySpeed(2);
        }
        else
        {
            replayManager.SetPlaySpeed(1);
        }
    }

    public void OnBackwardsButtonClick()
    {
        if (isDragged) { return; }
        if (replayManager.GetPlayDirection() == 1)
        {
            replayManager.SetPlayDirection(-1);
        }
        else
        {
            replayManager.SetPlayDirection(1);;
        }
    }

    public void StartDrag()
    {
        wasRunning = replayManager.IsRunning();
        replayManager.Pause();
        isDragged = true;
    }

    public void HandleSliderInteraction(BaseEventData eventData)
    {
        if (!isDragged)
        {
            StartDrag();
        }
    }

    public void EndDrag()
    {
        isDragged = false;
        if (wasRunning)
        {
            replayManager.Play();
        }
    }


    public void OnTimelineValueChanged(float value)
    {
        if (!isDragged) { return; }

        int closestHighlight = 6;
        if(shownHighlights.Count != 0) { closestHighlight = shownHighlights.OrderBy(x => Mathf.Abs((long)x - value)).First(); }
        

        // Überprüfe, ob ein Highlight in der Nähe des neuen Werts existiert
        if (Mathf.Abs(closestHighlight - value) <= highlightThreshold)
        {
            // Setze den Replay-Zeitpunkt auf den nächsten Highlight-Wert
            frame = closestHighlight;
            if(frame < 6) { frame = 6; }
            replayManager.SetReplayTime(frame);
            replayManager.LoadFrame(frame);
        }
        else
        {
            // Bestimme die Differenz zwischen dem aktuellen Wert und dem neuen Wert
            float valueDifference = Mathf.Abs(frame - value);

            // Überprüfe, ob die Differenz größer als eine bestimmte Schwelle ist
            if (valueDifference > threshold)
            {
                // Setze den Replay-Zeitpunkt auf den Wert des Timelines
                frame = (int)value;
                replayManager.SetReplayTime(frame);
                replayManager.LoadFrame(frame);
            }
        }
    }

    public void IsButtonActive(Button button, bool active)
    {
        if (active)
        {
            SetButtonActive(button);
        }
        else
        {
            SetButtonInactive(button);
        }
    }

    void Update()
    {
        IsButtonActive(playButton, replayManager.IsRunning());
        IsButtonActive(pauseButton, !replayManager.IsRunning());
        IsButtonActive(backwardsButton, replayManager.GetPlayDirection() == -1);
        IsButtonActive(fastPlayButton, replayManager.GetPlaySpeed() > 1);

        IsButtonActive(audioFeedbackButton, heartrateAnimator.useAudioFeedback);
        IsButtonActive(visualFeedbackButton, heartrateAnimator.useVisualFeedback);
        IsButtonActive(hapticFeedbackButton, heartrateAnimator.useHapticFeedback);

        UpdateHeartrateInfo();
        timeline.value = replayManager.GetReplayTime();
    }

    public Vector2 GetMarkerPosition(float value)
    {
        float normalizedValue = (value - timeline.minValue) / (timeline.maxValue - timeline.minValue);
        float timelineWidth = timeline.GetComponent<RectTransform>().rect.width;
        float positionX = timeline.transform.localPosition.x - timelineWidth / 2f + normalizedValue * timelineWidth;

        Vector3 position = new Vector3(positionX, timeline.transform.localPosition.y, timeline.transform.localPosition.z);
        Vector2 markerPosition = new Vector2(positionX, timeline.transform.localPosition.y);

        return markerPosition;
    }

    public void SetMark(int frame, Color color)
    {
        if (frame < 6) { return; }
        if (shownHighlights.Contains(frame))
        {
            shownHighlights.Add(frame);
            return;
        }
        else
        {
            shownHighlights.Add(frame);
            Vector2 markerPosition = GetMarkerPosition(frame);
            GameObject mark = Instantiate(marker, transform, false);
            mark.GetComponent<RectTransform>().localPosition = markerPosition;
            mark.GetComponent<Image>().color = color;
            shownMarkers.Add(frame, mark);
        }
    }

    public void RemoveMark(int frame)
    {
        if (!shownHighlights.Contains(frame)) return;
        shownHighlights.Remove(frame);
        if (!shownHighlights.Contains(frame))
        {
            Destroy(shownMarkers[frame]);
            shownMarkers.Remove(frame);
        }

    }

    public void SetupHeartrateGraph()
    {
        LineRenderer lineRenderer = transform.Find("Slider").Find("Background").GetComponent<LineRenderer>();
        lineRenderer.positionCount = replayManager.GetHRLog().Count+1;
        int point = 0;
        Vector3 position = new Vector3(-400, -20, 1);
        lineRenderer.SetPosition(point, position);

        point++;

        float normalizedXValue = 800 / (timeline.maxValue - timeline.minValue);

        foreach(KeyValuePair<int, HRLog> log in replayManager.GetHRLog())
        {
            position.x = (log.Key * normalizedXValue)-400;
            position.y = (log.Value.heartRate)-100;
            lineRenderer.SetPosition(point, position);
            point++;
        }
    }

    public void UpdateHeartrateInfo()
    {
        heartrateInfo.text = heartrateAnimator.GetBPM().ToString();
    }

}