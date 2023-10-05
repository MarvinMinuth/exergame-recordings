using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingAnchor : MonoBehaviour
{
    public GameObject referenceObject;
    private float xDistance, zDistance;
    private GameObject heartSymbol;
    // Start is called before the first frame update
    void Start()
    {
        heartSymbol = transform.Find("Heart Symbol").gameObject;
        if (referenceObject == null) return;
        transform.position = new Vector3(referenceObject.transform.position.x, transform.position.y, referenceObject.transform.position.z);
        transform.eulerAngles = new Vector3(0, referenceObject.transform.eulerAngles.y, 0);
        xDistance = transform.position.x - referenceObject.transform.position.x;
        zDistance = transform.position.z - referenceObject.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (referenceObject != null)
        {
            float xPosition = referenceObject.transform.position.x - xDistance;
            float zPosition = referenceObject.transform.position.z - zDistance;
            transform.position = new Vector3(xPosition, transform.position.y, zPosition);
            transform.eulerAngles = new Vector3(0, referenceObject.transform.eulerAngles.y, 0);
        }
    }


}
