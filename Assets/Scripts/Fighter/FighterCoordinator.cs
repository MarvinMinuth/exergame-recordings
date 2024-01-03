using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FighterCoordinator : MonoBehaviour
{
    public static FighterCoordinator Instance {  get; private set; }

    public event EventHandler OnHeadHidden;
    public event EventHandler OnHeadShown;
    public event EventHandler OnLeftHandHidden;
    public event EventHandler OnLeftHandShown;
    public event EventHandler OnRightHandHidden;
    public event EventHandler OnRightHandShown;
    public event EventHandler OnBodyHidden;
    public event EventHandler OnBodyShown;

    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material hmdMaterial;

    [SerializeField] private FighterVisuals fighterVisuals;
    [SerializeField] private Trajectories headTrajectories, leftHandTrajectories, rightHandTrajectories;
    [SerializeField] private Transform headTransform, leftHandTransform, rightHandTransform;


    private bool headShown, leftHandShown, rightHandShown, bodyShown;
    private LoadingStatueSO loadingStatueSO;
    private bool fighterMovementEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one FighterCoordinator found");
        }

        headShown = true;
        leftHandShown = true;
        rightHandShown = true;
        bodyShown = true;
    }
    private void Start()
    {
        ReplayManager.Instance.OnReplayLoaded += ReplayManager_OnReplayLoaded;
        ReplayManager.Instance.OnReplayUnloaded += ReplayManager_OnReplayUnloaded;
        ReplayManager.Instance.OnFrameLoaded += ReplayManager_OnFrameLoaded;

        FighterLoader.Instance.OnFighterInPosition += FighterLoader_OnFighterInPosition;

        fighterVisuals.Hide();
    }

    private void FighterLoader_OnFighterInPosition(object sender, EventArgs e)
    {
        ShowHead();
        ShowLeftHand();
        ShowRightHand();
        ShowBody();
        fighterVisuals.Show();
        fighterVisuals.ChangeMaterial(loadingStatueSO.material, false);
    }

    public void SetHeadTransparent()
    {
        headShown = false;
        fighterVisuals.ChangeHeadMaterial(loadingStatueSO.transparentMaterial, true);

        OnHeadHidden?.Invoke(this, EventArgs.Empty);
    }

    public void ShowHead()
    {
        headShown = true;
        fighterVisuals.ChangeHeadMaterial(loadingStatueSO.material, false);
        fighterVisuals.ChangeHMDMaterial(loadingStatueSO.hmdMaterial);

        OnHeadShown?.Invoke(this, EventArgs.Empty);
    }

    public void SetLeftHandTransparent()
    {
        leftHandShown = false;
        fighterVisuals.ChangeLeftHandMaterial(loadingStatueSO.transparentMaterial);

        OnLeftHandHidden?.Invoke(this, EventArgs.Empty);
    }

    public void ShowLeftHand()
    {
        leftHandShown = true;
        fighterVisuals.ChangeLeftHandMaterial(loadingStatueSO.material);

        OnLeftHandShown?.Invoke(this, EventArgs.Empty);
    }

    public void SetRightHandTransparent()
    {
        rightHandShown = false;
        fighterVisuals.ChangeRightHandMaterial(loadingStatueSO.transparentMaterial);

        OnRightHandHidden?.Invoke(this, EventArgs.Empty);
    }

    public void ShowRightHand()
    {
        rightHandShown = true;
        fighterVisuals.ChangeRightHandMaterial(loadingStatueSO.material);

        OnRightHandShown?.Invoke(this, EventArgs.Empty);
    }

    public void SetBodyTransparent()
    {
        bodyShown = false;
        fighterVisuals.ChangeBodyMaterial(loadingStatueSO.transparentMaterial);

        OnBodyHidden?.Invoke(this, EventArgs.Empty);
    }

    public void ShowBody()
    {
        bodyShown = true;
        fighterVisuals.ChangeBodyMaterial(loadingStatueSO.material);

        OnBodyShown?.Invoke(this, EventArgs.Empty);
    }

    private void ReplayManager_OnFrameLoaded(object sender, ReplayManager.OnFrameLoadedEventArgs e)
    {
        if (!fighterMovementEnabled) return;
        //transform.position = new Vector3(e.headTransformLog.Position.x, 0, e.headTransformLog.Position.z);

        headTransform.position = e.headTransformLog.Position;
        headTransform.rotation = Quaternion.Euler(e.headTransformLog.Rotation);

        leftHandTransform.position = e.leftHandTransformLog.Position;
        leftHandTransform.rotation = Quaternion.Euler(e.leftHandTransformLog.Rotation);

        rightHandTransform.position = e.rightHandTransformLog.Position;
        rightHandTransform.rotation = Quaternion.Euler(e.rightHandTransformLog.Rotation);

        fighterVisuals.ResetVisuals();
    }

    private void ReplayManager_OnReplayUnloaded(object sender, System.EventArgs e)
    {
        if (headTrajectories != null) { headTrajectories.DestroyTrajectories(); }
        if (leftHandTrajectories != null) { leftHandTrajectories.DestroyTrajectories(); }
        if (rightHandTrajectories != null) { rightHandTrajectories.DestroyTrajectories(); }

        ChangeToIdleMaterial();
        fighterVisuals.Hide();
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        
        loadingStatueSO = e.loadingStatueSO;
    }

    public void ChangeToIdleMaterial()
    {
        fighterVisuals.ChangeMaterial(idleMaterial, false);
        fighterVisuals.ChangeHMDMaterial(hmdMaterial);
    }

    public bool IsFighterMovementEnabled()
    {
        return fighterMovementEnabled;
    }

    public void SetFighterMovement(bool enable)
    {
        fighterMovementEnabled = enable;
    }

    public bool IsHeadShown()
    {
        return headShown;
    }
    public bool IsLeftHandShown()
    {
        return leftHandShown;
    }
    public bool IsRightHandShown()
    {
        return rightHandShown;
    }
    public bool IsBodyShown()
    {
        return bodyShown;
    }
}
