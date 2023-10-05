using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterLoader : MonoBehaviour
{
    [Header("General")]
    public ReplayController replayController;
    public Material defaultMaterial;
    public Material hmdMaterial;
    public TaskWall taskWall;

    [Header("Tutorial Statue")]
    public FighterLoadingStatue tutorialLoadingStatue;
    public Material tutorialMaterial;
    public bool tutorialEnabled;

    [Header("Task One Statue")]
    public FighterLoadingStatue taskOneLoadingStatue;
    public Material taskOneMaterial;
    public bool taskOneEnabled;

    [Header("Task Two Statue")]
    public FighterLoadingStatue taskTwoLoadingStatue;
    public Material taskTwoMaterial;
    public bool taskTwoEnabled;

    [Header("Task Three Statue")]
    public FighterLoadingStatue taskThreeLoadingStatue;
    public Material taskThreeMaterial;
    public bool taskThreeEnabled;


    private FighterLoadingStatue loadedFighter;
    private FighterLoadingStatue shownMessage;

    private bool loadingAllowed = true;

    void Start()
    {
        if(replayController == null)
        {
            replayController = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayController>();
        }
        if(tutorialEnabled) tutorialLoadingStatue.SetupStatue(Savefile.Tutorial, tutorialMaterial, hmdMaterial);
        if(taskOneEnabled) taskOneLoadingStatue.SetupStatue(Savefile.TaskOne, taskOneMaterial, hmdMaterial);
        if(taskTwoEnabled) taskTwoLoadingStatue.SetupStatue(Savefile.TaskTwo, taskTwoMaterial, hmdMaterial);
        if(taskThreeEnabled) taskThreeLoadingStatue.SetupStatue(Savefile.TaskThree, taskThreeMaterial, hmdMaterial);
    }

    public void LoadReplay(Savefile saveFile)
    {
        if (!loadingAllowed) return;
        if (replayController.ManagerIsLoading()) return;
        if (loadedFighter != null && !loadedFighter.IsFinished())
        {
            loadedFighter.ShowStatue();
        }
        taskWall.DeleteText();
        Material fighterMaterial = defaultMaterial;

        switch (saveFile)
        {
            case Savefile.Tutorial:
                fighterMaterial = tutorialMaterial;
                loadedFighter = tutorialLoadingStatue;
                break;
            case Savefile.TaskOne:
                fighterMaterial = taskOneMaterial;
                loadedFighter = taskOneLoadingStatue;
                break;
            case Savefile.TaskTwo:
                fighterMaterial = taskTwoMaterial;
                loadedFighter = taskTwoLoadingStatue;
                break;
            case Savefile.TaskThree:
                fighterMaterial = taskThreeMaterial;
                loadedFighter = taskThreeLoadingStatue;
                break;
        }

        loadedFighter.HideStatue();
        taskWall.SetText(loadedFighter.GetMessage());

        replayController.Load(saveFile, fighterMaterial);
    }

    public void FinishLoadedReplay()
    {
        if(loadedFighter == null) return;
        loadedFighter.Finish();
        taskWall.DeleteText();
        loadedFighter = null;
        replayController.Unload();
    }

    public void ShowMessage(FighterLoadingStatue fighterLoadingStatue)
    {
        if(shownMessage != null)
        {
            shownMessage.HideMessage();
        }
        shownMessage = fighterLoadingStatue;
    }

    public void HideMessage()
    {
        shownMessage = null;
    }

    public void PreventLoading()
    {
        if (tutorialLoadingStatue != null) { tutorialLoadingStatue.InteractionsAllowed(false);  tutorialLoadingStatue.HideMessage(); }
        if (taskOneLoadingStatue != null) { taskOneLoadingStatue.InteractionsAllowed(false); taskOneLoadingStatue.HideMessage(); }
        if (taskTwoLoadingStatue != null) { taskTwoLoadingStatue.InteractionsAllowed(false); taskTwoLoadingStatue.HideMessage(); }
        if (taskThreeLoadingStatue != null) { taskThreeLoadingStatue.InteractionsAllowed(false); taskThreeLoadingStatue.HideMessage(); }
    }

    public void AllowLoading()
    {
        if (tutorialLoadingStatue != null) { tutorialLoadingStatue.InteractionsAllowed(true); }
        if (taskOneLoadingStatue != null) { taskOneLoadingStatue.InteractionsAllowed(true); }
        if (taskTwoLoadingStatue != null) { taskTwoLoadingStatue.InteractionsAllowed(true); }
        if (taskThreeLoadingStatue != null) { taskThreeLoadingStatue.InteractionsAllowed(true); }
    }
}
