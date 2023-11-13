using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuTimeline : Timeline
{
    // Slider
    [Header("Slider")]
    [SerializeField] private float threshold = 60f; 
    [SerializeField] private Slider timelineSlider;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image handleImage;

    //Replay
    private ReplayController replayController;

    // Markers
    [Header("Marker")]
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private Color markerColor = Color.white;
    [SerializeField] private float highlightThreshold = 120f;
    private List<int> shownHighlights = new List<int>();
    private Dictionary<int, GameObject> shownMarkers = new Dictionary<int, GameObject>();
    [SerializeField] private bool showUnsuccsessfulFightCollisionHighlights;
    private int minFrame, maxFrame;
    private int activatedButton;
    [SerializeField] private int windowSize = 180;

    private bool isDragged = false;
    private bool wasRunning;

    private int activeFrame;
    protected override void Start()
    {
        base.Start();
        replayController = ReplayController.Instance;

        replayController.OnReplayControllerLoaded += ReplayController_OnReplayControllerLoaded;
        replayController.OnReplayControllerUnload += ReplayController_OnReplayControllerUnload;
        replayController.OnStop += ReplayController_OnStop;

        timelineSlider = transform.Find("Slider").GetComponent<Slider>();

        timelineSlider.maxValue = 0;
    }

    private void ReplayController_OnStop(object sender, EventArgs e)
    {
        DeactivateActiveMarker();
    }

    protected override void ReplayController_OnReplayControllerUnload(object sender, EventArgs e)
    {
        DeleteTimeline();
    }

    private void ReplayController_OnReplayControllerLoaded(object sender, EventArgs e)
    {
        SetupTimeline();
    }

    private void SetupTimeline()
    {
        timelineSlider.maxValue = (replayController.GetReplayLength()) - 2;

        SwitchUnsuccsessfulFightCollisionHighlights();

        maxFrame = (int)timelineSlider.maxValue;
        minFrame = (int)timelineSlider.minValue;
        activeFrame = minFrame;
    }

    private void DeleteTimeline()
    {
        timelineSlider.value = 0;
        timelineSlider.maxValue = 0;
        RemoveAllMarkers();
    }

    public void CreateFightCollisionHighlights(Dictionary<int, FightCollisionLog[]> fightCollisionDic)
    {
        foreach (KeyValuePair<int, FightCollisionLog[]> log in fightCollisionDic)
        {
            SetMark(log.Key, markerColor);
        }
    }

    public void SwitchUnsuccsessfulFightCollisionHighlights()
    {
        if (showUnsuccsessfulFightCollisionHighlights)
        {
            CreateFightCollisionHighlights(ReplayManager.Instance.GetUnsuccsessfulFightCollisionDic());
        }
        else
        {
            RemoveFightCollisionHighlights(ReplayManager.Instance.GetUnsuccsessfulFightCollisionDic());
        }

    }

    public void RemoveFightCollisionHighlights(Dictionary<int, FightCollisionLog[]> fightCollisionDic)
    {
        foreach (KeyValuePair<int, FightCollisionLog[]> log in fightCollisionDic)
        {
            RemoveMark(log.Key);
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

    public override void StartDrag()
    {
        TriggerOnTimelineUsed(this);

        wasRunning = replayController.IsRunning();
        replayController.Pause();
        isDragged = true;
        replayController.SetReceivingInput(true);
    }

    public void HandleSliderInteraction(BaseEventData eventData)
    {
        if (!isDragged)
        {
            StartDrag();
        }
    }

    public override void EndDrag()
    {
        isDragged = false;
        if (wasRunning)
        {
            replayController.Play();
        }
        replayController.SetReceivingInput(false);
        TriggerOnTimelineFreed();
    }

    public void OnTimelineValueChanged(float value)
    {
        if (!isDragged) { return; }
        if (value < minFrame) { value  = minFrame; }
        if (value > maxFrame) { value = maxFrame; }

        int closestHighlight = minFrame;
        if(shownHighlights.Count != 0) { closestHighlight = shownHighlights.OrderBy(x => Mathf.Abs((long)x - value)).First(); }
        

        // �berpr�fe, ob ein Highlight in der N�he des neuen Werts existiert
        if (Mathf.Abs(closestHighlight - value) <= highlightThreshold)
        {
            // Setze den Replay-Zeitpunkt auf den n�chsten Highlight-Wert
            activeFrame = closestHighlight;
            replayController.SetFrame(activeFrame);
        }
        else
        {
            // Bestimme die Differenz zwischen dem aktuellen Wert und dem neuen Wert
            float valueDifference = Mathf.Abs(activeFrame - value);

            // �berpr�fe, ob die Differenz gr��er als eine bestimmte Schwelle ist
            if (valueDifference > threshold)
            {
                // Setze den Replay-Zeitpunkt auf den Wert des Timelines
                activeFrame = (int)value;
                replayController.SetFrame(activeFrame);
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
        if (!replayController.IsReplayReady()) { timelineSlider.value = 0; }
        else { timelineSlider.value = replayController.GetFrame(); }
    }

    public Vector2 GetMarkerPosition(float value)
    {
        float normalizedValue = (value - timelineSlider.minValue) / (timelineSlider.maxValue - timelineSlider.minValue);
        float timelineWidth = timelineSlider.GetComponent<RectTransform>().rect.width;
        float positionX = timelineSlider.transform.localPosition.x - timelineWidth / 2f + normalizedValue * timelineWidth;

        Vector3 position = new Vector3(positionX, timelineSlider.transform.localPosition.y, timelineSlider.transform.localPosition.z);
        Vector2 markerPosition = new Vector2(positionX, timelineSlider.transform.localPosition.y);

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
            GameObject mark = Instantiate(markerPrefab, transform, false);
            mark.GetComponent<Marker>().frame = frame;
            mark.GetComponent<RectTransform>().localPosition = markerPosition;
            mark.GetComponent<Image>().color = color;
            shownMarkers.Add(frame, mark);
        }
    }

    public void OnMarkerButtonClick(int frame)
    {
        int oldActivatedButton = activatedButton;
        // deactivate active marker
        DeactivateActiveMarker();

        if (frame != oldActivatedButton)
            // if a new marker is selected, activate it
        {
            activatedButton = frame;
            SetButtonActive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonActive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
            minFrame = frame - (windowSize / 2);
            if (minFrame < timelineSlider.minValue) { minFrame = (int)timelineSlider.minValue; }
            maxFrame = frame + (windowSize / 2);
            if (maxFrame > timelineSlider.maxValue) { maxFrame = (int)timelineSlider.maxValue; }
            replayController.ChangeReplayWindow(minFrame, maxFrame);
            replayController.SetFrame(minFrame);
        }    
    }

    public void DeactivateActiveMarker()
    {
        if (shownMarkers.ContainsKey(activatedButton))
        {
            SetButtonInactive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonInactive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
            replayController.ResetReplayWindow();
        }
        minFrame = (int)timelineSlider.minValue;
        maxFrame = (int)timelineSlider.maxValue;
        activatedButton = 0;
    }

    public void RemoveMark(int frame)
    {
        if (!shownHighlights.Contains(frame)) return;
        shownHighlights.Remove(frame);
        if (!shownHighlights.Contains(frame))
        {
            Destroy(shownMarkers[frame]);
            if (frame == activatedButton)
            {
                minFrame = (int)timelineSlider.minValue;
                maxFrame = (int)timelineSlider.maxValue;
                replayController.ResetReplayWindow();
            }
            shownMarkers.Remove(frame);
        }

    }

    public void RemoveAllMarkers()
    {
        DeactivateActiveMarker();
        foreach (KeyValuePair <int, GameObject> marker in shownMarkers)
        {
            Destroy(marker.Value);
        }
        shownHighlights.Clear();
        shownMarkers.Clear();
    }

    public float GetMaxValue()
    {
        return timelineSlider.maxValue;
    }

    public float GetMinValue()
    {
        return timelineSlider.minValue;
    }

    protected override void Timeline_OnTimelineUsed(object sender, OnTimelineUsedEventArgs e)
    {
        if(e.usedTimeline != this)
        {
            timelineSlider.interactable = false;
            backgroundImage.raycastTarget = false;
            fillImage.raycastTarget = false;
            handleImage.raycastTarget = false;
        }
        
    }
    protected override void Timeline_OnTimelineFreed(object sender, EventArgs e)
    {
        timelineSlider.interactable = true;
        backgroundImage.raycastTarget = true;
        fillImage.raycastTarget = true;
        handleImage.raycastTarget = true;
    }
}