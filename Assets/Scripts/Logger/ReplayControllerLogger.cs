using UnityEngine;

public class ReplayControllerLogger : MonoBehaviour
{
    private ReplayControllerStudyLog log;
    private ReplayController replayController;

    private void Start()
    {
        log = new ReplayControllerStudyLog();

        replayController = ReplayController.Instance;

        //replayController.OnReplayControllerLoaded += ReplayController_OnChange;
        replayController.OnPlay += ReplayController_OnChange;
        replayController.OnPause += ReplayController_OnChange;
        replayController.OnDirectionChanged += ReplayController_OnChange;
        replayController.OnSpeedChanged += ReplayController_OnChange;
        replayController.OnRepeat += ReplayController_OnChange;
    }

    private void ReplayController_OnChange(object sender, System.EventArgs e)
    {
        CreateLog();
    }

    private void CreateLog()
    {
        log.loadedFrame = replayController.GetFrame();
        log.isRunning = replayController.IsRunning();
        log.isLooping = replayController.IsLooping(); 
        log.playSpeed = replayController.GetPlaySpeed();
        log.playDirection = replayController.GetPlayDirection();

        Logger.Instance.LogReplayControllerSettings(log);
    }
}
