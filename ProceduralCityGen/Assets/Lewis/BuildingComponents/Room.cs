using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public List<Wall> Walls { get; private set; }
    public Roof RoomRoof { get; private set; }

    public bool IsInterior { get; private set; } = false;
    public bool HasRoof { get;  set; }
    public Vector2 Position { get;  set; }

    public Room(Vector2 position)
    {
        this.Position = position;
    }

    public void SetIsInterior(bool flag)
    {
        this.IsInterior = flag;
    }

    public void CreateWalls(int floorLevel, WallSide side)
    {
        //Create list of walls if needed 
        if (Walls == null) Walls = new List<Wall>();

        //Randomly pick a wall type based on the floor
        if (floorLevel == 0)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= GenerateBuilding.WindowPercentChance)
            {
                Walls.Add(new Wall(WallType.Window, side) );
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= GenerateBuilding.DoorPercentChance)
            {
                Walls.Add(new Wall(WallType.Door, side));
            }
            else
            {
                Walls.Add(new Wall(WallType.Normal, side));
            }
        }
        else
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= GenerateBuilding.BalconyPercentChance)
            {
                Walls.Add(new Wall(WallType.Balcony, side) );
            }
            else if (UnityEngine.Random.Range(0.0f, 1.0f) <= GenerateBuilding.WindowPercentChance)
            {
                Walls.Add(new Wall(WallType.Window, side));
            }
            else
            {
                Walls.Add(new Wall(WallType.Normal, side));
            }
        }
    }
}