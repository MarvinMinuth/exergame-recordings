using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartControlButtons : MonoBehaviour
{
    private HeartrateCoordinator heartrateCoordinator;
    private Button audioFeedbackButton;
    private Button visualFeedbackButton;
    private Button hapticFeedbackButton;
    private TMP_Dropdown heartPositionDropdown;
    [SerializeField] private HeartMenu menu;

    void Start()
    {
        heartrateCoordinator = GameObject.FindGameObjectWithTag("Heartrate").GetComponent<HeartrateCoordinator>();
        audioFeedbackButton = transform.Find("Audio Feedback Button").GetComponent<Button>();
        visualFeedbackButton = transform.Find("Visual Feedback Button").GetComponent<Button>();
        hapticFeedbackButton = transform.Find("Haptic Feedback Button").GetComponent<Button>();
        heartPositionDropdown = transform.Find("Heart Position Dropdown").GetComponent<TMP_Dropdown>();

        audioFeedbackButton.onClick.AddListener(OnAudioFeedbackButtonPressed);
        visualFeedbackButton.onClick.AddListener(OnVisualFeedbackButtonPressed);
        hapticFeedbackButton.onClick.AddListener(OnHapticFeedbackButtonPressed);
        heartPositionDropdown.onValueChanged.AddListener(OnHeartPositionChanged);
    }


    void Update()
    {
        CheckButtonStatus(hapticFeedbackButton, heartrateCoordinator.useHapticFeedback);
        CheckButtonStatus(visualFeedbackButton, heartrateCoordinator.useVisualFeedback);
        CheckButtonStatus(audioFeedbackButton, heartrateCoordinator.useAudioFeedback);
        CheckPositionStatus();
    }

    private void CheckPositionStatus()
    {
        switch (heartrateCoordinator.position)
        {
            case Position.Dummy:
                heartPositionDropdown.value = 0; break;
            case Position.FighterHead:
                heartPositionDropdown.value = 1; break;
            case Position.FighterBody:
                heartPositionDropdown.value = 2; break;
        }
    }

    public void OnAudioFeedbackButtonPressed()
    {
        if (heartrateCoordinator.useAudioFeedback) { heartrateCoordinator.useAudioFeedback = false; }
        else { heartrateCoordinator.useAudioFeedback = true; }
    }
    public void OnVisualFeedbackButtonPressed()
    {
        if (heartrateCoordinator.useVisualFeedback) { heartrateCoordinator.useVisualFeedback = false; }
        else { heartrateCoordinator.useVisualFeedback = true; }
    }
    public void OnHapticFeedbackButtonPressed()
    {
        if (heartrateCoordinator.useHapticFeedback) { heartrateCoordinator.useHapticFeedback = false; }
        else { heartrateCoordinator.useHapticFeedback = true; }
    }
    public void OnHeartPositionChanged(int value)
    {   
        switch (value)
        {
            case 0:
                heartrateCoordinator.position = Position.Dummy; break;
            case 1:
                heartrateCoordinator.position = Position.FighterHead; break;
            case 2:
                heartrateCoordinator.position = Position.FighterBody; break;
        }
        menu.Deactivate();
    }
    public void SetButtonActive(Button button)
    {
        button.GetComponent<Image>().color = Color.grey;
    }
    public void SetButtonInactive(Button button)
    {
        button.GetComponent<Image>().color = Color.white;
    }
    public void CheckButtonStatus(Button button, bool active)
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
}
