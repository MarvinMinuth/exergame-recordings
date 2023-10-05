using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    public int frame;
    private Button button;

    void Start()
    {
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }
    public void OnButtonClick()
    {
        GameObject.FindGameObjectWithTag("Timeline").GetComponent<Timeline>().OnMarkerButtonClick(frame);
    }
    void Update()
    {
        
    }
}
