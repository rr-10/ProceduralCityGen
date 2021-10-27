using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Roof Rule") ]
public class SinglePointRoof : RoofRules
{
    public RoofType towerRoof;
    public RoofType normalRoof;

   public override Roof GenerateRoof(BuildingSettings type, RectInt bounds)
   {
       if (bounds.x == 1 && bounds.y == 1)
       {
            return new Roof(towerRoof, RoofDirection.North);
       }

       return new Roof(normalRoof, RoofDirection.North );
   }
}
