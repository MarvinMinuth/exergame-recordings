using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Timeline : MonoBehaviour
{
    // Slider
    public float threshold = 60f;
    
    private Slider timeline;

    //Replay

    private ReplayController controller;
    private ReplayManager manager;
    public MenuCoordinator menuCoordinator;

    // Markers
    [Header("Marker")]
    public GameObject marker;
    public Color markerColor = Color.white;
    public float highlightThreshold = 120f;
    private List<int> shownHighlights = new List<int>();
    private Dictionary<int, GameObject> shownMarkers = new Dictionary<int, GameObject>();
    //public bool showSuccsessfulArmCollisionHighlights;
    //public bool showUnsuccsessfulArmCollisionHighlights;
    //public bool showSuccsessfulFightCollisionHighlights;
    public bool showUnsuccsessfulFightCollisionHighlights;
    //private Button sacButton, sfcButton, uacButton, ufcButton;
    private int minFrame, maxFrame;
    private int activatedButton;
    public int windowSize = 180;
    public bool isLooping = false;
    


    private bool isDragged = false;
    private bool wasRunning;

    private int frame;
    void Start()
    {
        if(menuCoordinator == null)
        {
            menuCoordinator = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>();
        }

        timeline = transform.Find("Slider").GetComponent<Slider>();

        timeline.maxValue = 0;
        controller = menuCoordinator.GetReplayController();
        manager = menuCoordinator.GetReplayManager();

        //sacButton = transform.Find("Marker Options").Find("SAC").GetComponent<Button>();
        //sfcButton = transform.Find("Marker Options").Find("SFC").GetComponent<Button>();
        //uacButton = transform.Find("Marker Options").Find("UAC").GetComponent<Button>();
        //ufcButton = transform.Find("Marker Options").Find("UFC").GetComponent<Button>();

        //sacButton.onClick.AddListener(OnSACButtonClick);
        //sfcButton.onClick.AddListener(OnSFCButtonClick);
        //uacButton.onClick.AddListener(OnUACButtonClick);
        //ufcButton.onClick.AddListener(OnUFCButtonClick);
    }

    public void SetupTimeline()
    {
        timeline.maxValue = (controller.GetReplayLength()) - 2;

        frame = 6;
        controller.LoadFrame(6);

        //SwitchSuccsessfulArmCollisionHighlights();
        //SwitchUnsuccsessfulArmCollisionHighlights();
        //SwitchSuccsessfulFightCollisionHighlights();
        SwitchUnsuccsessfulFightCollisionHighlights();

        maxFrame = (int)timeline.maxValue;
        minFrame = (int)timeline.minValue; 
    }

    public void DeleteTimeline()
    {
        timeline.maxValue = 0;
        RemoveAllMarkers();
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

    /*
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
            CreateArmCollisionHighlights(manager.GetSuccsessfulArmCollisionDic());
            SetButtonActive(sacButton);
        }
        else
        {
            RemoveArmCollisionHighlights(manager.GetSuccsessfulArmCollisionDic() );
            SetButtonInactive(sacButton);
        }
        
    }
    public void SwitchUnsuccsessfulArmCollisionHighlights() 
    {
        if (showUnsuccsessfulArmCollisionHighlights)
        {
            CreateArmCollisionHighlights(manager.GetUnsuccsessfulArmCollisionDic());
            SetButtonActive(uacButton);
        }
        else
        {
            RemoveArmCollisionHighlights(manager.GetUnsuccsessfulArmCollisionDic());
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
    */

    public void CreateFightCollisionHighlights(Dictionary<int, FightCollisionLog[]> fightCollisionDic)
    {
        foreach (KeyValuePair<int, FightCollisionLog[]> log in fightCollisionDic)
        {
            SetMark(log.Key, markerColor);
        }
    }
    /*
    public void SwitchSuccsessfulFightCollisionHighlights()
    {
        if (showSuccsessfulFightCollisionHighlights)
        {
            CreateFightCollisionHighlights(manager.GetSuccsessfulFightCollisionDic());
            SetButtonActive(sfcButton);
        }
        else
        {
            RemoveFightCollisionHighlights(manager.GetSuccsessfulFightCollisionDic());
            SetButtonInactive(sfcButton);
        }

    }
    */
    public void SwitchUnsuccsessfulFightCollisionHighlights()
    {
        if (showUnsuccsessfulFightCollisionHighlights)
        {
            CreateFightCollisionHighlights(manager.GetUnsuccsessfulFightCollisionDic());
            //SetButtonActive(ufcButton);
        }
        else
        {
            RemoveFightCollisionHighlights(manager.GetUnsuccsessfulFightCollisionDic());
            //SetButtonInactive(ufcButton);
        }

    }
    /*
    public void OnSACButtonClick()
    {
        if(!menuCoordinator.IsMenuActive()) { return; }
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
        if (!menuCoordinator.IsMenuActive()) { return; }
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
        if (!menuCoordinator.IsMenuActive()) { return; }
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
        if (!menuCoordinator.IsMenuActive()) { return; }
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
    */

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

    public void StartDrag()
    {
        wasRunning = controller.IsRunning();
        controller.Pause();
        isDragged = true;
        controller.SetReceivingInput(true);
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
            controller.Play();
        }
        controller.SetReceivingInput(false);
    }


    public void OnTimelineValueChanged(float value)
    {
        if (!isDragged) { return; }
        if (value < minFrame) { value  = minFrame; }
        if (value > maxFrame) { value = maxFrame; }

        int closestHighlight = 6;
        if(shownHighlights.Count != 0) { closestHighlight = shownHighlights.OrderBy(x => Mathf.Abs((long)x - value)).First(); }
        

        // Überprüfe, ob ein Highlight in der Nähe des neuen Werts existiert
        if (Mathf.Abs(closestHighlight - value) <= highlightThreshold)
        {
            // Setze den Replay-Zeitpunkt auf den nächsten Highlight-Wert
            frame = closestHighlight;
            if(frame < 6) { frame = 6; }
            controller.SetFrame(frame);
            controller.LoadFrame(frame);
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
                controller.SetFrame(frame);
                controller.LoadFrame(frame);
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
        if (!controller.IsReplayReady()) { timeline.value = 0; }
        else { timeline.value = controller.GetFrame(); }
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
            mark.GetComponent<Marker>().frame = frame;
            mark.GetComponent<RectTransform>().localPosition = markerPosition;
            mark.GetComponent<Image>().color = color;
            shownMarkers.Add(frame, mark);
        }
    }

    public void OnMarkerButtonClick(int frame)
    {
        if (shownMarkers.ContainsKey(activatedButton))
        { 
            SetButtonInactive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonInactive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
            manager.DestroyTrajectories();
        }
        if (frame != activatedButton)
        {
            activatedButton = frame;
            SetButtonActive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonActive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
            minFrame = frame - (windowSize / 2);
            if (minFrame < timeline.minValue) { minFrame = (int)timeline.minValue; }
            maxFrame = frame + (windowSize / 2);
            if (maxFrame > timeline.maxValue) { maxFrame = (int)timeline.maxValue; }
            manager.CreateTrajectories(minFrame, maxFrame);
            controller.ChangeReplayWindow(minFrame, maxFrame);
            controller.SetFrame(minFrame);
        }
        else
        {
            minFrame = (int) timeline.minValue;
            maxFrame = (int) timeline.maxValue;
            controller.ResetReplayWindow();
            activatedButton = 0;
        }    
    }

    public void DeactivateActiveMark()
    {
        if (shownMarkers.ContainsKey(activatedButton))
        {
            SetButtonInactive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonInactive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
            manager.DestroyTrajectories();
        }
        minFrame = (int)timeline.minValue;
        maxFrame = (int)timeline.maxValue;
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
                minFrame = (int)timeline.minValue;
                maxFrame = (int)timeline.maxValue;
                controller.ResetReplayWindow();
                manager.DestroyTrajectories();
            }
            shownMarkers.Remove(frame);
        }

    }

    public void RemoveAllMarkers()
    {
        foreach (KeyValuePair <int, GameObject> marker in shownMarkers)
        {
            Destroy(marker.Value);
        }
        shownHighlights.Clear();
        shownMarkers.Clear();
    }

    public float GetMaxValue()
    {
        return timeline.maxValue;
    }

    public float GetMinValue()
    {
        return timeline.minValue;
    }

    public Slider GetSlider()
    {
        return timeline;
    }

}