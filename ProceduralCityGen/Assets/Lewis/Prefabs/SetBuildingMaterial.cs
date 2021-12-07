using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBuildingMaterial : MonoBehaviour
{
    [SerializeField] private Renderer WallRenderer;

    [SerializeField] private Renderer DoorRenderer;

    [SerializeField] private Renderer FloorRenderer;
    
    //TODO : Can have more functions for the balcony and doors for more variation to colouring 
    public void SetWallMaterial(Material materialToUse)
    {
        WallRenderer.material = materialToUse;

        if (FloorRenderer)
        {
            FloorRenderer.material = materialToUse;
        }
    }
    
    public void SetDoorMaterial(Material materialToUse)
    {
        if (DoorRenderer)
        {
            DoorRenderer.material = materialToUse;
        }
    }
}
