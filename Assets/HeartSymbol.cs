using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


public class HeartSymbol : MonoBehaviour
{
    [SerializeField] private HeartrateCoordinator heartrateCoordinator;
    [SerializeField] private HeartMenu menu;
    private GameObject heartSymbol;
    [SerializeField] private bool stayOnHeight;
    [SerializeField] private bool lookAtPlayer;
    [SerializeField] private bool onlyRotateOnY;
    
    private float y;

    void Start()
    {
        heartSymbol = transform.Find("Heart Symbol").gameObject;
        if (stayOnHeight) { y = heartSymbol.transform.position.y; }
    }

   

    

    public void OnClick()
    {
        if (menu == null) { return; }
        if (menu.isActivated) { DeactivateMenu(); }
        else { ActivateMenu(); }
    }

    public void ActivateMenu()
    {
        menu.Activate();
    }

    public void DeactivateMenu() { menu.Deactivate(); }
}
