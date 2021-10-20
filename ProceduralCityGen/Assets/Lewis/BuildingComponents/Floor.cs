using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor
{
    int level;
    Wall[] walls;

    public int Level { get => level; }
    public Wall[] Walls { get => walls; }

    public Floor(int level, Wall[] walls)
    {
        this.level = level;
        this.walls = walls;
    } 
}
