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
    North = 0,
    East = 90,
    South = 180,
    West = 270
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