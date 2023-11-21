using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinishTaskUI : MonoBehaviour
{
    [SerializeField] private Button endButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button yesButton;
    [SerializeField] private TMP_Text textField;

    private void Start()
    {
        FighterLoader.Instance.OnFighterInPosition += FighterLoader_OnFighterInPosition;
        ReplayManager.Instance.OnReplayUnloaded += ReplayManager_OnReplayUnloaded;

        endButton.onClick.AddListener(OnEndButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
        yesButton.onClick.AddListener(OnYesButtonClick);

        yesButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        Hide();
    }

    private void ReplayManager_OnReplayUnloaded(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void FighterLoader_OnFighterInPosition(object sender, System.EventArgs e)
    {
        Show();
    }

    private void OnEndButtonClick()
    {
        textField.text = "Aufgabe wirklich beenden?";
        endButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    private void OnCancelButtonClick()
    {
        textField.text = "Aufgabe beenden?";
        endButton.gameObject.SetActive(true);
        yesButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    private void OnYesButtonClick()
    {
        OnCancelButtonClick();
        FighterLoader.Instance.FinishLoadedReplay();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }


}
