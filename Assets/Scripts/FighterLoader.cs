using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterLoader : MonoBehaviour
{
    [SerializeField]
    public GameObject replayManagerObject;

    
    ReplayManager replayManager;
    GameObject loadedFighter;

    // Start is called before the first frame update
    void Start()
    {
      replayManager = replayManagerObject.GetComponent<ReplayManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadReplay(string filename, GameObject fighter)
    {
        if (replayManager.IsLoading()) return;
        if (loadedFighter != null)
        {
            loadedFighter.SetActive(true);
        }

        loadedFighter = fighter;
        loadedFighter.SetActive(false);

        replayManager.Load(filename);
    }
}
