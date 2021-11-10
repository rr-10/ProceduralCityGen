using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RoofType 
{
    Flat,
    Pyrimid,
    FrontGabel
}


public class Roof 
{
    public RoofType type { get; private set; }

    public Roof(RoofType roofType = RoofType.Flat)
    {
        this.type = roofType;
    }
}
