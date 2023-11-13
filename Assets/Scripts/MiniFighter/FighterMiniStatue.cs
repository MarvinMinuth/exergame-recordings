using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FighterMiniStatue : MonoBehaviour
{
    
    private FighterCoordinator fighterCoordinator;
    [SerializeField] private MiniFighterButton leftHandMiniFighterButton, rightHandMiniFighterButton, bodyMiniFighterButton;
    [SerializeField] private MiniFighterButtonHead headMiniFighterButton;
    [SerializeField] private Transform bottomPieceTransform;

    [SerializeField] private Material defaultMaterial;

    private ReplayManager replayManager;
    private LoadingStatueSO loadingStatueSO;
    public bool miniIsActivated;
    private bool headShown, leftHandShown, rightHandShown, bodyShown;

    private void Awake()
    {
        miniIsActivated = false;
        headShown = true;
        leftHandShown = true;
        rightHandShown = true;
        bodyShown = true;
    }
    void Start()
    {
        replayManager = ReplayManager.Instance;
        replayManager.OnReplayLoaded += ReplayManager_OnReplayLoaded;
        replayManager.OnReplayUnloaded += ReplayManager_OnReplayUnloaded;

        fighterCoordinator = FighterCoordinator.Instance;

        Hide();
    }

    private void ReplayManager_OnReplayUnloaded(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        loadingStatueSO = e.loadingStatueSO;
        SetupFighterButtons();
    }

    public void Hide()
    {
        miniIsActivated = false;
        headMiniFighterButton.Hide();
        leftHandMiniFighterButton.Hide();
        rightHandMiniFighterButton.Hide();
        bodyMiniFighterButton.Hide();
        bottomPieceTransform.gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!replayManager.FileIsLoaded()) return;

        miniIsActivated = true;
        bottomPieceTransform.gameObject.SetActive(true);
        headMiniFighterButton.Show();
        leftHandMiniFighterButton.Show();
        rightHandMiniFighterButton.Show();
        bodyMiniFighterButton.Show();
    }

    public void SetupFighterButtons()
    {
        SetHeadButtonActive();
        SetBodyButtonActive();
        SetLeftHandButtonActive();
        SetRightHandButtonActive();
    }

    public void ResetFighterButtons()
    {
        SetHeadButtonInactive();
        SetBodyButtonInactive();
        SetLeftHandButtonInactive();
        SetRightHandButtonInactive();
    }

    public void OnHeadButtonClick()
    {
        headShown = !headShown;
        if (!headShown)
        {
            SetHeadButtonInactive();
            fighterCoordinator.SetHeadTransparent();
        }
        else
        {
            SetHeadButtonActive();
            fighterCoordinator.ShowHead();
        }
    }
    public void SetHeadButtonActive()
    {
        headMiniFighterButton.SetMaterial(loadingStatueSO.material);
        headMiniFighterButton.SetHMDMaterial(loadingStatueSO.hmdMaterial);
    }
    public void SetHeadButtonInactive()
    {
        headMiniFighterButton.SetMaterial(loadingStatueSO.transparentMaterial);
        headMiniFighterButton.SetHMDMaterial(loadingStatueSO.transparentMaterial);
    }
    public void OnLeftHandButtonClick()
    {
        leftHandShown = !leftHandShown;
        if (!leftHandShown)
        {
            SetLeftHandButtonInactive();
            fighterCoordinator.SetLeftHandTransparent();
        }
        else
        {
            SetLeftHandButtonActive();
            fighterCoordinator.ShowLeftHand();
        }
    }
    public void SetLeftHandButtonActive()
    {
        leftHandMiniFighterButton.SetMaterial(loadingStatueSO.material);
    }
    public void SetLeftHandButtonInactive()
    {
        leftHandMiniFighterButton.SetMaterial(loadingStatueSO.transparentMaterial);
    }

    public void OnRightHandButtonClick()
    {
        rightHandShown = !rightHandShown;
        if (!rightHandShown)
        {
            SetRightHandButtonInactive();
            fighterCoordinator.SetRightHandTransparent();
        }
        else
        {
            SetRightHandButtonActive();
            fighterCoordinator.ShowRightHand();
        }
    }
    public void SetRightHandButtonActive()
    {
        rightHandMiniFighterButton.SetMaterial(loadingStatueSO.material);
    }
    public void SetRightHandButtonInactive()
    {
        rightHandMiniFighterButton.SetMaterial(loadingStatueSO.transparentMaterial);
    }

    public void OnBodyButtonClick()
    {
        bodyShown = !bodyShown;
        if (!bodyShown)
        {
            SetBodyButtonInactive();
            fighterCoordinator.SetBodyTransparent();
        }
        else
        {
            SetBodyButtonActive();
            fighterCoordinator.ShowBody();
        }
    }
    public void SetBodyButtonActive()
    {
        bodyMiniFighterButton.SetMaterial(loadingStatueSO.material);
    }
    public void SetBodyButtonInactive()
    {
        bodyMiniFighterButton.SetMaterial(loadingStatueSO.transparentMaterial);
    }
}
