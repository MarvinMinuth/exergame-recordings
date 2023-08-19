using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public class Timeline : MonoBehaviour
{
    public float threshold = 60f;
    public float highlightThreshold = 120f;

    public GameObject marker;
    private Slider timeline;
    private Button pauseButton;
    private Button playButton;
    private Button backwardsButton;
    private Button fastPlayButton;
    private Button stopButton;
    private ReplayManager replayManager;

    private GameObject activeObject;

    private List<int> bottomArmHighlights;

    private List<int> shownHighlights = new List<int>();

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

        bottomArmHighlights = replayManager.GetBottomArmHighlights();
        foreach(int frame in bottomArmHighlights)
        {
            SetMark(frame, Color.white);
        }

        SetButtonActive(playButton);
        frame = 6;
        replayManager.LoadFrame(6);
        SetupHeartrateGraph();
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


    // Method that is called when the value of the timeline is changed
    public void OnTimelineValueChanged(float value)
    {
        if (!isDragged) { return; }

        int closestHighlight = shownHighlights.OrderBy(x => Mathf.Abs((long)x - value)).First();

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
        shownHighlights.Add(frame);
        if (frame < 6) { return; }
        Vector2 markerPosition = GetMarkerPosition(frame);
        GameObject mark = Instantiate(marker, transform, false);
        mark.GetComponent<RectTransform>().localPosition = markerPosition;
        mark.GetComponent<Image>().color = color;
    }

    public void SetupHeartrateGraph()
    {
        LineRenderer lineRenderer = transform.Find("Slider").Find("Background").GetComponent<LineRenderer>();
        lineRenderer.positionCount = replayManager.GetHRLog().Count+1;
        int point = 0;
        Vector3 position = new Vector3(-400, -20, -1);
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

}