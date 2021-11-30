using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;

public class Building
{
    public List<Floor> Floors { get; private set; }

    public int NumberOfFloors { get; private set; } = 0;


    private int sizeX = 0;
    private int sizeY = 0;

    public Building(int x, int y)
    {
        Floors = new List<Floor>();
        this.sizeX = x;
        this.sizeY = y;
    }

    public bool AddFloor(BuildProcess buildProcess)
    {
        //Create a new floor and generate the walls, the first floor is generated from scratch and following floors have to be generated from the previous floor
        Floor floor = new Floor(NumberOfFloors);
        if (NumberOfFloors == 0)
        {
            floor.CreateFirstFloor(sizeX, sizeY);
            floor.GenerateWalls();
        }
        else
        {
            floor.CreateFromPreviousFloor(Floors.Last(), buildProcess);
            floor.GenerateWalls(Floors.Last());
        }


        Floors.Add(floor);
        NumberOfFloors++;

        //Check if the last floor created was all roofed 
        return Floors.Last().CheckForComplete();
    }
}