using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFighterButton : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshRendererArray;

    public void SetMaterial(Material material)
    {
        foreach (MeshRenderer renderer in meshRendererArray)
        {
            renderer.material = material;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
