using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawnBuilding : MonoBehaviour
{
    GenerateBuilding gen;

    private int gridX = 10;
    private int gridY = 10;
    public int Spacing = 100;

    // Start is called before the first frame update
    void Start()
    {
        gen = GetComponent<GenerateBuilding>();

        //Spawn Buildings on Grid
        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                //Inrease the maximum floors and balcony chance as i and j increase
                gen.setBalconyChance = i * 0.1f;
                gen.MaximumFloors = (gridY - j) + 2;
                gen.Generate(new Vector3(i * Spacing, 0, j * Spacing), Random.Range(3, 5), new Vector3(-45f, 45f, -45f));
            }
        }
    }
}
