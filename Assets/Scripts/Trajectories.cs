using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Trajectories : MonoBehaviour
{
    public GameObject ghostObject;
    private int minFrame, maxFrame;
    private List<TransformLog> transformLogs = new List<TransformLog>();
    private LineRenderer lineRenderer;

    public MenuCoordinator menuCoordinator;
    public ReplayController replayController;
    public FighterLoader fighterLoader;

    public Material opaqueTrajectoryMaterial;
    public Material transparentTrajectoryMaterial;

    private bool isDragged = false;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();

        if(menuCoordinator == null ) menuCoordinator = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>();
        if(replayController == null ) replayController = GameObject.FindGameObjectWithTag("ReplayManager").GetComponent<ReplayController>();
        if(fighterLoader == null ) fighterLoader = GameObject.FindGameObjectWithTag("Fighter Loader").GetComponent<FighterLoader>();

        grabInteractable = transform.GetComponent<XRGrabInteractable>();
        grabInteractable.enabled = false;
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

    public void OnDrag()
    {
        menuCoordinator.GetReplayController().Pause();
        menuCoordinator.StopTimelineUsage();
        fighterLoader.PreventLoading();
        ghostObject.SetActive(true);
        isDragged = true;
        if(transform.Find("Body") != null)
        {
            transform.Find("Body").GetComponent<MeshRenderer>().enabled = false;
        }
        lineRenderer.material = opaqueTrajectoryMaterial;
    }

    public void WhileDragged()
    {
        int frame = CalculateFrameToLoad();
        SetGhostObjectTransform(frame);
        replayController.SetFrame(frame);
    }

    public void OnEndDrag()
    {
        isDragged = false;
        int frame = CalculateFrameToLoad();
        replayController.SetFrame(frame);
        ghostObject.SetActive(false);
        menuCoordinator.AllowTimelineUsage();
        fighterLoader.AllowLoading();
        if (transform.Find("Body") != null)
        {
            transform.Find("Body").GetComponent<MeshRenderer>().enabled = true;
        }
        lineRenderer.material = transparentTrajectoryMaterial;
    }

    private int CalculateFrameToLoad()
    {
        Vector3 position = transform.position;
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
