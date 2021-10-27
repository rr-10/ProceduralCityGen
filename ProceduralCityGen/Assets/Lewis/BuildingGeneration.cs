using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGeneration 
{
    public static Building GenerateBuilding(BuildingSettings settings)
    {
        return new Building(settings.Size.x, settings.Size.y, GenerateRooms(settings));
    }

    static Room[] GenerateRooms(BuildingSettings settings)
    {
        return new Room[] { GenerateSingleRoom(settings, new RectInt(0,0, settings.Size.x, settings.Size.y), 1) };
    }

    static Room GenerateSingleRoom(BuildingSettings settings, RectInt bounds, int numberOfFloors)
    {
        return new Room(
            bounds,
            GenerateFloors(settings, bounds, numberOfFloors),
            GenerateRoof(settings, bounds));
    }
    
    static Floor[] GenerateFloors(BuildingSettings settings, RectInt bounds, int numberOfFloors)
    {
        return new Floor[] { GenerateSingleFloor(settings, bounds, 1) }; 
    }

    static Floor GenerateSingleFloor(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Floor(0, GenerateWalls(settings, bounds, level));
    }

    static Wall[] GenerateWalls(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Wall[(bounds.size.x + bounds.size.y ) * 2];
    }

    static Roof GenerateRoof(BuildingSettings settings, RectInt bounds)
    {
        //If a rule is set use it, otherwise use the default rule     
        if (!settings.roofRule)
        {
            return settings.roofRule.GenerateRoof(settings, bounds);
        }

        return ScriptableObject.CreateInstance<DefaultRoofRules>().GenerateRoof(settings, bounds);
    }
}