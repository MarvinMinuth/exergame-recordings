using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RigCoordinator : MonoBehaviour
{
    [SerializeField]
    public GameObject head ;
    public GameObject leftHand, rightHand;
    public string filename;


    FighterLoader loader;
    

    public void SetPosition(GameObject bodyPart, Vector3 position)
    {
        bodyPart.transform.position = position;
    }

    public void SetRotation(GameObject bodyPart, Quaternion rotation)
    {
        bodyPart.transform.rotation = rotation;
    }

    public void SetScale(GameObject bodyPart, Vector3 scale)
    {
        bodyPart.transform.localScale = scale;
    }
    void Start()
    {
        loader = transform.GetComponentInParent<FighterLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadReplay()
    {
        loader.LoadReplay(filename, transform.gameObject);
    }
}
