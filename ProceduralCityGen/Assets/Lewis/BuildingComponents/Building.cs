using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class Building
{
    public List<Floor> Floors { get; private set; }

    public int NumberOfFloors { get; private set; } = 0;

    public Transform Offset;
    public Quaternion Rotation;

    private int sizeX = 0;
    private int sizeY = 0;

    public Building(int x, int y , Transform offset, Quaternion rotation)
    {
        Floors = new List<Floor>();
        this.Offset = offset;
        this.Rotation = rotation;
        this.sizeX = x;
        this.sizeY = y;
    }

    public bool AddFloor(BuildProcess buildProcess)
    {
        Floor floor = new Floor(NumberOfFloors);
        if (NumberOfFloors == 0)
        {
            floor.CreateFirstFloor(sizeX, sizeY); 
        }
        else
        {
            floor.CreateFromPreviousFloor(Floors.Last(), buildProcess);
        }
        
        //Create the walls for that floor, this function will also place doors, windows and balconies
        floor.GenerateWalls();
        
        Floors.Add(floor);
        NumberOfFloors++;

        //Check if the last floor created was all roofed 
        return Floors.Last().CheckForComplete();
    }
}