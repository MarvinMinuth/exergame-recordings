using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class FighterLoadingStatue : MonoBehaviour
{
    private Savefile savefile;
    private bool isFinished = false;

    private GameObject head;
    private GameObject leftHand, rightHand;
    private GameObject message;
    private GameObject spotLight;
    private GameObject unfinishedSymbol, finishedSymbol;
    private Button loadButton, cancelButton;

    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    private MeshRenderer hmdMesh;

    private FighterLoader loader;
    private XRSimpleInteractable interactable;

    private bool isInitialized = false;

    private void Start()
    {
        if (isInitialized) return;
        Initialize();
    }

    public void SetupStatue(Savefile savefile, Material fighterMaterial, Material hmdMaterial)
    {
        this.savefile = savefile;
        if (!isInitialized) Initialize();
        ChangeMaterial(fighterMaterial, hmdMaterial);
    }

    private void Initialize()
    {
        head = transform.Find("Head").gameObject;
        leftHand = transform.Find("Left Hand").gameObject;
        rightHand = transform.Find("Right Hand").gameObject;
        message = transform.Find("Message").gameObject;
        spotLight = transform.Find("Spot Light").gameObject;

        GameObject messagePanel = message.transform.Find("Canvas").transform.Find("Panel").gameObject;
        loadButton = messagePanel.transform.Find("Load").GetComponent<Button>();
        cancelButton = messagePanel.transform.Find("Cancel").GetComponent<Button>();

        loadButton.onClick.AddListener(LoadReplay);
        cancelButton.onClick.AddListener(HideMessage);

        GameObject cube = transform.Find("Cube").gameObject;
        unfinishedSymbol = cube.transform.Find("Unfinished Symbol").gameObject;
        finishedSymbol = cube.transform.Find("Finished Symbol").gameObject;

        loader = transform.GetComponentInParent<FighterLoader>();
        interactable = transform.GetComponent<XRSimpleInteractable>();

        hmdMesh = transform.Find("Head").Find("HMD").GetComponent<MeshRenderer>();
        AddHeadRenderers();
        AddLeftHandRenderers();
        AddRightHandRenderers();

        finishedSymbol.SetActive(false);
        message.SetActive(false);

        isInitialized = true;
    }

    private void AddHeadRenderers()
    {
        renderers.Add(transform.Find("Head").GetComponent<MeshRenderer>());
        renderers.Add(transform.Find("Head").transform.Find("Nose").GetComponent<MeshRenderer>());
        renderers.Add(transform.Find("Head").transform.Find("Body").GetComponent<MeshRenderer>());
    }

    private void AddLeftHandRenderers()
    {
        renderers.Add(transform.Find("Left Hand").transform.Find("Hand").GetComponent<MeshRenderer>());
        renderers.Add(transform.Find("Left Hand").transform.Find("Arm").GetComponent<MeshRenderer>());
    }

    private void AddRightHandRenderers() 
    {
        renderers.Add(transform.Find("Right Hand").transform.Find("Hand").GetComponent<MeshRenderer>());
        renderers.Add(transform.Find("Right Hand").transform.Find("Arm").GetComponent<MeshRenderer>());
    }

    public void HideStatue()
    {
        HideMessage();
        interactable.enabled = false;
        spotLight.SetActive(false);
        head.SetActive(false);
        rightHand.SetActive(false);
        leftHand.SetActive(false);
    }

    public void ShowStatue()
    {
        interactable.enabled = true;
        spotLight.SetActive(true);
        head.SetActive(true);
        rightHand.SetActive(true);
        leftHand.SetActive(true);
    }

    public void ShowMessage()
    {
        loader.ShowMessage(this);
        message.SetActive(true);
    }
    public void HideMessage()
    {
        loader.HideMessage();
        message.SetActive(false);
    }

    public void ChangeMaterial(Material fighterMaterial, Material hmdMaterial)
    {
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material = fighterMaterial;
        }
        hmdMesh.material = hmdMaterial;
    }
    public void LoadReplay()
    {
        HideStatue();
        loader.LoadReplay(savefile);
    }

    public void Finish()
    {
        unfinishedSymbol.SetActive(false);
        finishedSymbol.SetActive(true);
        isFinished = true;
        HideStatue();
    }

    public bool IsFinished()
    {
        return isFinished;
    }

    public void InteractionsAllowed(bool allowed)
    {
        if (interactable != null) 
        {
            if (!allowed) { interactable.enabled = false; }
            else { interactable.enabled = true; }
        }
    }

    public string GetMessage()
    {
        string message = transform.Find("Message").Find("Canvas").Find("Panel").Find("Text (TMP)").GetComponent<TMP_Text>().text;
        return message;
    }


}
