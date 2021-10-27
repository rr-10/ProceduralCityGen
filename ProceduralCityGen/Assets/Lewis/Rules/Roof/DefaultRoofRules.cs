using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRoofRules : RoofRules
{
    public override Roof GenerateRoof (BuildingSettings settings, RectInt bounds)
    {
        return new Roof(RoofType.Flat, RoofDirection.North); // Set the roof to default settings 
    }
}
