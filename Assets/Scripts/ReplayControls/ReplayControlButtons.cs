using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReplayControlButtons : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button backwardsButton;
    [SerializeField] private Button halfSpeedButton, normalSpeedButton, doubleSpeedButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button repeatButton;

    private ReplayController replayController;

    private void OnEnable()
    {
        replayController = ReplayController.Instance;

        pauseButton.onClick.AddListener(OnPauseButtonClick);
        playButton.onClick.AddListener(OnPlayButtonClick);
        stopButton.onClick.AddListener(OnStopButtonClick);
        backwardsButton.onClick.AddListener(OnBackwardsButtonClick);
        halfSpeedButton.onClick.AddListener(OnHalfSpeedButtonClick);
        normalSpeedButton.onClick.AddListener(OnNormalSpeedButtonClick);
        doubleSpeedButton.onClick.AddListener(OnDoubleSpeedButtonClick);
        repeatButton.onClick.AddListener(OnRepeatButtonClick);

        replayController.OnPlay += OnPlayPause;
        replayController.OnPause += OnPlayPause;
        replayController.OnDirectionChanged += OnDirectionChanged;
        replayController.OnSpeedChanged += OnSpeedChanged;
        replayController.OnRepeat += OnRepeating;
        replayController.OnReplayControllerUnload += ReplayController_OnReplayControllerUnload;

        CheckAllButtonStatus();
    }

    private void ReplayController_OnReplayControllerUnload(object sender, EventArgs e)
    {
        SetAllButtonsInactive();
    }

    private void CheckAllButtonStatus()
    {
        CheckButtonStatus(pauseButton, replayController.IsRunning());
        CheckButtonStatus(pauseButton, !replayController.IsRunning());
        CheckButtonStatus(backwardsButton, replayController.GetPlayDirection() == Direction.Backwards);
        CheckButtonStatus(repeatButton, replayController.IsLooping());
        CheckButtonStatus(halfSpeedButton, replayController.GetPlaySpeed() == 0.5f);
        CheckButtonStatus(normalSpeedButton, replayController.GetPlaySpeed() == 1f);
        CheckButtonStatus(doubleSpeedButton, replayController.GetPlaySpeed() == 2f);
    }

    private void OnPlayPause(object sender, EventArgs e)
    {
        CheckButtonStatus(playButton, replayController.IsRunning());
        CheckButtonStatus(pauseButton, !replayController.IsRunning());
    }
    private void OnDirectionChanged(object sender, EventArgs e)
    {
        CheckButtonStatus(backwardsButton, replayController.GetPlayDirection() == Direction.Backwards);
    }
    private void OnRepeating(object sender, EventArgs e)
    {
        CheckButtonStatus(repeatButton, replayController.IsLooping());
    }
    private void OnSpeedChanged(object sender, EventArgs e)
    {
        CheckButtonStatus(halfSpeedButton, replayController.GetPlaySpeed() == 0.5f);
        CheckButtonStatus(normalSpeedButton, replayController.GetPlaySpeed() == 1f);
        CheckButtonStatus(doubleSpeedButton, replayController.GetPlaySpeed() == 2f);
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
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.ChangeSpeed(Speed.Half);
    }
    public void OnNormalSpeedButtonClick()
    {
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.ChangeSpeed(Speed.Normal);
    }
    public void OnDoubleSpeedButtonClick()
    {
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.ChangeSpeed(Speed.Double);
    }
    public void OnPauseButtonClick()
    {
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.Pause();
    }

    public void OnPlayButtonClick()
    {
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.Play();

    }

    public void OnStopButtonClick()
    {
        if (!replayController.IsControllable())
        {
            return;
        }
        replayController.Stop();
    }

    public void OnBackwardsButtonClick()
    {
        replayController.ChangeDirection();
    }

    public void OnRepeatButtonClick()
    {
        replayController.ChangeLooping();
    }

    public void SetAllButtonsInactive()
    {
        SetButtonInactive(playButton);
        SetButtonInactive(pauseButton);
        SetButtonInactive(stopButton);
        SetButtonInactive(stopButton);
        SetButtonInactive(halfSpeedButton);
        SetButtonInactive(repeatButton);
        SetButtonInactive(normalSpeedButton);
        SetButtonInactive(doubleSpeedButton);
    }
}
