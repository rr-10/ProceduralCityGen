using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour
{
    RoofType type;
    RoofDirection direction;

    public RoofType Type { get => type; }
    public RoofDirection Direction { get => direction; }

    public Roof(RoofType type, RoofDirection direction)
    {
        this.type = type;
        this.direction = direction;
    }
}

public enum RoofType { 
    Flat,
    Pyrimid,
    FrontGabel
}

public enum RoofDirection { 
    North,
    East,
    South,
    West
}

