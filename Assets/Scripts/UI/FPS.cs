using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private int refreshRate = 10;
    private int frameCounter;
    private float totalTime;

    private void Awake()
    {
        frameCounter = 0;
        totalTime = 0;
    }

    private void Update()
    {
        if(frameCounter >= refreshRate)
        {
            float averageFPS = (1.0f / (totalTime / refreshRate));
            textField.text = "FPS: " + averageFPS.ToString("F1");
            frameCounter = 0;
            totalTime = 0;
        }
        else
        {
            totalTime += Time.deltaTime;
            frameCounter++;
        }
    }
}
