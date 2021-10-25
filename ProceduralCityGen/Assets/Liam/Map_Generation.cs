using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Generation : MonoBehaviour
{
    //Values for map generation
    public int Width;
    public int Height;
    public float Scale_Noise;

    public bool Perlin_Noise;
    public bool value_noise;
    public bool Simplex_noise;

    public enum Draw_Mode {NoiseMap, ColourMap, Mesh};
    public Draw_Mode DrawMap;
    public enum Noise_Type {Perlin, Value, simplex}
    public Noise_Type NoiseType;

    public int Octaves;
    [Range(0,1)]
    public float Amplitude;
    public float Frequency;
    public bool Buildings;

    public int Seed;
    public Vector2 OffSet;

    public float MeshHeight;
    public AnimationCurve MeshHeightCurve;

    double FlatLand = 0.01;




    public bool Auto_Update;

    public Terrain[] Biomes;

    //Struct for sorting colours on map
    [System.Serializable]
    public struct Terrain
    {
        public string name;
        public float height;
        public Color colour;
    }




    public void Generate_Map()
    {
        int type = 1;

        if (NoiseType == Noise_Type.Value)
            type = 2;
        else if (NoiseType == Noise_Type.simplex)
            type = 3;




        float[,] Map_Noise = Noise_Maps.GenNoiseMap(Width, Height, Seed, Scale_Noise, Octaves, Amplitude, Frequency, OffSet, Perlin_Noise, value_noise, Simplex_noise,  type);

        //colouring the map
        Color[] Map_Colour = new Color[Width * Height];
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float CurrentHeight = Map_Noise[x, y];
                //loop through each biome to see what this current height falls within
                for (int i = 0; i < Biomes.Length; i++)
                {
                    if (CurrentHeight <= Biomes[i].height)
                    {
                        Map_Colour[y * Width + x] = Biomes[i].colour;
                        //found biome so can move one
                        break;
                    }
                }

            }
        }
        int lol = 0;
        bool once = false;
        int[] bulidingMap = new int[Width * Height];
        //Detect where buildsing can go

        if (Buildings == true)
        {

            

            for (int y = 0; y < Height - 3; y++)
            {
                for (int x = 0; x < Width - 3; x++)
                {
                    //loop through for building sizes
                    float CurrentHeight = Map_Noise[x, y];
                    if (CurrentHeight > Biomes[3].height)
                    {

                       


                            for (int i = 3; i > 0; i--)
                            {
                            if (bulidingMap[y * Width + x] == 0)
                            {
                                float Check = CurrentHeight - Map_Noise[x + i, y];
                                float check2 = CurrentHeight - Map_Noise[x, y + i];
                                float Check3 = CurrentHeight - Map_Noise[x + i, y + i];


                                //Large buildings
                                if (i > 2)
                                {
                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand && check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
                                    {

                                        float checkCenterLT = CurrentHeight - Map_Noise[x + i - 2, y + i - 1];
                                        float checkCenterLB = CurrentHeight - Map_Noise[x + i - 2, y + i - 2];
                                        float checkCenterRT = CurrentHeight - Map_Noise[x + i - 1, y + i - 1];
                                        float checkCenterRB = CurrentHeight - Map_Noise[x + i - 1, y + i - 2];

                                        if (checkCenterLT > -FlatLand && Check < checkCenterLT && checkCenterLB > -FlatLand && checkCenterLB < FlatLand && checkCenterRT > -FlatLand && checkCenterRT < FlatLand && checkCenterRB > -FlatLand && checkCenterRB < FlatLand)
                                        {
                                            if (bulidingMap[(y + i) * Width + x] == 0 && bulidingMap[(y + i) * Width + x + i] == 0 && bulidingMap[y  * Width + x + i] == 0) { 

                                            for (int j = 0; j < 3; j++)
                                            {
                                                Map_Colour[(y + j) * Width + x] = Color.black;
                                                Map_Colour[(y + j) * Width + x + 1] = Color.black;
                                                Map_Colour[(y + j) * Width + x + 2] = Color.black;

                                            }
                                                Map_Colour[(y ) * Width + x] = Color.grey;
                                            for (int j = 0; j < 4; j++)
                                            {
                                                bulidingMap[(y + j) * Width + x] = 1;
                                                bulidingMap[(y + j) * Width + x + 1] = 1;
                                                bulidingMap[(y + j) * Width + x + 2] = 1;
                                                bulidingMap[(y + j) * Width + x + 3] = 1;
                                                
                                            }

                                            bulidingMap[y  * Width + x] = 4;
                                            }

                                        }
                                        //medium buildings



                                    }


                                }

                                else
                                {


                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand && check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
                                    {
                                        if (i == 1)
                                        {
                                            Map_Colour[y * Width + x] = Color.yellow;
                                            bulidingMap[y * Width + x ] = 2;
                                        }

                                        else
                                        {

                                            if (bulidingMap[(y + i) * Width + x] == 0 && bulidingMap[(y + i) * Width + x + i] == 0 && bulidingMap[y * Width + x + i] == 0 && (CurrentHeight - Map_Noise[x + 1, y + 1]) > -FlatLand)
                                            {

                                                Map_Colour[y * Width + x] = Color.gray;
                                                Map_Colour[y * Width + x + 1] = Color.red;
                                                Map_Colour[(y + 1) * Width + x] = Color.red;
                                                Map_Colour[(y + 1) * Width + x + 1] = Color.red;


                                                for (int j = 0; j < 3; j++)
                                                {
                                                    bulidingMap[(y + j) * Width + x] = 1;
                                                    bulidingMap[(y + j) * Width + x + 1] = 1;
                                                    bulidingMap[(y + j) * Width + x + 2] = 1;

                                                    if (once == false)
                                                    {
                                                        once = true;
                                                         lol = y * Width + x;
                                                    }


                                                }

                                                bulidingMap[y * Width + x] = 3;
                                                
                                            }
                                        }
                                    }


                                }


                            }
                        }


                    }


                }
            }
        }







        Map_Colour[0] = Color.magenta;
        //Send data to display script
        Display_Map Display = FindObjectOfType<Display_Map>();

        if (DrawMap == Draw_Mode.NoiseMap)
        {
            Display.Drawtextures(Textures.textureHeightMap(Map_Noise));
        }
        else if (DrawMap == Draw_Mode.ColourMap)
            Display.Drawtextures(Textures.TextureFromMap(Map_Colour, Width, Height));
        else if (DrawMap == Draw_Mode.Mesh)
        {
            Display.DrawMesh(MapMeshGenertion.GenerateMeshTerrain(Map_Noise, MeshHeight, MeshHeightCurve), Textures.TextureFromMap(Map_Colour, Width, Height), lol);
            Debug.Log("here");
        }
        //house map
        
    }


    //make sure editor values are valide
    private void OnValidate()
    {
        if (Width < 1)
            Width = 1;

        if (Height < 1)
            Height = 1;

        if (Frequency < 1)
            Frequency = 1;

        if (Octaves < 1)
            Octaves = 1;

        if (Perlin_Noise == false && Simplex_noise == false && value_noise == false)
            Perlin_Noise = true;
          
    }


}
