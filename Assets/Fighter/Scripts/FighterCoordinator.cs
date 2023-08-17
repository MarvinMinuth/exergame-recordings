using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class FighterCoordinator : MonoBehaviour
{
    [SerializeField]
    public int saveFileNumber;
    public Material fighterMaterial;
    public Material hmdMaterial;

    private GameObject head;
    private GameObject leftHand, rightHand;

    private MeshRenderer[] renderers;

    private MeshRenderer hmdMesh;

    FighterLoader loader;
    void Start()
    {
        head = transform.Find("Head").gameObject;
        leftHand = transform.Find("Left Hand").gameObject;
        rightHand = transform.Find("Right Hand").gameObject;
        loader = transform.GetComponentInParent<FighterLoader>();     
    }

    private void OnEnable()
    {
        hmdMesh = transform.Find("Head").Find("HMD").GetComponent<MeshRenderer>();
        renderers = transform.GetComponentsInChildren<MeshRenderer>();
        ChangeMaterial(fighterMaterial);
    }
    public void SetPosition(GameObject bodyPart, Vector3 position)
    {
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

    public void LoadReplay()
    {
        loader.LoadReplay(saveFileNumber, transform.gameObject);
    }

    public GameObject GetHead() { return head; }
    public GameObject GetLeftHand() {  return leftHand; }
    public GameObject GetRightHand() {  return rightHand; }

    public void ChangeMaterial(Material material)
    {
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = material;
        }
        hmdMesh.material = hmdMaterial;
    }

    public void ChangeToIdleMaterial()
    {
        ChangeMaterial(Resources.Load<Material>("Assets/Fighter/Materials/FighterMaterial Idle.mat"));
    }
}
