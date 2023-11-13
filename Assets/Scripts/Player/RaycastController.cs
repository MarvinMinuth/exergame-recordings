using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastController : MonoBehaviour
{
    public GameObject leftRayInteractor, rightRayInteractor;
    public InputActionReference leftActionReference, rightActionReference;

    private bool leftRayActive, rightRayActive;

    void Start()
    {
        leftActionReference.action.performed += ChangeLeftRay;
        rightActionReference.action.performed += ChangeRightRay;

        leftRayInteractor.SetActive(false);
        rightRayInteractor.SetActive(false);
    }

    public void ChangeLeftRay(InputAction.CallbackContext context)
    {
        if (leftRayActive)
        {
            leftRayActive = false;
            leftRayInteractor.SetActive(false);
        }
        else
        {
            leftRayActive = true;
            leftRayInteractor.SetActive(true);
        }
    }

    public void ChangeRightRay(InputAction.CallbackContext context)
    {
        if (rightRayActive)
        {
            rightRayActive = false;
            rightRayInteractor.SetActive(false);
        }
        else
        {
            rightRayActive = true;
            rightRayInteractor.SetActive(true);
        }
    }
}
