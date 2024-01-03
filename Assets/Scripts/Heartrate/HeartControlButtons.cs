using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartControlButtons : MonoBehaviour
{
    private HeartrateCoordinator heartrateCoordinator;
    [SerializeField] private Button audioFeedbackButton;
    [SerializeField] private Button visualFeedbackButton;
    [SerializeField] private Button hapticFeedbackButton;
    [SerializeField] private TMP_Dropdown heartPositionDropdown;

    private void OnEnable()
    {
        if(heartrateCoordinator == null)
        {
            heartrateCoordinator = HeartrateCoordinator.Instance;
        }
    }
    private void Awake()
    {
        audioFeedbackButton.onClick.AddListener(OnAudioFeedbackButtonPressed);
        visualFeedbackButton.onClick.AddListener(OnVisualFeedbackButtonPressed);
        hapticFeedbackButton.onClick.AddListener(OnHapticFeedbackButtonPressed);
        heartPositionDropdown.onValueChanged.AddListener(OnHeartPositionChanged);
    }
    void Start()
    {
        heartrateCoordinator = HeartrateCoordinator.Instance;

        heartrateCoordinator.OnAudioFeedbackChanged += HeartrateCoordinator_OnAudioFeedbackChanged;
        heartrateCoordinator.OnHapticFeedbackChanged += HeartrateCoordinator_OnHapticFeedbackChanged;
        heartrateCoordinator.OnVisualFeedbackChanged += HeartrateCoordinator_OnVisualFeedbackChanged;

        CheckAllButtonStatus();
        CheckPositionStatus();
    }

    private void HeartrateCoordinator_OnVisualFeedbackChanged(object sender, System.EventArgs e)
    {
        CheckButtonStatus(visualFeedbackButton, heartrateCoordinator.IsVisualFeedbackActivated());
    }

    private void HeartrateCoordinator_OnHapticFeedbackChanged(object sender, System.EventArgs e)
    {
        CheckButtonStatus(hapticFeedbackButton, heartrateCoordinator.IsHapticFeedbackActivated());
    }

    private void HeartrateCoordinator_OnAudioFeedbackChanged(object sender, System.EventArgs e)
    {
        CheckButtonStatus(audioFeedbackButton, heartrateCoordinator.IsAudioFeedbackActivated());
    }

    public void CheckPositionStatus()
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
        heartrateCoordinator.ChangeAudioFeedback();
    }
    public void OnVisualFeedbackButtonPressed()
    {
        heartrateCoordinator.ChangeVisualFeedback();
    }
    public void OnHapticFeedbackButtonPressed()
    {
        heartrateCoordinator.ChangeHapticFeedback();
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

    public void CheckAllButtonStatus()
    {
        CheckButtonStatus(visualFeedbackButton, heartrateCoordinator.IsVisualFeedbackActivated());
        CheckButtonStatus(hapticFeedbackButton, heartrateCoordinator.IsHapticFeedbackActivated());
        CheckButtonStatus(audioFeedbackButton, heartrateCoordinator.IsAudioFeedbackActivated());
    }
}
