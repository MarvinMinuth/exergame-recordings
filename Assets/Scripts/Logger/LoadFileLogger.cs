using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFileLogger : MonoBehaviour
{
    private LoadFileStudyLog log;
    private void Start()
    {
        log = new LoadFileStudyLog();

        ReplayManager.Instance.OnReplayLoaded += ReplayManager_OnReplayLoaded;
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        log.savefile = e.loadingStatueSO.saveFile;
        Logger.Instance.LogLoadFile(log);
    }
}
