using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class FighterButtons : MonoBehaviour
{
    public GameObject fighter;
    private GameObject head;
    private GameObject body;
    private GameObject leftHand, rightHand;
    public MenuCoordinator menuCoordinator;

    private Button headButton, bodyButton, leftHandButton, rightHandButton;

    private bool headShown, bodyShown, leftHandShown, rightHandShown;
    private Material usedMaterial, hmdMaterial, transparentMaterial;

    // Start is called before the first frame update
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

        headButton = transform.Find("Head").GetComponent<Button>();
        bodyButton = transform.Find("Body").GetComponent<Button>();
        leftHandButton =transform.Find("Left Hand").GetComponent<Button>();
        rightHandButton = transform.Find("Right Hand").GetComponent<Button>();

        headButton.onClick.AddListener(OnHeadButtonClick);
        bodyButton.onClick.AddListener(OnBodyButtonClick);
        leftHandButton.onClick.AddListener(OnLeftHandButtonClick);
        rightHandButton.onClick.AddListener(OnRightHandButtonClick);

        transparentMaterial = menuCoordinator.transparentFighterMaterial;
    }

    public void SetupFighterButtons()
    {
        usedMaterial = fighter.GetComponent<FighterCoordinator>().fighterMaterial;
        hmdMaterial = fighter.GetComponent<FighterCoordinator>().hmdMaterial;

        SetButtonActive(headButton);
        SetButtonActive(bodyButton);
        SetButtonActive(leftHandButton);
        SetButtonActive(rightHandButton);

        head.GetComponent<LineRenderer>().enabled = true;
        leftHand.GetComponent<LineRenderer>().enabled = true;
        rightHand.GetComponent<LineRenderer>().enabled = true;
    }

    public void ResetFighterButtons()
    {
        SetButtonInactive(headButton);
        SetButtonInactive(bodyButton);
        SetButtonInactive(leftHandButton);
        SetButtonInactive(rightHandButton);

        head.GetComponent<LineRenderer>().enabled = false;
        leftHand.GetComponent<LineRenderer>().enabled = false;
        rightHand.GetComponent<LineRenderer>().enabled = false;
    }

    void OnHeadButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if(menuCoordinator.fighterHeadShown)
        {
            menuCoordinator.fighterHeadShown = false;
            SetButtonInactive(headButton);
            head.GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = transparentMaterial;
            head.GetComponent<LineRenderer>().enabled = false;
            head.GetComponent<XRGrabInteractable>().enabled = false;
        }
        else
        {
            menuCoordinator.fighterHeadShown = true;
            SetButtonActive(headButton);
            head.GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = hmdMaterial;
            head.GetComponent<LineRenderer>().enabled = true;
            head.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }

    void OnBodyButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterBodyShown)
        {
            menuCoordinator.fighterBodyShown = false;
            SetButtonInactive(bodyButton);
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        else
        {
            menuCoordinator.fighterBodyShown = true;
            SetButtonActive(bodyButton);
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = usedMaterial;
        }
    }

    void OnLeftHandButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterLeftHandShown)
        {
            menuCoordinator.fighterLeftHandShown = false;
            SetButtonInactive(leftHandButton);
            foreach(MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = transparentMaterial;
            }
            leftHand.GetComponent<LineRenderer>().enabled = false;
            leftHand.GetComponent<XRGrabInteractable>().enabled = false;
        }
        else
        {
            menuCoordinator.fighterLeftHandShown = true;
            SetButtonActive(leftHandButton);
            foreach (MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }
            leftHand.GetComponent<LineRenderer>().enabled = true;
            leftHand.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }

    void OnRightHandButtonClick()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        if (menuCoordinator.fighterRightHandShown)
        {
            menuCoordinator.fighterRightHandShown = false;
            SetButtonInactive(rightHandButton);
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
            SetButtonActive(rightHandButton);
            foreach (MeshRenderer renderer in rightHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }
            rightHand.GetComponent<LineRenderer>().enabled = true;
            rightHand.GetComponent<XRGrabInteractable>().enabled = true;
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

    // Update is called once per frame
    void Update()
    {
        if (!menuCoordinator.IsMenuActive()) { return; }
        CheckButtonStatus(bodyButton, menuCoordinator.fighterBodyShown);
        CheckButtonStatus(headButton, menuCoordinator.fighterHeadShown);
        CheckButtonStatus(leftHandButton, menuCoordinator.fighterLeftHandShown);
        CheckButtonStatus(rightHandButton, menuCoordinator.fighterRightHandShown);
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
}
