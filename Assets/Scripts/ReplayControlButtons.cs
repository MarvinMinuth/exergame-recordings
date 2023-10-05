using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReplayControlButtons : MonoBehaviour
{
    private Button pauseButton;
    private Button playButton;
    private Button backwardsButton;
    private Button halfSpeedButton, normalSpeedButton, doubleSpeedButton;
    private Button stopButton;
    private Button repeatButton;
    //private Slider speedSlider;
    // private TMP_Text speedInfo;

    public ReplayController controller;

    // Start is called before the first frame update
    void Start()
    {
        if (controller == null)
        {
            controller = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayController>();
        }

        pauseButton = transform.Find("Pause Button").GetComponent<Button>();
        playButton = transform.Find("Play Button").GetComponent<Button>();
        backwardsButton = transform.Find("Backwards Button").GetComponent<Button>();
        stopButton = transform.Find("Stop Button").GetComponent<Button>();
        //speedSlider = transform.Find("Replay Speed Slider").GetComponent<Slider>();
        halfSpeedButton = transform.Find("Half Speed Button").GetComponent<Button>();
        normalSpeedButton = transform.Find("Normal Speed Button").GetComponent<Button>();
        doubleSpeedButton = transform.Find("Double Speed Button").GetComponent<Button>();
        //speedInfo = speedButton.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        repeatButton = transform.Find("Repeat Button").GetComponent<Button>();

        //speedInfo.text = "1x";
        //speedSlider.onValueChanged.AddListener(OnSpeedValueChanged);

        pauseButton.onClick.AddListener(OnPauseButtonClick);
        playButton.onClick.AddListener(OnPlayButtonClick);
        stopButton.onClick.AddListener(OnStopButtonClick);
        backwardsButton.onClick.AddListener(OnBackwardsButtonClick);
        halfSpeedButton.onClick.AddListener(OnHalfSpeedButtonClick);
        normalSpeedButton.onClick.AddListener(OnNormalSpeedButtonClick);
        doubleSpeedButton.onClick.AddListener(OnDoubleSpeedButtonClick);
        repeatButton.onClick.AddListener(OnRepeatButtonClick);

        //SetButtonActive(speedButton);
    }

    public void SetupButtons()
    {
    }

    void Update()
    {
        if (!controller.IsReplayReady()) { return; }

        CheckButtonStatus(playButton, controller.IsRunning());
        CheckButtonStatus(pauseButton, !controller.IsRunning());
        CheckButtonStatus(backwardsButton, controller.GetPlayDirection() == Direction.Backwards);
        CheckButtonStatus(repeatButton, controller.IsLooping());
        CheckButtonStatus(halfSpeedButton, controller.GetPlaySpeed() == 0.5f);
        CheckButtonStatus(normalSpeedButton, controller.GetPlaySpeed() == 1f);
        CheckButtonStatus(doubleSpeedButton, controller.GetPlaySpeed() == 2f);
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

    public void SetButtonActive(Button button)
    {
        button.GetComponent<Image>().color = Color.grey;
    }

    public void SetButtonInactive(Button button)
    {
        button.GetComponent<Image>().color = Color.white;
    }

    public void OnHalfSpeedButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        controller.ChangeSpeed(Speed.Half);
    }
    public void OnNormalSpeedButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        controller.ChangeSpeed(Speed.Normal);
    }
    public void OnDoubleSpeedButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        controller.ChangeSpeed(Speed.Double);
    }
    public void OnPauseButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        if (controller.IsRunning())
        {
            controller.Pause();
            CheckButtonStatus(playButton, controller.IsRunning());
            CheckButtonStatus(pauseButton, !controller.IsRunning());
        }
    }

    public void OnPlayButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        if (!controller.IsRunning())
        {
            controller.Play();
            CheckButtonStatus(playButton, controller.IsRunning() );
            CheckButtonStatus(pauseButton, !controller.IsRunning());
        }

    }

    public void OnStopButtonClick()
    {
        if (!controller.IsControllable())
        {
            return;
        }
        /*
        if (shownMarkers.ContainsKey(activatedButton))
        {
            SetButtonInactive(shownMarkers[activatedButton].GetComponent<Button>());
            SetButtonInactive(shownMarkers[activatedButton].transform.Find("Top Button").GetComponent<Button>());
        }
        minFrame = (int)timeline.minValue;
        maxFrame = (int)timeline.maxValue;
        */
        controller.Stop();
        CheckButtonStatus(playButton, controller.IsRunning());
        CheckButtonStatus(pauseButton, !controller.IsRunning());
    }

    public void OnFastPlayButtonPressed()
    {
        if (controller.GetPlaySpeed() == 1)
        {
            controller.SetPlaySpeed(2);
        }
        else
        {
            controller.SetPlaySpeed(1);
        }
    }

    public void OnBackwardsButtonClick()
    {
        controller.ChangeDirection();
        CheckButtonStatus(backwardsButton, controller.GetPlayDirection() == Direction.Backwards);
    }

    public void OnRepeatButtonClick()
    {
        controller.ChangeLooping();
        CheckButtonStatus(repeatButton, controller.IsLooping());
    }

    /*
    public void OnSpeedValueChanged(float speed)
    {
        if (!controller.IsReplayReady()) { return; }
        speedInfo.text = (speed / 2).ToString();
        controller.SetPlaySpeed(speed / 2);
    }
    */

    public void ResetButtons()
    {
        SetButtonInactive(playButton);
        SetButtonInactive(pauseButton);
        SetButtonInactive(stopButton);
        SetButtonInactive(stopButton);
        SetButtonInactive(halfSpeedButton);
        SetButtonInactive(repeatButton);
        SetButtonInactive(normalSpeedButton);
        SetButtonInactive(doubleSpeedButton);
        //speedInfo.text = "-";
    }
}
