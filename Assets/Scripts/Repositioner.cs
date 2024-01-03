using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repositioner : MonoBehaviour
{
    [SerializeField] private GameObject positionReferenceObject;
    [SerializeField] private GameObject lookAtObject;
    [SerializeField] private bool rotate;
    [SerializeField] private bool reposition;
    [SerializeField] private bool stayOnHeight;
    private float x, y, z;

    void Start()
    {
        x = positionReferenceObject.transform.position.x - transform.position.x;
        y = positionReferenceObject.transform.position.y - transform.position.y;
        z = positionReferenceObject.transform.position.z - transform.position.z;
    }

    public void Reposition()
    {
        float newX, newY, newZ;
        newX = positionReferenceObject.transform.position.x + x;
        if (stayOnHeight) { newY = transform.position.y; }
        else { newY = positionReferenceObject.transform.position.y + y; }
        newZ = positionReferenceObject.transform.position.z + z;

        transform.position = new Vector3(newX, newY, newZ);
    }

    public void LookAtTarget()
    {
        transform.LookAt(lookAtObject.transform.position);
    }
}
