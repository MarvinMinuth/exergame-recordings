using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FighterMiniStatue : MonoBehaviour
{
    public GameObject fighter;
    private GameObject head;
    private GameObject body;
    private GameObject leftHand, rightHand;
    public MenuCoordinator menuCoordinator;
    public Material defaultMaterial;

    private GameObject headButton, bodyButton, leftHandButton, rightHandButton;



    private bool headShown, bodyShown, leftHandShown, rightHandShown;
    private bool miniShown;
    private Material usedMaterial, hmdMaterial, transparentMaterial;

    void Start()
    {
        if (menuCoordinator == null)
        {
            menuCoordinator = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>();
        }

        if (fighter == null)
        {
            fighter = GameObject.FindGameObjectWithTag("Fighter");
        }

        head = fighter.transform.Find("Head").gameObject;
        body = head.transform.Find("Body").gameObject;
        leftHand = fighter.transform.Find("Left Hand").gameObject;
        rightHand = fighter.transform.Find("Right Hand").gameObject;

        headButton = transform.Find("Head").gameObject;
        bodyButton = transform.Find("Body").gameObject;
        leftHandButton = transform.Find("Left Hand").gameObject;
        rightHandButton = transform.Find("Right Hand").gameObject;

        transparentMaterial = menuCoordinator.transparentFighterMaterial;
        usedMaterial = defaultMaterial;

        gameObject.SetActive(false);
    }

    public void SetupFighterButtons()
    {
        usedMaterial = fighter.GetComponent<FighterCoordinator>().GetActiveMaterial();
        hmdMaterial = fighter.GetComponent<FighterCoordinator>().hmdMaterial;

        SetHeadButtonActive();
        SetBodyButtonActive();
        SetLeftHandButtonActive();
        SetRightHandButtonActive();

        head.GetComponent<LineRenderer>().enabled = true;
        leftHand.GetComponent<LineRenderer>().enabled = true;
        rightHand.GetComponent<LineRenderer>().enabled = true;
    }

    public void ResetFighterButtons()
    {
        usedMaterial = defaultMaterial;

        SetHeadButtonInactive();
        SetBodyButtonInactive();
        SetLeftHandButtonInactive();
        SetRightHandButtonInactive();

        head.GetComponent<LineRenderer>().enabled = false;
        leftHand.GetComponent<LineRenderer>().enabled = false;
        rightHand.GetComponent<LineRenderer>().enabled = false;
    }

    public void OnHeadButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterHeadShown)
        {
            menuCoordinator.fighterHeadShown = false;

            SetHeadButtonInactive();

            head.GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = transparentMaterial;
            head.GetComponent<LineRenderer>().enabled = false;
            head.GetComponent<XRGrabInteractable>().enabled = false;
        }
        else
        {
            menuCoordinator.fighterHeadShown = true;

            SetHeadButtonActive();

            head.GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = hmdMaterial;
            head.GetComponent<LineRenderer>().enabled = true;
            head.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }
    public void SetHeadButtonActive()
    {
        Debug.Log("Head active");
        headShown = true;
        headButton.GetComponent<MeshRenderer>().material = usedMaterial;
        headButton.transform.Find("Nose").GetComponent<MeshRenderer>().material = usedMaterial;
        headButton.transform.Find("HMD").GetComponent<MeshRenderer>().material = hmdMaterial;
    }
    public void SetHeadButtonInactive()
    {
        Debug.Log("Head inactive");
        headShown = false;
        headButton.GetComponent<MeshRenderer>().material = transparentMaterial;
        headButton.transform.Find("Nose").GetComponent<MeshRenderer>().material = transparentMaterial;
        headButton.transform.Find("HMD").GetComponent<MeshRenderer>().material = transparentMaterial;
    }

    public void OnBodyButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterBodyShown)
        {
            menuCoordinator.fighterBodyShown = false;
            SetBodyButtonInactive();
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        else
        {
            menuCoordinator.fighterBodyShown = true;
            SetBodyButtonActive();
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = usedMaterial;
        }
    }
    public void SetBodyButtonActive()
    {
        bodyShown = true;
        bodyButton.GetComponent<MeshRenderer>().material = usedMaterial;
    }
    public void SetBodyButtonInactive()
    {
        bodyShown = false;
        bodyButton.GetComponent<MeshRenderer>().material = transparentMaterial;
    }

    public void OnLeftHandButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterLeftHandShown)
        {
            menuCoordinator.fighterLeftHandShown = false;

            SetLeftHandButtonInactive();

            foreach (MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = transparentMaterial;
            }
            leftHand.GetComponent<LineRenderer>().enabled = false;
            leftHand.GetComponent<XRGrabInteractable>().enabled = false;
        }
        else
        {
            menuCoordinator.fighterLeftHandShown = true;

            SetLeftHandButtonActive();

            foreach (MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }

            leftHand.GetComponent<LineRenderer>().enabled = true;
            leftHand.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }
    public void SetLeftHandButtonActive()
    {
        leftHandShown = true;
        foreach (MeshRenderer renderer in leftHandButton.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = usedMaterial;
        }
    }
    public void SetLeftHandButtonInactive()
    {
        leftHandShown = false;
        foreach (MeshRenderer renderer in leftHandButton.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = transparentMaterial;
        }
    }

    public void OnRightHandButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterRightHandShown)
        {
            menuCoordinator.fighterRightHandShown = false;

            SetRightHandButtonInactive();

            foreach (MeshRenderer renderer in rightHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = transparentMaterial;
            }
            rightHand.GetComponent<LineRenderer>().enabled = false;
            rightHand.GetComponent<XRGrabInteractable>().enabled = false;
        }
        else
        {
            menuCoordinator.fighterRightHandShown = true;

            SetRightHandButtonActive();

            foreach (MeshRenderer renderer in rightHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }
            rightHand.GetComponent<LineRenderer>().enabled = true;
            rightHand.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }
    public void SetRightHandButtonActive()
    {
        rightHandShown = true;
        foreach (MeshRenderer renderer in rightHandButton.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = usedMaterial;
        }
    }
    public void SetRightHandButtonInactive()
    {
        rightHandShown = false;
        foreach (MeshRenderer renderer in rightHandButton.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.material = transparentMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        CheckBodyButtonStatus(menuCoordinator.fighterBodyShown);
        CheckHeadButtonStatus(menuCoordinator.fighterHeadShown);
        CheckLeftHandButtonStatus(menuCoordinator.fighterLeftHandShown);
        CheckRightHandButtonStatus(menuCoordinator.fighterRightHandShown);
    }

    public void CheckBodyButtonStatus(bool active)
    {
        if(active && !bodyShown) { SetBodyButtonActive(); }
        if(!active && bodyShown) { SetBodyButtonInactive(); }
    }
    public void CheckHeadButtonStatus(bool active)
    {
        if (active && !headShown) { SetHeadButtonActive(); }
        if (!active && headShown) { SetHeadButtonInactive(); }
    }
    public void CheckLeftHandButtonStatus(bool active)
    {
        if (active && !leftHandShown) { SetLeftHandButtonActive(); }
        if (!active && leftHandShown) { SetLeftHandButtonInactive(); }
    }
    public void CheckRightHandButtonStatus(bool active)
    {
        if (active && !rightHandShown) { SetRightHandButtonActive(); }
        if (!active && rightHandShown) { SetRightHandButtonInactive(); }
    }
}
