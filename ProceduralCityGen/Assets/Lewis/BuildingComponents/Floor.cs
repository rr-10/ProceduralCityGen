using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    public int FloorLevel { get; private set; }
    public Room[,] Rooms { get; private set; }
    public int RoomsWithNoRoof { get; private set; }

    public Floor(int level)
    {
        this.FloorLevel = level;
        RoomsWithNoRoof = 0;
    }

    public void CreateFirstFloor(int x, int y)
    {
        Rooms = new Room[x, y];
        RoomsWithNoRoof = x * y;
        
        //Set all the rooms to be interior and set position
        for (int i = 0; i < Rooms.GetLength(0); i++)
        {
            for (int j = 0; j < Rooms.GetLength(1); j++)
            {
                Rooms[i, j].SetIsInterior(true);
                Rooms[i, j].Position = new Vector2(i, j);
            }
        }
    }

    public void CreateFromPreviousFloor(Floor previous, Process process)
    {
        //TODO : Handle the other processes
        //Get a copy of the previous floor but with only rooms with no roof
        Rooms = (RemoveRoomsWithRoof(previous.Rooms));

        switch (process)
        {
            case Process.ApplyRoof:
                ApplyRoofToAll();
                break;
        }
    }

    //TODO : Select a roof to use 
    private void ApplyRoofToAll()
    {
        for (int x = 0; x < Rooms.GetLength(0); x++)
        {
            for (int y = 0; y < Rooms.GetLength(1); y++)
            {
                if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior)
                {
                    Rooms[x, y].HasRoof = true;
                    RoomsWithNoRoof--;
                }
            }
        }
    }

    public bool CheckForComplete()
    {
        return (RoomsWithNoRoof > 0);
    }

    public void GenerateWalls()
    { 
        //Determine which rooms have exterior walls and where those walls are, creating those walls when found
        
        for (int x = 0; x < Rooms.GetLength(0); x++)
        {
            for (int y = 0; y < Rooms.GetLength(1); y++)
            {
                if (Rooms[x, y].IsInterior)
                {
                    //North Wall
                    if (x == 0)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.North);
                    }
                    else if (!Rooms[x - 1, y].IsInterior)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.North);
                    }
                    
                    //East Wall
                    if (y == Rooms.GetLength(1))
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.East);
                    }
                    else if (!Rooms[x, y + 1].IsInterior)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.East);
                    }
                    
                    //West Wall
                    if (y == 0)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.West);
                    }
                    else if (!Rooms[x, y + 1].IsInterior)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.West);
                    }
                    
                    //South Wall
                    if (x == Rooms.GetLength(0))
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.South);
                    }
                    else if (!Rooms[x + 1, y].IsInterior)
                    {
                        Rooms[x,y].CreateWalls(FloorLevel, WallSide.South);
                    }
                }
            }
        }
    }


    
    private Room[,] RemoveRoomsWithRoof(Room[,] floor)
    {
        Room[,] newFloorRooms = new Room[floor.GetLength(0), floor.GetLength(1)];

        for (int x = 0; x < floor.GetLength(0); x++)
        {
            for (int y = 0; y < floor.GetLength(1); y++)
            {
                if (!floor[x, y].HasRoof && floor[x, y].IsInterior)
                {
                    newFloorRooms[x, y] = new Room(new Vector2(x, y));
                    RoomsWithNoRoof++;
                }
            }
        }

        return newFloorRooms;
    }
}