using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Generation : MonoBehaviour
{
    //Values for map generation
    public int Width;
    public int Height;
    public float Scale_Noise;

    //Disable or enable these noise types
    public bool Perlin_Noise;
    public bool value_noise;
    public bool Simplex_noise;

    //Type of map to dawy
    public enum Draw_Mode { NoiseMap, ColourMap, Mesh };
    public Draw_Mode DrawMap;

    //Type of Noise to use as base
    public enum Noise_Type { Perlin, Value, simplex }
    public Noise_Type NoiseType;

    //Settings that affect overall outcome of terrain
    public int Octaves;
    [Range(0, 1)]
    public float Amplitude;
    public float Frequency;
    public int Seed;
    public Vector2 OffSet;
    public float MeshHeight;
    public AnimationCurve MeshHeightCurve;

    //enable showing bulding location and spawning them
    public bool Buildings;
    public bool BuildingsPrefabs;


    //TODO add vegation option

    //value used to check how flat land needs to be to have a building
    public double FlatLand = 0.0050;



    //Enable updating of terrain in editor 
    public bool Auto_Update;

    //Used in editor to create the colours of the map
    public Terrain[] Biomes;
    [System.Serializable]
    public struct Terrain //Struct for sorting colours on map
    {
        public string name;
        public float height;
        public Color colour;
    }




    public void Generate_Map()
    {

        //Set base type of map
        int type = 1;
        if (NoiseType == Noise_Type.Value)
            type = 2;
        else if (NoiseType == Noise_Type.simplex)
            type = 3;

        //Generate a noise map
        float[,] Map_Noise = Noise_Maps.GenNoiseMap(Width, Height, Seed, Scale_Noise, Octaves, Amplitude, Frequency, OffSet, Perlin_Noise, value_noise, Simplex_noise, type);

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


        //Stores where buildings are located on the map
        int[] bulidingMap = new int[Width * Height];

        if (Buildings == true)
        {
            for (int y = 0; y < Height - 3; y++)
            {
                for (int x = 0; x < Width - 3; x++)
                {
                    //loop through for building sizes
                    float CurrentHeight = Map_Noise[x, y];

                    if (CurrentHeight > Biomes[3].height && CurrentHeight < Biomes[6].height) //add max height
                    {




                        for (int i = 3; i > 0; i--)
                        {
                            if (bulidingMap[y * Width + x] == 0)
                            {
                                float Check = CurrentHeight - Map_Noise[x + i, y];
                                float check2 = CurrentHeight - Map_Noise[x, y + i];
                                float Check3 = CurrentHeight - Map_Noise[x + i, y + i];


                                //Check if a the verticies can hold  large building
                                if (i > 2)
                                {
                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand && check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
                                    {


                                        //Checking centre verticies to see if flat with out verticies
                                        float checkCenterLT = CurrentHeight - Map_Noise[x + i - 2, y + i - 1];
                                        float checkCenterLB = CurrentHeight - Map_Noise[x + i - 2, y + i - 2];
                                        float checkCenterRT = CurrentHeight - Map_Noise[x + i - 1, y + i - 1];
                                        float checkCenterRB = CurrentHeight - Map_Noise[x + i - 1, y + i - 2];

                                        if (checkCenterLT > -FlatLand && Check < checkCenterLT && checkCenterLB > -FlatLand && checkCenterLB < FlatLand && checkCenterRT > -FlatLand && checkCenterRT < FlatLand && checkCenterRB > -FlatLand && checkCenterRB < FlatLand)
                                        {
                                            if (bulidingMap[(y + i) * Width + x] == 0 && bulidingMap[(y + i) * Width + x + i] == 0 && bulidingMap[y * Width + x + i] == 0)
                                            {
                                                //loop through the verticies and set the colour to black
                                                for (int j = 0; j < 3; j++)
                                                {
                                                    Map_Colour[(y + j) * Width + x] = Color.black;
                                                    Map_Colour[(y + j) * Width + x + 1] = Color.black;
                                                    Map_Colour[(y + j) * Width + x + 2] = Color.black;

                                                }

                                                //loop trhough all the height points and set them to occupied by a building
                                                Map_Colour[(y) * Width + x] = Color.grey;
                                                for (int j = 0; j < 4; j++)
                                                {
                                                    bulidingMap[(y + j) * Width + x] = 1;
                                                    bulidingMap[(y + j) * Width + x + 1] = 1;
                                                    bulidingMap[(y + j) * Width + x + 2] = 1;
                                                    bulidingMap[(y + j) * Width + x + 3] = 1;

                                                    Map_Noise[x, y + j] = Map_Noise[x, y];
                                                    Map_Noise[x + 1, y + j] = Map_Noise[x, y];
                                                    Map_Noise[x + 2, y + j] = Map_Noise[x, y];
                                                    Map_Noise[x + 3, y + j] = Map_Noise[x, y];

                                                }


                                                //Designate building type 4 == large
                                                bulidingMap[y * Width + x] = 4;
                                            }

                                        }

                                    }


                                }

                                else
                                {

                                    //SMALL and Medium buildings
                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand && check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
                                    {
                                        //if area for small buildind
                                        if (i == 1)
                                        {
                                            Map_Colour[y * Width + x] = Color.yellow;
                                            bulidingMap[y * Width + x] = 2;
                                            bulidingMap[(y + 1) * Width + x + 1] = 1;
                                            bulidingMap[(y + 1) * Width + x] = 1;
                                            bulidingMap[y * Width + x + 1] = 1;

                                            Map_Noise[x + 1, y + 1] = Map_Noise[x, y];
                                            Map_Noise[x + 1, y] = Map_Noise[x, y];
                                            Map_Noise[x, y + 1] = Map_Noise[x, y];
                                        }


                                        //if area for medium building
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

                                                    Map_Noise[x, y + j] = Map_Noise[x, y];
                                                    Map_Noise[x + 1, y + j] = Map_Noise[x, y];
                                                    Map_Noise[x + 2, y + j] = Map_Noise[x, y];


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










        //Create references to scripts that generate buildings, map and vegation
        Display_Map Display = FindObjectOfType<Display_Map>();
        Building_Generator Buildings_gen = FindObjectOfType<Building_Generator>();
        Vegation veg = FindObjectOfType<Vegation>();

        //Just noise map
        if (DrawMap == Draw_Mode.NoiseMap)
        {
            Display.Drawtextures(Textures.textureHeightMap(Map_Noise));
        }

        //just colour map
        else if (DrawMap == Draw_Mode.ColourMap)
            Display.Drawtextures(Textures.TextureFromMap(Map_Colour, Width, Height));


        //Mesh with possible buildings and vegation
        else if (DrawMap == Draw_Mode.Mesh)
        {
            //Display script activates creation of mesh
            Display.DrawMesh(MapMeshGenertion.GenerateMeshTerrain(Map_Noise, MeshHeight, MeshHeightCurve), Textures.TextureFromMap(Map_Colour, Width, Height));


            if (BuildingsPrefabs == true)
            {
                Buildings_gen.ClearBuildings();
                Buildings_gen.GenerateBuildings(Width, Height, Map_Noise, bulidingMap, MeshHeightCurve, MeshHeight);
                veg.ClearVegation();
                veg.GenerateVegation(Width, Height, Map_Noise, bulidingMap, MeshHeightCurve, MeshHeight, Seed);

            }

            else
            {
                Buildings_gen.ClearBuildings();
                veg.ClearVegation();
            }
        }


    }


    //make sure editor values are valide
    private void OnValidate()
    {
        //Make sure editor values are not invalid or simulation breaking
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

        if (Octaves > 20)
            Octaves = 20;

    }


}
