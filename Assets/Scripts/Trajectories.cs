using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Trajectories : Timeline
{
    private enum BodyPart
    {
        Head,
        LeftHand,
        RightHand,
    }

    [SerializeField] private BodyPart bodyPart;
    [SerializeField] private GameObject ghostObject;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    private int minFrame, maxFrame;
    private List<TransformLog> transformLogs = new List<TransformLog>();
    [SerializeField] private LineRenderer lineRenderer;

    private ReplayManager replayManager;
    private ReplayController replayController;
    private FighterLoader fighterLoader;

    [SerializeField] private Material opaqueTrajectoryMaterial;
    [SerializeField] private Material transparentTrajectoryMaterial;

    private bool isDragged = false;

    [SerializeField] private XRGrabInteractable grabInteractable;

    protected override void Start()
    {
        base.Start();
        replayManager = ReplayManager.Instance;
        if (replayManager == null)
        {
            Debug.LogError("No ReplayManager found");
        }
        replayManager.OnReplayLoaded += ReplayManager_OnReplayLoaded;

        replayController = ReplayController.Instance;
        if (replayController == null)
        {
            Debug.LogError("No ReplayController found");
        }
        replayController.OnReplayWindowSet += ReplayController_OnReplayWindowSet;
        replayController.OnReplayWindowReset += ReplayController_OnReplayWindowUnset;
        replayController.OnReplayControllerUnload += ReplayController_OnReplayControllerUnload;

        fighterLoader = FighterLoader.Instance;
        if (fighterLoader == null)
        {
            Debug.LogError("No FighterLoader found");
        }

        grabInteractable.enabled = false;

        switch (bodyPart)
        {
            case BodyPart.Head:
                FighterCoordinator.Instance.OnHeadHidden += FighterCoordinator_OnBodyPartHidden;
                FighterCoordinator.Instance.OnHeadShown += FighterCoordinator_OnBodyPartShown;
                break;
            case BodyPart.LeftHand:
                FighterCoordinator.Instance.OnLeftHandHidden += FighterCoordinator_OnBodyPartHidden;
                FighterCoordinator.Instance.OnLeftHandShown += FighterCoordinator_OnBodyPartShown;
                break;
            case BodyPart.RightHand:
                FighterCoordinator.Instance.OnRightHandHidden += FighterCoordinator_OnBodyPartHidden;
                FighterCoordinator.Instance.OnRightHandShown += FighterCoordinator_OnBodyPartShown;
                break;
        }
    }

    protected override void ReplayController_OnReplayControllerUnload(object sender, System.EventArgs e)
    {
        DestroyTrajectories();
    }

    private void FighterCoordinator_OnBodyPartShown(object sender, System.EventArgs e)
    {
        grabInteractable.enabled = true;
        lineRenderer.enabled = true;
    }

    private void FighterCoordinator_OnBodyPartHidden(object sender, System.EventArgs e)
    {
        grabInteractable.enabled = false;
        lineRenderer.enabled = false;
    }

    private void ReplayController_OnReplayWindowUnset(object sender, System.EventArgs e)
    {
        DestroyTrajectories();
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                transformLogs = replayManager.GetHeadTransformLogs();
                break;
            case BodyPart.LeftHand:
                transformLogs = replayManager.GetLeftHandTransformLogs();
                break;
            case BodyPart.RightHand:
                transformLogs = replayManager.GetRightHandTransformLogs();
                break;
        }
    }

    private void ReplayController_OnReplayWindowSet(object sender, ReplayController.OnReplayWindowSetEventArgs e)
    {
        SetFrames(e.minReplayWindowFrame, e.maxReplayWindowFrame);
        CreateTrajectories();
    }

    void Update()
    {
        if (isDragged)
        {
            WhileDragged();
        }
    }

    public void SetFrames(int minFrame, int maxFrame)
    {
        this.minFrame = minFrame;
        this.maxFrame = maxFrame;
    }

    public void SetLogs(List<TransformLog> logs)
    {
        this.transformLogs = logs;
    }

    public void CreateTrajectories()
    {
        lineRenderer.positionCount = maxFrame - minFrame;
        for (int i = minFrame; i < maxFrame; i++)
        {
            lineRenderer.SetPosition(i - minFrame, transformLogs[i].Position);
        }
        lineRenderer.Simplify(0.05f);
        grabInteractable.enabled = true;
    }

    public void DestroyTrajectories()
    {
        grabInteractable.enabled = false;
        lineRenderer.positionCount = 0;
    }

    public override void StartDrag()
    {
        TriggerOnTimelineUsed(this);

        FighterCoordinator.Instance.SetFighterMovement(false);
        replayController.Pause();

        fighterLoader.PreventLoading();
        ghostObject.SetActive(true);
        isDragged = true;
        if(bodyMeshRenderer != null)
        {
            bodyMeshRenderer.enabled = false;
        }
        lineRenderer.material = opaqueTrajectoryMaterial;
    }

    public void WhileDragged()
    {
        int frame = CalculateFrameToLoad();
        SetGhostObjectTransform(frame);
        replayController.SetFrame(frame);
    }

    public override void EndDrag()
    {
        FighterCoordinator.Instance.SetFighterMovement(true);
        isDragged = false;
        int frame = CalculateFrameToLoad();
        replayController.SetFrame(frame);
        ghostObject.SetActive(false);
        fighterLoader.AllowLoading();
        if (bodyMeshRenderer != null)
        {
            bodyMeshRenderer.enabled = true;
        }
        lineRenderer.material = transparentTrajectoryMaterial;
        
        TriggerOnTimelineFreed();
    }

    private int CalculateFrameToLoad()
    {
        Vector3 position = grabInteractable.transform.position;
        int nearestFrame = 0;

        float nearestDistance = float.MaxValue;

        for (int i = minFrame; i <= maxFrame; i++)
        {
            Vector3 loggedPosition = transformLogs[i].Position;
            float distance = Vector3.Distance(position, loggedPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestFrame = i;
            }
        }
        return nearestFrame;
    }

    private void SetGhostObjectTransform(int frame)
    {
        ghostObject.transform.position = transformLogs[frame].Position;
        ghostObject.transform.eulerAngles = transformLogs[frame].Rotation;
    }
    protected override void Timeline_OnTimelineUsed(object sender, OnTimelineUsedEventArgs e)
    {
        if (e.usedTimeline != this)
        {
            grabInteractable.enabled = false;
        }

    }
    protected override void Timeline_OnTimelineFreed(object sender, EventArgs e)
    {
        if (replayController.IsReplayWindowChanged() && IsBodyPartShown())
        {
            grabInteractable.enabled = true;
        }
    }

    private bool IsBodyPartShown()
    {
        switch (bodyPart)
        {
            case BodyPart.Head:
                return FighterCoordinator.Instance.IsHeadShown();
            case BodyPart.LeftHand:
                return FighterCoordinator.Instance.IsLeftHandShown();
            case BodyPart.RightHand:
                return FighterCoordinator.Instance.IsRightHandShown();
        }
        return false;
    }

    /*
    int FindNearestVector3Key(Vector3 target, int time, int positionsToCheck)
    {
        positionsToCheck = positionsToCheck / 2;
        int nearestKey = 0;
        float nearestDistance = float.MaxValue;

        for (int i = -positionsToCheck; i <= positionsToCheck; i++)
        {
            if (positions.ContainsKey(time + i))
            {
                float distance = Vector3.Distance(target, positions[time + i]);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestKey = time + i;
                }
            }
        }
        return nearestKey;
    }
    */
}
