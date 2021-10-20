using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room 
{
    RectInt bounds;
    Floor[] floors;
    Roof roof;

    public RectInt Bounds { get => bounds; }
    public Floor[] Floors { get => floors; }
    public Roof GetRoof { get => roof; }

    public Room(RectInt bounds)
    {
        this.bounds = bounds;
    }

    public Room(RectInt bounds, Floor[] floors, Roof roof)
    {
        this.bounds = bounds;
        this.floors = floors;
        this.roof = roof;
    }
 
}
