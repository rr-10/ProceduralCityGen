using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Normal,
    Window,
    Door,
    Balcony
}
public enum WallSide : ushort
{
    North = 270,
    East = 0,
    South = 90,
    West = 180
}
public class Wall
{
    public WallType Type { get; private set; }
    public WallSide Side { get; set; }

    public Wall(WallType wallType, WallSide side)
    {
        this.Type = wallType;
        this.Side = side;
    }
}