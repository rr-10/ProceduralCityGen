using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Building_Generator : MonoBehaviour
{
    //just easy way to get prefabs and information dont have to use
    public GameObject cube;
    public GameObject medium;
    public GameObject large;
    public GameObject Meshh;

    private GenerateBuilding gen;
    
    public void GenerateBuildings(int width, int height, float[,] heightmap, int[] BuldingMap,
        AnimationCurve HeightCurve, float mesh_Height
    ) //add references  for your prefabs here or watever if you want or just include them
    {
        gen = GetComponent<GenerateBuilding>();
        
        Vector3 RelativePosition = Meshh.transform.position;

        int size = 0;
        //array for buildings to be put into, 
        for (int i = 0; i < BuldingMap.Length; i++)
        {
            if (BuldingMap[i] != 0 && BuldingMap[i] != 1)
                size++;
        }


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //reset relative position
                RelativePosition.x = (-width * Meshh.transform.localScale.x) / 2 + 5;
                RelativePosition.z = (height * Meshh.transform.localScale.z) / 2 - 5;

                //small buildings
                if (BuldingMap[y * width + x] == 2)
                {
                    RelativePosition.x +=
                        x * Meshh.transform.localScale.x + Meshh.transform.localScale.x / 2; //Get location on Map
                    RelativePosition.z -=
                        y * Meshh.transform.localScale.z +
                        Meshh.transform.localScale.z / 2; //Z is used for Y axis in the 3d world
                    RelativePosition.y =
                        HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y; // Calulate height
                    
                    gen.Generate(RelativePosition, 3);
                    
                    
                }

                //medium building
                else if (BuldingMap[y * width + x] == 3)
                {
                    RelativePosition.x += x * Meshh.transform.localScale.x + Meshh.transform.localScale.x;
                    RelativePosition.z -= y * Meshh.transform.localScale.z + Meshh.transform.localScale.z;
                    RelativePosition.y =
                        HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y ;
                    
                    gen.Generate(RelativePosition, 4);
                }

                //large building
                else if (BuldingMap[y * width + x] == 4)
                {
                    RelativePosition.x += x * Meshh.transform.localScale.x + Meshh.transform.localScale.x +
                                          Meshh.transform.localScale.x / 2;
                    RelativePosition.z -= y * Meshh.transform.localScale.z + Meshh.transform.localScale.z +
                                          Meshh.transform.localScale.z / 2;
                    RelativePosition.y =
                        HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y;

                    gen.Generate(RelativePosition, 5);
                }
            }
        }
    }

    //Clear the existing buildings in the scene
    public void ClearBuildings()
    {
        if (gen)
        {
            gen.Clear();
        }

    }
}