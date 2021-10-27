using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGeneration 
{
    public static Building GenerateBuilding()
    {
        return new Building(4, 4, GenerateRooms());
    }

    static Room[] GenerateRooms()
    {
        return new Room[] { GenerateSingleRoom() };
    }

    static Room GenerateSingleRoom()
    {
        return new Room(
            new RectInt(0, 0, 4, 4),
            GenerateFloors(),
            GenerateRoof());
    }
    
    static Floor[] GenerateFloors()
    {
        return new Floor[] { GenerateSingleFloor() }; 
    }

    static Floor GenerateSingleFloor()
    {
        return new Floor(0, GenerateWalls());
    }

    static Wall[] GenerateWalls()
    {
        return new Wall[16];
    }

    static Roof GenerateRoof()
    {
        return new Roof(RoofType.Pyrimid, RoofDirection.North);
    }
}