using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WallRules : ScriptableObject
{
    public abstract Wall[] GenerateWalls(BuildingSettings settings,RectInt bounds, int level);
}
