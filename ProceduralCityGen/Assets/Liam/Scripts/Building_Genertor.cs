using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Building_Genertor : MonoBehaviour
{
    //just easy way to get prefabs and information dont have to use
    public GameObject cube;
    public GameObject medium;
    public GameObject large;
    public GameObject Meshh;

    //dont delete
    GameObject[] Index_Buldings;

    public void GenerateBuildings(int width, int height, float[,] heightmap, int[] BuldingMap, AnimationCurve HeightCurve, float mesh_Height
        ) //add references  for your prefabs here or watever if you want or just include them
    {

      
        Vector3 RelativePosition = Meshh.transform.position;

        int size = 0;
        //array for buildings to be put into, 
        for (int i = 0; i < BuldingMap.Length; i++)
        {
            if (BuldingMap[i] != 0 && BuldingMap[i] != 1)
                size++;
        }

        Index_Buldings = new GameObject[size];
        int size_index = 0;

        for (int y = 0; y < height ; y++)
        {
            for (int x = 0; x < width ; x++)
            {

                
                //reset relative position
                RelativePosition.x = (-width * Meshh.transform.localScale.x) / 2 + 5;
                RelativePosition.z = (height * Meshh.transform.localScale.z) / 2 - 5;

                //small buildings
                if (BuldingMap[y * width + x] == 2)
                {
                    



                        RelativePosition.x += x * Meshh.transform.localScale.x + Meshh.transform.localScale.x / 2; //Get location on Map
                        RelativePosition.z -= y * Meshh.transform.localScale.z + Meshh.transform.localScale.z / 2; //Z is used for Y axis in the 3d world
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y + (cube.transform.localScale.y / 2.2f); // Calulate height

                        //Make sure to increase index
                        Index_Buldings[size_index] = Instantiate(cube, RelativePosition, transform.rotation); //Create object
                        size_index++;
                    
                }

                //medium building
                else if (BuldingMap[y * width + x] == 3)
                {

                    
                        RelativePosition.x += x * Meshh.transform.localScale.x + Meshh.transform.localScale.x;
                        RelativePosition.z -= y * Meshh.transform.localScale.z + Meshh.transform.localScale.z;
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y + (cube.transform.localScale.y / 1.8f);


                        Index_Buldings[size_index] = Instantiate(medium, RelativePosition, transform.rotation);
                        size_index++;
                    
                }

                //large building
                else if (BuldingMap[y * width + x] == 4)
                {
                    

                    
                        RelativePosition.x += x * Meshh.transform.localScale.x + Meshh.transform.localScale.x + Meshh.transform.localScale.x / 2;
                        RelativePosition.z -= y * Meshh.transform.localScale.z + Meshh.transform.localScale.z + Meshh.transform.localScale.z / 2;
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * Meshh.transform.localScale.y + (cube.transform.localScale.y / 1.5f);

                        Index_Buldings[size_index] = Instantiate(large, RelativePosition, transform.rotation);
                        size_index++;
                    
                }
                
            }
        }

                


    }

    public void clearBuildings()
    {
        if (Index_Buldings != null)
        {
            for (int i = 0; i < Index_Buldings.Length; i++)
            {
                //destroys all objects in scene (buildings)
                DestroyImmediate(Index_Buldings[i]);
            }

        }
    }

    
}
