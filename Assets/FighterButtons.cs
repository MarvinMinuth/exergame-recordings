using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterButtons : MonoBehaviour
{
    private GameObject fighter;
    private GameObject head;
    private GameObject body;
    private GameObject leftHand, rightHand;

    private GameObject buttons;
    private Button headButton, bodyButton, leftHandButton, rightHandButton;

    private bool headShown, bodyShown, leftHandShown, rightHandShown;
    private Material usedMaterial, hmdMaterial, transparentMaterial;

    // Start is called before the first frame update
    void Start()
    {
        headShown = true;
        bodyShown = true;
        leftHandShown = true;
        rightHandShown = true;

        fighter = GameObject.FindGameObjectWithTag("Fighter");
        head = fighter.transform.Find("Head").gameObject;
        body = head.transform.Find("Body").gameObject;
        leftHand = fighter.transform.Find("Left Hand").gameObject;
        rightHand = fighter.transform.Find("Right Hand").gameObject;


        buttons = transform.Find("Buttons").gameObject;
        headButton = buttons.transform.Find("Head").GetComponent<Button>();
        bodyButton = buttons.transform.Find("Body").GetComponent<Button>();
        leftHandButton =buttons.transform.Find("Left Hand").GetComponent<Button>();
        rightHandButton = buttons.transform.Find("Right Hand").GetComponent<Button>();

        headButton.onClick.AddListener(OnHeadButtonClick);
        bodyButton.onClick.AddListener(OnBodyButtonClick);
        leftHandButton.onClick.AddListener(OnLeftHandButtonClick);
        rightHandButton.onClick.AddListener(OnRightHandButtonClick);

        transparentMaterial = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>().transparentFighterMaterial;
        usedMaterial = fighter.GetComponent<FighterCoordinator>().fighterMaterial;
        hmdMaterial = fighter.GetComponent<FighterCoordinator>().hmdMaterial;
    }

    void OnHeadButtonClick()
    {
        if(headShown)
        {
            headShown = false;
            SetButtonActive(headButton);
            head.GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = transparentMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        else
        {
            headShown = true;
            SetButtonInactive(headButton);
            head.GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("Nose").GetComponent<MeshRenderer>().material = usedMaterial;
            head.transform.Find("HMD").GetComponent<MeshRenderer>().material = hmdMaterial;
        }
    }

    void OnBodyButtonClick()
    {
        if (bodyShown)
        {
            bodyShown = false;
            SetButtonActive(bodyButton);
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = transparentMaterial;
        }
        else
        {
            bodyShown = true;
            SetButtonInactive(bodyButton);
            head.transform.Find("Body").GetComponent<MeshRenderer>().material = usedMaterial;
        }
    }

    void OnLeftHandButtonClick()
    {
        if (leftHandShown)
        {
            leftHandShown = false;
            SetButtonActive(leftHandButton);
            foreach(MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = transparentMaterial;
            }
        }
        else
        {
            leftHandShown = true;
            SetButtonInactive(leftHandButton);
            foreach (MeshRenderer renderer in leftHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }
        }
    }

    void OnRightHandButtonClick()
    {
        if (rightHandShown)
        {
            rightHandShown = false;
            SetButtonActive(rightHandButton);
            foreach (MeshRenderer renderer in rightHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = transparentMaterial;
            }
        }
        else
        {
            rightHandShown = true;
            SetButtonInactive(rightHandButton);
            foreach (MeshRenderer renderer in rightHand.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material = usedMaterial;
            }
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
        
    }
}
