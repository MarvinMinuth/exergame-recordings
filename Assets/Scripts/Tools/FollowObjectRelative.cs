using UnityEngine;

public class FollowObjectRelative : MonoBehaviour
{
    public Transform target; // Das Objekt, dem gefolgt werden soll
    public bool ignoreYPosition = false; // Option zum Ignorieren der y-Position
    public bool onlyYRotation = false; // Option zum Anpassen nur der y-Rotation
    public bool noRotation;
    public bool followPermanently = true;

    private Vector3 initialOffsetPosition;
    private Quaternion initialRelativeRotation;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Kein Zielobjekt zugewiesen!");
            return;
        }
            // Initialen Positionsunterschied (Offset) zwischen diesem Objekt und dem Zielobjekt speichern
            initialOffsetPosition = transform.position - target.position;

            // Initiale relative Rotation zwischen diesem Objekt und dem Zielobjekt speichern
            initialRelativeRotation = Quaternion.Inverse(target.rotation) * transform.rotation;
    }

    void Update()
    {
        if (target == null)
            return;

        // Position und Rotation dieses Objekts entsprechend dem Zielobjekt aktualisieren
        if (followPermanently)
        {
            FollowTargetRelative();
        }
        
    }

    public void FollowTargetRelative()
    {
        // Neue Position berechnen, indem der initiale Offset zur aktuellen Position des Zielobjekts addiert wird
        Vector3 newPosition = target.position + target.rotation * initialOffsetPosition;

        if (ignoreYPosition)
        {
            newPosition.y = transform.position.y; // y-Position des GameObject beibehalten
        }

        transform.position = newPosition;

        if (noRotation)
        {
            return;
        }
        else if (onlyYRotation)
        {
            // Nur die y-Rotation anpassen
            float yRotation = target.eulerAngles.y + (initialRelativeRotation.eulerAngles.y - target.rotation.eulerAngles.y);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, yRotation-180, transform.rotation.eulerAngles.z);
        }
        else
        {
            // Neue Rotation berechnen, indem die initiale relative Rotation zur aktuellen Rotation des Zielobjekts addiert wird
            transform.rotation = target.rotation * initialRelativeRotation;
        }
    }
}
