using UnityEngine;

public class HeartrateOptionsLogger : MonoBehaviour
{
    private HeartrateCoordinator heartrateCoordinator;
    private HeartrateOptionsStudyLog hrOptionsLog;

    private void Awake()
    {
        hrOptionsLog = new HeartrateOptionsStudyLog();
    }
    private void Start()
    {
        heartrateCoordinator = HeartrateCoordinator.Instance;

        heartrateCoordinator.OnAudioFeedbackChanged += HeartrateCoordinator_OnOptionsChanged;
        heartrateCoordinator.OnHapticFeedbackChanged += HeartrateCoordinator_OnOptionsChanged;
        heartrateCoordinator.OnVisualFeedbackChanged += HeartrateCoordinator_OnOptionsChanged;
        heartrateCoordinator.OnPositionChanged += HeartrateCoordinator_OnOptionsChanged;

        LogHeartrateOptions();
    }

    private void HeartrateCoordinator_OnOptionsChanged(object sender, System.EventArgs e)
    {
        LogHeartrateOptions();
    }

    private void LogHeartrateOptions()
    {
        hrOptionsLog.audioFeedbackActivated = heartrateCoordinator.IsAudioFeedbackActivated();
        hrOptionsLog.hapticFeedbackActivated = heartrateCoordinator.IsHapticFeedbackActivated();
        hrOptionsLog.visualFeedbackActivated = heartrateCoordinator.IsVisualFeedbackActivated();

        hrOptionsLog.position = heartrateCoordinator.GetCurrentPosition();

        Logger.Instance.Log(hrOptionsLog);
    }
}
