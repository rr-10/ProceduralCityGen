using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Vegation : MonoBehaviour
{

    public GameObject Grass;
    public GameObject Sand;
    public GameObject MainMesh;
    GameObject[] Index_Vegation;
    public void GenerateVegation(int width, int height, float[,] heightmap, int[] BuldingMap, AnimationCurve HeightCurve, float mesh_Height, int seed) //add references  for your prefabs here or watever if you want or just include them
    {


        Vector3 RelativePosition = MainMesh.transform.position;

        int size = 0;
        //array for buildings to be put into, 
        for (int i = 0; i < BuldingMap.Length; i++)
        {
            if (BuldingMap[i] == 0 )
                size++;
        }

        Index_Vegation = new GameObject[size];
        int size_index = 0;


        var Meshh2 = MainMesh.GetComponent<MeshFilter>().sharedMesh;

        Vector3[] verticies = Meshh2.normals;
        Vector3[] MeshV = Meshh2.vertices;
       // for (var i = 0; i < verticies.Length; i++)
     //   {
       //     Vector3 vertPosition = transform.TransformPoint(MeshV[i]) * 10;
        //    Vector3 vertNormal = transform.TransformDirection(verticies[i]);
         //   if (Vector3.Dot(vertNormal, Vector3.up) > 0.98)
             //   Debug.DrawRay(vertPosition, vertNormal * 10, Color.red, 10, true);
        //}





        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {


                //reset relative position
                RelativePosition.x = (-width * MainMesh.transform.localScale.x) / 2 + 5;
                RelativePosition.z = (height * MainMesh.transform.localScale.z) / 2 - 5;
   
                //algorithm for using the seed to create a "random" pattern of spawns
                System.Random Ran_Seed = new System.Random(seed);
                seed = seed + Ran_Seed.Next(seed, seed);
                seed++;
                seed = seed + Ran_Seed.Next(21, 45);

                Vector3 vertPosition = transform.TransformPoint(MeshV[y * width + x]) * 10;
                Vector3 vertNormal = transform.TransformDirection(verticies[y * width + x]);
                if (Vector3.Dot(vertNormal, Vector3.up) > 0.98 && BuldingMap[y * width + x] == 0)
                    BuldingMap[y * width + x] = 10;

                //Spawn an object in sandy area on random
                if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.3 && heightmap[x, y] < 0.45)
                {

                    float OffSet_X = Ran_Seed.Next(1, 10); //1 in 10 chance 
                    
                    if (OffSet_X <= 1) // lower == less chance
                    {
                        //Get location in world
                        RelativePosition.x += x * MainMesh.transform.localScale.x + MainMesh.transform.localScale.x / 2; 
                        RelativePosition.z -= y * MainMesh.transform.localScale.z + MainMesh.transform.localScale.z / 2; //Z is Y axis in a 3d Plane
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Sand.transform.localScale.y / 2.2f);

                        //Make sure index goes up so can store all the objects 
                        Index_Vegation[size_index] = Instantiate(Sand, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

                //Spawn a tree in grass at random
                else if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.45 && heightmap[x, y] < 0.60)
                {

                    float OffSet_X = Ran_Seed.Next(1, 1);
                    if (OffSet_X <= 1)
                    {
                        RelativePosition.x += x * MainMesh.transform.localScale.x   ;
                        RelativePosition.z -= y * MainMesh.transform.localScale.z  ;
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Grass.transform.localScale.y / 1.8f);


                        Index_Vegation[size_index] = Instantiate(Grass, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

            }

        }


        //TODO, import biomomes array and use those values instead so the is no with values




    }


    public void ClearVegation()
    {
        if (Index_Vegation != null)
        {
            for (int i = 0; i < Index_Vegation.Length; i++)
            {

                DestroyImmediate(Index_Vegation[i]);
            }

        }
    }
}
