using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingGeneration 
{
    public static Building GenerateBuilding()
    {
        return new Building(4, 4,
            new Room[]
            {
                new Room(
                    new RectInt(0,0,4,4),
                    new Floor[]
                    {
                        new Floor(0, new Wall[16])
                    },
                    new Roof(RoofType.Pyrimid, RoofDirection.North)
                    )
            });
    }
}