using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDemo : MonoBehaviour
{
    public BuildingSettings settings;

    // Start is called before the first frame update
    void Start()
    {
        Building b1 = BuildingGeneration.GenerateBuilding(settings);
        GetComponent<BuildingRenderer>().Render(b1);
    }
}
