using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Generation : MonoBehaviour
{
    //Values for map generation
    public int Width;
    public int Height;
    public float Scale_Noise;

    public enum Draw_Mode {NoiseMap, ColourMap};
    public Draw_Mode DrawMap;
    public int Octaves;
    [Range(0,1)]
    public float Amplitude;
    public float Frequency;
    public bool Buildings;

    public int Seed;
    public Vector2 OffSet;

     bool Perlin_Noise = true;
     bool White_Noise = true;
     bool Cubic_Noise = true;

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
        float[,] Map_Noise = Noise_Maps.GenNoiseMap(Width, Height, Seed, Scale_Noise, Octaves, Amplitude, Frequency, OffSet, Perlin_Noise, White_Noise, Cubic_Noise);

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


        //Detect where buildsing can go
        if (Buildings == true)
        {

            for (int y = 0; y < Height - 1; y++)
            {
                for (int x = 0; x < Width - 1; x++)
                {
                    float CurrentHeight = Map_Noise[x, y];
                    //loop through heigts

                    if (CurrentHeight <= Biomes[5].height && CurrentHeight > Biomes[3].height)
                    {

                        //check if too much height diff for building sto spawn
                        float Check = CurrentHeight - Map_Noise[x + 1, y];
                        float check2 = CurrentHeight - Map_Noise[x, y + 1];
                        float Check3 = CurrentHeight - Map_Noise[x + 1, y + 1];

                        if (Check > -0.03 && Check < 0.03 && check2 > -0.03 && check2 < 0.03 && Check3 > -0.03 && Check3 < 0.03)
                        {


                            Map_Colour[y * Width + x] = Color.red;
                            Map_Colour[y * Width + x + 1] = Color.red;
                            Map_Colour[(y + 1) * Width + x] = Color.red;
                            Map_Colour[(y + 1) * Width + x + 1] = Color.red;
                        }



                    }

                    //Map_Colour[y * Width + x] = Biomes[i].colour;
                    //found biome so can move one
                    // break;
                }


            }
        }

    
        





        //Send data to display script
        Display_Map Display = FindObjectOfType<Display_Map>();

        if (DrawMap == Draw_Mode.NoiseMap)
        {
            Display.Drawtextures(Textures.textureHeightMap(Map_Noise));
        }
        else if (DrawMap == Draw_Mode.ColourMap)
            Display.Drawtextures(Textures.TextureFromMap(Map_Colour, Width, Height));
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
          
    }


}
