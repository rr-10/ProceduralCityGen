using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
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
                Rooms[i, j] = new Room(new Vector2(i, j));
                Rooms[i, j].SetIsInterior(true);
                Rooms[i, j].Position = new Vector2(i, j);
            }
        }
    }

    public void CreateFromPreviousFloor(Floor previous, BuildProcess buildProcess)
    {
        //TODO : Handle the other processes
        //Get a copy of the previous floor but with only rooms with no roof
        Rooms = (RemoveRoomsWithRoof(previous.Rooms));

        switch (buildProcess)
        {
            case BuildProcess.ApplyRoof:
                ApplyRoofToAll();
                break;
            case BuildProcess.ShrinkColumn:
                ShrinkColumn(previous);
                break;
            case BuildProcess.ShrinkRow:
                ShrinkRow(previous);
                break;
            case BuildProcess.ShrinkRandom:
                ShrinkRandom(previous);
                break;
            default:
                //No Change 
                break;
        }
    }

    private void ShrinkRandom(Floor previous)
    {
        //Randomly select a direction to shrink in 
        bool shrinkDirecion = false;
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.5f)
        {
            shrinkDirecion = !shrinkDirecion;
        }
        
        //TODO : This is temporary. Have this function done properly 
        if (shrinkDirecion)
        {
            ShrinkRow(previous);
        }
        else
        {
            ShrinkColumn(previous);
        }
        
        //Decide how much to shrink 
        //Apply Shrink
    }
    private void ShrinkRow(Floor previous)
    {
        //Find the row that we want to remove
        int rowToRemove = 0;
        //Randomly take from the left or right of the building 
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.5f)
        {
            for (int x = 0; x < Rooms.GetLength(0); x++)
            {
                for (int y = 0; y < Rooms.GetLength(1); y++)
                {
                    if (x == Rooms.GetLength(0) - 1)
                    {
                        return;
                    }

                    if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior && Rooms[x + 1, y].IsInterior)
                    {
                        rowToRemove = x;
                        goto LoopEnd;
                    }
                }
            }
        }
        else
        {
            for (int x = Rooms.GetLength(0) - 1; x > 0 ; x--)
            {
                for (int y = Rooms.GetLength(1) - 1; y > 0; y--)
                {
                    if (x == 1)
                    {
                        return;
                    }

                    if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior && Rooms[x - 1, y].IsInterior)
                    {
                        rowToRemove = x;
                        goto LoopEnd;
                    }
                }
            }
        }

        LoopEnd:

        //Remove the row
        for (int y = 0; y < Rooms.GetLength(1); y++)
        {
            if (!Rooms[rowToRemove, y].HasRoof && Rooms[rowToRemove, y].IsInterior)
            {
                Rooms[rowToRemove, y].SetIsInterior(false);
                Rooms[rowToRemove, y].HasRoof = true;
                RoomsWithNoRoof--;

                //Set the previous floor to have a roof
                previous.Rooms[rowToRemove, y].HasRoof = true;
                previous.Rooms[rowToRemove, y].RoomRoof = new Roof();
            }
        }
    }


    private void ShrinkColumn(Floor previous)
    {
        //Find the column that we want to remove
        int columnToRemove = 0;

        //Randomly take from the front or back of the building 
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.5f)
        {
            for (int y = 0; y < Rooms.GetLength(1); y++)
            {
                for (int x = 0; x < Rooms.GetLength(0); x++)
                {
                    if (y == Rooms.GetLength(0) - 1)
                    {
                        return;
                    }
        
                    if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior && Rooms[x, y + 1].IsInterior)
                    {
                        columnToRemove = y;
                        goto LoopEnd;
                    }
                }
            }
        }
        else
        {
            for (int y = Rooms.GetLength(1) - 1; y > 0; y--)
            {
                for (int x = Rooms.GetLength(0) - 1; x > 0; x--)
                {
                    if (y == 1 )
                    {
                    
                        return;
                    }
                    
                    if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior && Rooms[x, y - 1].IsInterior)
                    {
                        columnToRemove = y;
                        goto LoopEnd;
                    }
                }
            }
        }
        
        LoopEnd:
       
        //Remove the row
        for (int x = 0; x < Rooms.GetLength(0); x++)
        {
            if (!Rooms[x, columnToRemove].HasRoof && Rooms[x, columnToRemove].IsInterior)
            {
                Rooms[x, columnToRemove].SetIsInterior(false);
                Rooms[x, columnToRemove].HasRoof = true;
                RoomsWithNoRoof--;

                //Set the previous floor to have a roof
                previous.Rooms[x, columnToRemove].HasRoof = true;
                previous.Rooms[x, columnToRemove].RoomRoof = new Roof();
            }
        }
    }

    //TODO : Select a roof to use 
    public void ApplyRoofToAll()
    {
        for (int x = 0; x < Rooms.GetLength(0); x++)
        {
            for (int y = 0; y < Rooms.GetLength(1); y++)
            {
                if (!Rooms[x, y].HasRoof && Rooms[x, y].IsInterior)
                {
                    Rooms[x, y].HasRoof = true;
                    RoomsWithNoRoof--;
                    Rooms[x, y].RoomRoof = new Roof();
                }
            }
        }
    }

    public bool CheckForComplete()
    {
        return (RoomsWithNoRoof > 0);
    }

    //TODO : We can determine and pass in wether a set of walls can have a balcony from here and replace the floor level with a flog for canHaveBalconies
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
                    if (x == 0 || !Rooms[x - 1, y].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.North, false);
                    }
                  
                    //East Wall
                    if ((y == Rooms.GetLength(1) - 1) || !Rooms[x, y + 1].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.East, false);
                    }

                    //West Wall
                    if (y == 0 || !Rooms[x, y - 1].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.West, false);
                    }

                    //South Wall
                    if ((x == Rooms.GetLength(0) - 1) || !Rooms[x + 1, y].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.South, false);
                    }
                }
            }
        }
    }
    
    public void GenerateWalls(Floor previous)
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
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.North, true);
                    }
                    else if (!Rooms[x - 1, y].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.North, !previous.Rooms[x - 1, y].IsInterior);
                    }

                    //East Wall
                    if ((y == Rooms.GetLength(1) - 1))
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.East, true);
                    }
                    else if (!Rooms[x, y + 1].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.East, !previous.Rooms[x, y + 1].IsInterior);
                    }

                    //West Wall
                    if (y == 0)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.West, true);
                    }
                    else if (!Rooms[x, y - 1].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.West, !previous.Rooms[x, y - 1].IsInterior);
                    }

                    //South Wall
                    if ((x == Rooms.GetLength(0) - 1))
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.South, true);
                    }
                    else if (!Rooms[x + 1, y].IsInterior)
                    {
                        Rooms[x, y].CreateWalls(FloorLevel, WallSide.South, !previous.Rooms[x + 1, y].IsInterior);
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
                newFloorRooms[x, y] = new Room(new Vector2(x, y));
                if (!floor[x, y].HasRoof && floor[x, y].IsInterior)
                {
                    RoomsWithNoRoof++;
                    newFloorRooms[x, y].SetIsInterior(true);
                }
                else
                {
                    newFloorRooms[x, y].SetIsInterior(false);
                }
            }
        }

        return newFloorRooms;
    }
}