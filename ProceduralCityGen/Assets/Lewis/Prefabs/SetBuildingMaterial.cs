using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuildingMaterial : MonoBehaviour
{
    [SerializeField] private Renderer ObjectRenderer;
    //TODO : Can have more functions for the balcony and doors for more variation to colouring 
    public void SetWallMaterial(Material materialToUse)
    {
        ObjectRenderer.material = materialToUse;
    }
}
