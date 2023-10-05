using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class HeartAnchor : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject referenceObject;
    [SerializeField] private bool stayOnHeight;
    [SerializeField] private HeartMenu menu;

    [Header("Anchor Rotation")]
    [SerializeField] private bool rotateAnchor;
    [SerializeField] private GameObject anchorRotationTarget;

    [Header("Symbol Rotation")]
    [SerializeField] private GameObject heartSymbol;
    [SerializeField] private bool rotateHeart;
    [SerializeField] private GameObject heartRotationTarget;

    
    
    private float x, y, z;

    void Awake()
    {
        x = transform.position.x - referenceObject.transform.position.x;
        y = transform.position.y - referenceObject.transform.position.y;
        z = transform.position.z - referenceObject.transform.position.z;

        if (heartSymbol == null) { heartSymbol = transform.Find("Heart Symbol").gameObject; }
    }
    void Update()
    {
        if (rotateAnchor) { SetPosition(); }
        SetRotation();
        if (rotateHeart) RotateHeart();


    }

    private void SetRotation()
    {
        LookAtTargetOnYAxis(anchorRotationTarget.transform);
    }

    private void SetPosition()
    {
        float newX, newY, newZ;
        newX = referenceObject.transform.position.x + x;
        if(stayOnHeight) { newY = transform.position.y; }
        else { newY =  referenceObject.transform.position.y + y; }
        newZ = referenceObject.transform.position.z + z;

        transform.position = new Vector3(newX, newY, newZ);
    }

    private void RotateHeart()
    {
        LookAtTargetOnYAxis(heartRotationTarget.transform);
    }

    public GameObject GetHeartSymbol()
    {
        return heartSymbol;
    }
    public void ActivateSymbol()
    {
        heartSymbol.SetActive(true);
    }
    public void DeactivateSymbol()
    {
        heartSymbol.SetActive(false);
    }
    public void ActivateMenu()
    {
        menu.Activate();
    }

    public void DeactivateMenu()
    {
        menu.Deactivate();
    }

    public void SwitchMenu()
    {
        if (menu.isActivated) DeactivateMenu();
        else ActivateMenu();
    }
    private void LookAtTargetOnYAxis(Transform target)
    {
        if (target == null) return; // Frühe Rückkehr, falls kein Ziel zugewiesen ist

        // Bestimmen der Richtung zum Ziel
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0; // Ignorieren der Y-Komponente für die Rotation

        // Bestimmen des gewünschten Rotationswinkels
        Quaternion desiredRotation = Quaternion.LookRotation(directionToTarget);

        // Anwenden der Rotation
        transform.rotation = desiredRotation;
    }
}
