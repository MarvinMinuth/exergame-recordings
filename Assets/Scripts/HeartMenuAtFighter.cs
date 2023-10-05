using UnityEngine;

public class HeartMenuAtFighter : HeartMenu
{
    public GameObject referenceObject;
    public GameObject lookAtTarget;
    private float xDistance, zDistance;

    void Start()
    {
        xDistance = transform.position.x - referenceObject.transform.position.x;
        zDistance = transform.position.z - referenceObject.transform.position.z;
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        if (!isActivated)
        {
            float xPosition = referenceObject.transform.position.x + xDistance;
            float zPosition = referenceObject.transform.position.z + zDistance;
            transform.position = new Vector3(xPosition, transform.position.y, zPosition);
        }
        if (lookAtTarget != null)
        {
            Vector3 lookDirection = lookAtTarget.transform.position - transform.position;
            lookDirection.y = 0; // Dies stellt sicher, dass sich die Blickrichtung nur in der XZ-Ebene bewegt
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        }
    }
}
