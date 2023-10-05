using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterMiniController : MonoBehaviour
{
    public GameObject fighterMiniStatue;
    public InputActionReference ActionReference;

    private bool miniIsActivated = false;

    private void OnEnable()
    {
        ActionReference.action.performed += ChangeMiniVisibility;
    }
    

    private void ChangeMiniVisibility(InputAction.CallbackContext context)
    {
        if(miniIsActivated)
        {
            miniIsActivated = false;
            fighterMiniStatue.SetActive(false);
        }
        else
        {
            miniIsActivated = true;
            fighterMiniStatue.SetActive(true);
        }
    }
}
