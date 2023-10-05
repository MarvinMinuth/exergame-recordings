using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FighterCoordinator : MonoBehaviour
{
    public Material fighterMaterial;
    public Material hmdMaterial;

    private GameObject head;
    private GameObject leftHand, rightHand;
    private Trajectories headTrajectories, leftHandTrajectories, rightHandTrajectories;
    private Material activeMaterial;

    //private MeshRenderer[] renderers;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    private MeshRenderer hmdMesh;

    private void OnEnable()
    {
        head = transform.Find("Head").gameObject;
        headTrajectories = head.GetComponent<Trajectories>();

        leftHand = transform.Find("Left Hand").gameObject;
        leftHandTrajectories = leftHand.GetComponent<Trajectories>();

        rightHand = transform.Find("Right Hand").gameObject;
        rightHandTrajectories = rightHand.GetComponent<Trajectories>();

        hmdMesh = transform.Find("Head").Find("HMD").GetComponent<MeshRenderer>();

        //renderers = transform.GetComponentsInChildren<MeshRenderer>();
        renderers.Add(transform.Find("Head").GetComponent<MeshRenderer>());
        renderers.AddRange(transform.Find("Head").GetComponentsInChildren<MeshRenderer>());
        renderers.AddRange(transform.Find("Left Hand").GetComponentsInChildren<MeshRenderer>());
        renderers.AddRange(transform.Find("Right Hand").GetComponentsInChildren<MeshRenderer>());
        ChangeMaterial(fighterMaterial);
    }
    private void OnDisable()
    {
        if(headTrajectories != null) { headTrajectories.DestroyTrajectories(); }
        if(leftHandTrajectories != null) { leftHandTrajectories.DestroyTrajectories(); }
        if(rightHandTrajectories != null) { rightHandTrajectories.DestroyTrajectories(); }    
    }

    public void SetPosition(GameObject bodyPart, Vector3 position)
    {
        if (bodyPart == null) { return; }
        bodyPart.transform.position = position;
    }
    public void SetRotation(GameObject bodyPart, Quaternion rotation)
    {
        bodyPart.transform.rotation = rotation;
    }
    public void SetScale(GameObject bodyPart, Vector3 scale)
    {
        bodyPart.transform.localScale = scale;
    }
    public GameObject GetHead() { return head; }
    public GameObject GetLeftHand() {  return leftHand; }
    public GameObject GetRightHand() {  return rightHand; }
    public void ChangeMaterial(Material bodyMaterial)
    {
        foreach (MeshRenderer renderer in renderers)
        {
            if (!renderer.transform.name.Equals("Heart Symbol")){
                renderer.material = bodyMaterial;
            }
        }
        activeMaterial = bodyMaterial;
        hmdMesh.material = hmdMaterial;
    }
    public void ChangeToIdleMaterial()
    {
        activeMaterial = fighterMaterial;
        ChangeMaterial(fighterMaterial);
    }

    public Material GetActiveMaterial() { return activeMaterial; }
}
