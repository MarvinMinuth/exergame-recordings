using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniFighterButtonHead : MiniFighterButton
{
    [SerializeField] private MeshRenderer hmdMeshRenderer;

    public void SetHMDMaterial(Material material)
    {
        hmdMeshRenderer.material = material;
    }
}
