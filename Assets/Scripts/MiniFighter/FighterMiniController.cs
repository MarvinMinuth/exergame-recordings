using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterMiniController : MonoBehaviour
{
    [SerializeField] private FighterMiniStatue fighterMiniStatue;
    [SerializeField] private InputActionReference ActionReference;

    private void Start()
    {
        ActionReference.action.performed += ChangeMiniVisibility;
    }

    private void ChangeMiniVisibility(InputAction.CallbackContext context)
    {
        if(fighterMiniStatue.miniIsActivated)
        {
            fighterMiniStatue.Hide();
        }
        else
        {
            fighterMiniStatue.Show();
        }
    }
}
