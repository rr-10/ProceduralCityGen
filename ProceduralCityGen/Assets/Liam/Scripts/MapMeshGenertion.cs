using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapMeshGenertion
{

    public static DataMesh GenerateMeshTerrain(float[,] HeightMap, float Height_MultiPly, AnimationCurve HeightCurve)

    {   //Get height and width of map
        int Width = HeightMap.GetLength(0);
        int height = HeightMap.GetLength(1);
        float topLeftX = (Width - 1) / -2f; // make sure float so it dosent round
        float topLeftY = (height - 1) / 2f; //used to get centre of map

        DataMesh meshD = new DataMesh(Width, height);
        int Index_Vertex = 0; // keep track of where we are in 1D array of verticies


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < Width; x++)
            {

                meshD.vertices[Index_Vertex] = new Vector3(topLeftX + x,HeightCurve.Evaluate(HeightMap[x, y]) * Height_MultiPly, topLeftY - y); //uses animation curve to determine height 
                meshD.UVData[Index_Vertex] = new Vector2(x / (float)Width, y / (float)height); // get relative position on map


                //dont have to create triangles for right and bottom edge of the map 
                if (x < Width - 1 && y < height - 1)
                { //ignoreing right and bottom edge of map
                    meshD.Add_Triangle(Index_Vertex, Index_Vertex + Width + 1, Index_Vertex + Width);// does Two triangles in a square 
                    meshD.Add_Triangle(Index_Vertex + Width + 1, Index_Vertex, Index_Vertex + 1); //Starts from the topleftmost vertex

                }
                Index_Vertex++;
            }
        }

        return meshD; //returning mesh data so threading can be achieved instead of the mesh itself
    }



}

public class DataMesh // class to sore mesh data
{
    public Vector3[] vertices;
    public int[] Triangles;
    public Vector2[] UVData;
    public Vector3[] copy;

    int Index_Triangles; // keep track of where we are in triangles array
    public DataMesh(int WidthM, int HeightM)
    {
        vertices = new Vector3[WidthM * HeightM];
        copy = new Vector3[WidthM * HeightM];
        Triangles = new int[(WidthM - 1) * (HeightM - 1) * 6];

        UVData = new Vector2[WidthM * HeightM];
    }



    public void Add_Triangle(int a, int b, int c)
    {
        Triangles[Index_Triangles] = a;
        Triangles[Index_Triangles + 1] = b;
        Triangles[Index_Triangles + 2] = c;
        Index_Triangles += 3; // 3points = completed tirangle
    }

    public Mesh CreateNewMesh()
    {
        Mesh Map_Mesh = new Mesh();
        Map_Mesh.vertices = vertices;
        Map_Mesh.triangles = Triangles;
        Map_Mesh.uv = UVData;
        Map_Mesh.RecalculateNormals(); // get normals


        return Map_Mesh;
    }


}