using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    Vector2Int size;
    Room[] rooms;

    public Vector2Int Size { get => size; }
    public Room[] Rooms { get => rooms; }

    public Building(int x, int y, Room[] rooms)
    {
        this.size = new Vector2Int(x, y);
        this.rooms = rooms;
    }    
}
