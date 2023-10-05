using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartMenu : MonoBehaviour
{
    public bool isActivated;
    public bool rotating;
    public HeartAnchor anchor;
    float x, y, z;

    private void Start()
    {
        x = transform.position.x - anchor.transform.position.x;
        y = transform.position.y - anchor.transform.position.y;
        z = transform.position.z - anchor.transform.position.z;
    }
    private void Update()
    {
        if(isActivated && rotating)
        {
            LookAtTargetOnYAxis(GameObject.FindGameObjectWithTag("MainCamera").transform);
        }
    }
    public void Deactivate()
    {
        isActivated = false;
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        isActivated = true;
        gameObject.SetActive(true);
        RepositionAtAnchor();
    }

    public void RepositionAtAnchor()
    {
        float newX, newY, newZ;
        newX = anchor.transform.position.x + x;
        newY = anchor.transform.position.y + y;
        newZ = anchor.transform.position.z + z;

        transform.position = new Vector3(newX, newY, newZ);
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
