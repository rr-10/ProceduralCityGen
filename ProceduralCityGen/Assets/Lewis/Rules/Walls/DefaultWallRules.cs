using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWallRules : WallRules
{
    public override Wall[] GenerateWalls(BuildingSettings settings, RectInt bounds, int level)
    {
        //TODO : Check for level and see if doors can be place 
        //TODO : Create some rules for how many doors should be spawned at that level 
        //TODO : Check if the door should be a larger door or if a normal door is better 
        //TODO : Create some rules for which size windows have to be used for that building
    }

}
