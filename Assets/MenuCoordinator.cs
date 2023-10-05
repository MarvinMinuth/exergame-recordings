using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuCoordinator : MonoBehaviour
{
    [SerializeField]
    public Material transparentFighterMaterial;

    public Timeline timeline;
    public FighterButtons fighterButtons;
    public FighterMiniStatue fighterMiniStatue;
    public HeartrateGraph heartrateGraph;
    public ReplayControlButtons replayControlButtons;
    public ReplayManager manager;
    public ReplayController controller;

    public bool fighterHeadShown, fighterBodyShown, fighterLeftHandShown, fighterRightHandShown;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Stop()
    {
        timeline.DeactivateActiveMark();
    }
    
    public void SetupMenu()
    {
        timeline.SetupTimeline();
        heartrateGraph.SetupHeartrateGraph(manager.GetHRLog());
        //fighterButtons.SetupFighterButtons();
        ShowAllBodyParts();
        fighterMiniStatue.SetupFighterButtons();
    }
    

    public void DestroyMenu()
    {
        replayControlButtons.ResetButtons();
        timeline.DeleteTimeline();
        heartrateGraph.DeleteGraph();
        ShowNoBodyParts();
        //fighterButtons.ResetFighterButtons();
        fighterMiniStatue.ResetFighterButtons();
    }

    public ReplayManager GetReplayManager() { return manager; }
    public ReplayController GetReplayController() { return controller; }

    public float GetTimelineMaxValue()
    {
        return timeline.GetMaxValue();
    }

    public float GetTimelineMinValue()
    {
        return timeline.GetMinValue();
    }

    public Slider GetTimelineSlider()
    {
        return timeline.GetSlider();
    }

    public bool IsMenuActive()
    {
        return controller.IsReplayReady();
    }

    private void ShowNoBodyParts()
    {
        fighterHeadShown = false;
        fighterBodyShown = false;
        fighterLeftHandShown = false;
        fighterRightHandShown = false;
    }
    private void ShowAllBodyParts()
    {
        fighterHeadShown = true;
        fighterBodyShown = true;
        fighterLeftHandShown = true;
        fighterRightHandShown = true;
    }

    public void StopTimelineUsage()
    {
        timeline.GetSlider().interactable = false;
    }

    public void AllowTimelineUsage()
    {
        timeline.GetSlider().interactable = true;
    }
}
