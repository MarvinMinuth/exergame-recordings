using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCoordinator : MonoBehaviour
{
    [SerializeField]
    public GameObject timelinePrefab;
    public GameObject fighterButtonsPrefab;
    public GameObject menuPanel;
    public Material transparentFighterMaterial;

    private GameObject timeline;
    private GameObject fighterButtons;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMenu()
    {
        if (timelinePrefab != null)
        {
            timeline = Instantiate(timelinePrefab, menuPanel.transform);
        }

        if (fighterButtonsPrefab != null)
        {
            fighterButtons = Instantiate(fighterButtonsPrefab, menuPanel.transform);
        }
    }

    public void DestroyMenu()
    {
        if (timeline != null) { Destroy(timeline); }
        if (fighterButtons != null) { Destroy(fighterButtons); }
    }



    public GameObject GetTimeline() { return timeline; }
    public GameObject GetFighterButtons() {  return fighterButtons; }
}
