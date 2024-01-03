using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishReplayLogger : MonoBehaviour
{
    private FinishReplayStudyLog log;
    private void Start()
    {
        log = new FinishReplayStudyLog();

        ReplayManager.Instance.OnReplayLoaded += ReplayManager_OnReplayLoaded;
        FighterLoader.Instance.OnReplayFinished += FighterLoader_OnReplayFinished;
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        log.savefile = e.loadingStatueSO.saveFile;
    }

    private void FighterLoader_OnReplayFinished(object sender, System.EventArgs e)
    {
        log.replayFrame = ReplayManager.Instance.GetFrame();

        Logger.Instance.Log(log);
    }
}
