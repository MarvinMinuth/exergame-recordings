using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskWall : MonoBehaviour
{
    private ReplayManager replayManager;
    [SerializeField] private TMP_Text textComponent;
    private void Start()
    {
        replayManager = ReplayManager.Instance;

        replayManager.OnReplayLoaded += replayManager_OnReplayLoaded;
        replayManager.OnReplayUnloaded += replayManager_OnReplayUnloaded;
        DeleteText();
    }

    private void replayManager_OnReplayUnloaded(object sender, System.EventArgs e)
    {
        DeleteText();
    }

    private void replayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        SetText(e.loadingStatueSO.messageText);
    }

    private void SetText(string text)
    {
        textComponent.text = text;
    }

    private void DeleteText()
    {
        textComponent.text = string.Empty;
    }

    public void AddText(string text)
    {
        textComponent.text += "\n" + text;
    }
}
