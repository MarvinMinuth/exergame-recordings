using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskWall : MonoBehaviour
{
    private TMP_Text textComponent;
    // Start is called before the first frame update
    void Start()
    {
        textComponent = transform.Find("Canvas").Find("Panel").Find("Text (TMP)").GetComponent<TMP_Text>();
        DeleteText();
    }
    public void SetText(string text)
    {
        textComponent.text = text;
    }

    public void DeleteText()
    {
        textComponent.text = string.Empty;
    }

    public void AddText(string text)
    {
        textComponent.text += "\n" + text;
    }
}
