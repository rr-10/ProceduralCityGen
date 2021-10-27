using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoofRules : ScriptableObject
{
   public abstract Roof GenerateRoof(BuildingSettings settings, RectInt bounds);   
}
