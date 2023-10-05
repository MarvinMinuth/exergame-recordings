using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class ArmsCoordinator : MonoBehaviour
{
    public string prefabFolder = "FightingDummy/Prefabs";

    public GameObject bottomArmBase;
    public GameObject middleArmBase;
    public GameObject topArmBase;
    private List<GameObject> armBases;

    private GameObject bottomArmSlot;
    private GameObject middleArmSlot;
    private GameObject topArmSlot;
    private List<GameObject> armSlots;

    private Dictionary<GameObject, GameObject> activeArms = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        //LoadPrefabs();

        bottomArmSlot = FindArmSlot(bottomArmBase);
        middleArmSlot = FindArmSlot(middleArmBase);
        topArmSlot = FindArmSlot(topArmBase);

        armBases = new List<GameObject>();
        armBases.Add(bottomArmBase);
        armBases.Add(middleArmBase);
        armBases.Add(topArmBase);
    }

    GameObject FindArmSlot(GameObject armBase)
    {
        return armBase.transform.Find("Binding").Find("ArmSlot").gameObject;
    }

    /*
    void LoadPrefabs()
    {
        prefabList = new List<GameObject>();

        // Suche im AssetDatabase nach Prefabs im angegebenen Ordner
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });

        // Durchlaufe die gefundenen GUIDs
        foreach (string prefabGUID in prefabGUIDs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                prefabList.Add(prefab);
            }
        }
    }
    */

    public void SetPosition(GameObject armBase, Vector3 position)
    {
        if (armBase == null)
        {
            return;
        }
        armBase.transform.position = position;
    }

    public void SetRotation(GameObject armBase, Quaternion rotation)
    {
        if (armBase == null)
        {
            return;
        }
        armBase.transform.rotation = rotation;
    }

    public void SetTargetPosition(GameObject armBase, Vector3 targetPosition)
    {
        Transform target = CheckForTarget(armBase);
        if (target == null)
        {
            return;
        }
        target.transform.position = targetPosition;
    }

    public void SetTargetRotation(GameObject armBase, Quaternion rotation)
    {
        Transform target = CheckForTarget(armBase);
        if (target == null)
        {
            return;
        }
        target.transform.rotation = rotation;
    }

    private Transform CheckForTarget(GameObject armBase)
    {
        if (armBase == null || !activeArms.ContainsKey(armBase) || activeArms[armBase] == null)
        {
            return null;
        }
        Transform target = activeArms[armBase].transform.Find("Target");

        return target;
    }

    public void AttachToArmBase(GameObject armBase, string arm)
    {
        GameObject armSlot = FindArmSlot(armBase);
        GameObject spawnArm = armSlot.transform.Find(arm).gameObject;
        if (spawnArm == null || (activeArms.ContainsKey(armBase) && activeArms[armBase] == spawnArm))
        {
            return;   
        }
        DetachFromArmBase(armBase);
        spawnArm.SetActive(true);
        // GameObject activeArm = Instantiate(spawnArm, armSlot.transform);
        activeArms.Add(armBase, spawnArm);
    }

    /*
    bool IsInstanceOfPrefab(GameObject obj, GameObject prefab)
    {
        if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) == prefab)
            return true;

        return false;
    }
    */

    public void DetachFromArmBase(GameObject armBase)
    {
        if (!activeArms.ContainsKey(armBase))
        {
            return;
        }
        activeArms[armBase].SetActive(false);
        //Destroy(activeArms[armBase]);
        activeArms.Remove(armBase);
    }

    /*
    public void SetBottomTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        bottomArmBase.transform.position = position;
        bottomArmBase.transform.rotation = rotation;
        bottomArmBase.transform.localScale = scale;
    }

    public void SetBottomTargetTransform(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        if (bottomArmSlot.transform.childCount < 2 || bottomArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        bottomArmSlot.transform.GetChild(1).Find("Target").transform.position = targetPosition;
        bottomArmSlot.transform.GetChild(1).Find("Target").transform.rotation = targetRotation;
        bottomArmSlot.transform.GetChild(1).Find("Target").transform.localScale = targetScale;
    }

    public void SetMiddleTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        middleArmBase.transform.position = position;
        middleArmBase.transform.rotation = rotation;
        middleArmBase.transform.localScale = scale;
    }

    public void SetMiddleTargetTransform(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        if (middleArmSlot.transform.childCount < 2 || middleArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        middleArmSlot.transform.GetChild(1).Find("Target").transform.position = targetPosition;
        middleArmSlot.transform.GetChild(1).Find("Target").transform.rotation = targetRotation;
        middleArmSlot.transform.GetChild(1).Find("Target").transform.localScale = targetScale;
    }

    public void SetTopTransform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        topArmBase.transform.position = position;
        topArmBase.transform.rotation = rotation;
        topArmBase.transform.localScale = scale;
    }

    public void SetTopTargetTransform(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale)
    {
        if (topArmSlot.transform.childCount < 2 || topArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        topArmSlot.transform.GetChild(1).Find("Target").transform.position = targetPosition;
        topArmSlot.transform.GetChild(1).Find("Target").transform.rotation = targetRotation;
        topArmSlot.transform.GetChild(1).Find("Target").transform.localScale = targetScale;
    }

    public void SetHeights(float bottomHeight, float middleHeight, float topHeight)
    {
        Vector3 bottomPosition = bottomArmBase.transform.position;
        bottomPosition.y = bottomHeight;
        bottomArmBase.transform.position = bottomPosition;

        Vector3 middlePosition = middleArmBase.transform.position;
        middlePosition.y = middleHeight;
        middleArmBase.transform.position = middlePosition;

        Vector3 topPosition = topArmBase.transform.position;
        topPosition.y = topHeight;
        topArmBase.transform.position = topPosition;
    }

    private void SetRotation(GameObject armBase, float rotation)
    {
        Quaternion newRotation = Quaternion.Euler(0f, rotation, 0f);
        armBase.transform.localRotation = newRotation;
    }

    public void SetBottomRotation(float rotation)
    {
        SetRotation(bottomArmBase, rotation);
    }

    public void SetMiddleRotation(float rotation)
    {
        SetRotation(middleArmBase, rotation);
    }

    public void SetTopRotation(float rotation)
    {
        SetRotation(topArmBase, rotation);
    }

    public void SetBottomTarget(Quaternion rotation)
    {
        if(bottomArmSlot.transform.childCount < 2 || bottomArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        bottomArmSlot.transform.GetChild(1).Find("Target").transform.rotation = rotation;

    }

    public void SetMiddleTarget(Quaternion rotation)
    {
        if (middleArmSlot.transform.childCount < 2 || middleArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        middleArmSlot.transform.GetChild(1).Find("Target").transform.rotation = rotation;

    }

    public void SetTopTarget(Quaternion rotation)
    {
        if (topArmSlot.transform.childCount < 2 || topArmSlot.transform.GetChild(1).Find("Target") == null)
        {
            return;
        }
        topArmSlot.transform.GetChild(1).Find("Target").transform.rotation = rotation;

    }

    public void AttachToBottom(string arm)
    {
        if (bottomArmSlot.transform.childCount > 1)
        {
            return;
        }
        GameObject spawnArm = GetPrefabByName(arm);
        if (spawnArm != null)
        {
            Instantiate(spawnArm, bottomArmSlot.transform);
        }      
    }

    public void AttachToMiddle(string arm)
    {
        if (middleArmSlot.transform.childCount > 1)
        {
            return;
        }
        GameObject spawnArm = GetPrefabByName(arm);
        if (spawnArm != null)
        {
            Instantiate(spawnArm, middleArmSlot.transform);
        }
    }

    public void AttachToTop(string arm)
    {
        if (topArmSlot.transform.childCount > 1)
        {
            return;
        }
        GameObject spawnArm = GetPrefabByName(arm);
        if (spawnArm != null)
        {
            Instantiate(spawnArm, topArmSlot.transform);
        }
    }

    */
    public void DetachFromBottom()
    {
        /*
        if (bottomArmSlot.transform.childCount < 2)
        {
            return; 
        }
        Destroy(bottomArmSlot.transform.GetChild(1).gameObject);
        */
        DetachFromArmBase(bottomArmBase);

    }

    public void DetachFromMiddle()
    {
        /*
        if (middleArmSlot.transform.childCount < 2)
        {
            return;
        }
        Destroy(middleArmSlot.transform.GetChild(1).gameObject);
        */
        DetachFromArmBase(middleArmBase);

    }

    public void DetachFromTop()
    {
        /*
        if (topArmSlot.transform.childCount < 2)
        {
            return;
        }
        Destroy(topArmSlot.transform.GetChild(1).gameObject);
        */
        DetachFromArmBase(topArmBase);

    }

    public void DetachAll()
    {
        DetachFromBottom();
        DetachFromMiddle();
        DetachFromTop();
    }

    /*
    private GameObject GetPrefabByName(string name)
    {
        foreach (GameObject prefab in prefabList)
        {
            if (prefab.name == name)
            {
                return prefab;
            }
        }

        Debug.Log("Arm " + name + " not found!");
        return null; // Kein Prefab mit dem angegebenen Namen gefunden
    }
    */

    public void ResetArms()
    {
        DetachAll();
        foreach(GameObject armBase in armBases)
        {
            SetPosition(armBase, new Vector3(0, 0, 0));
            SetRotation(armBase, new Quaternion(0, 0, 0, 0));
        }
    }
}
