using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowFrame : MonoBehaviour
{
    [SerializeField] private TMP_Text textfield;

    private void Start()
    {
        ReplayManager.Instance.OnFrameLoaded += ReplayManager_OnFrameLoaded;
    }

    private void ReplayManager_OnFrameLoaded(object sender, ReplayManager.OnFrameLoadedEventArgs e)
    {
        textfield.text = "Frame: " + e.headTransformLog.frame.ToString();
    }
}
