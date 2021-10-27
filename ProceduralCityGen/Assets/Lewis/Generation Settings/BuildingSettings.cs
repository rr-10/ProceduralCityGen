using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building Generation") ]
public class BuildingSettings : ScriptableObject
{
    public Vector2Int buildingSize;
    public RoofRules roofRule;
    public WallRules wallRule;
    
    //faster to type getter
    public Vector2Int Size { get => buildingSize; }
    
}
