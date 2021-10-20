using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Building b1 = BuildingGeneration.GenerateBuilding();
        GetComponent<BuildingRenderer>().Render(b1);
    }
}
