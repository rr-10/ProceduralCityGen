using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise_Maps
{
    //Generate noise map function
    public static float [,] GenNoiseMap(int Width, int Height,int Seed, float scale, int octaves, float Persistance, float lacunarit, Vector2 OffSet,bool Perlin, bool white, bool cubic)
    {
        //Create two sided array to store noise map,
        float[,] Noise_Map = new float[Width, Height];

        //Randomly generate seed for each octave
        System.Random Ran_Seed = new System.Random(Seed);
        Vector2[] Octave_OffSets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            //Dont make range too big, will return same number
            float OffSet_X = Ran_Seed.Next(-100000, 100000 ) + OffSet.x;
            float OffSet_Y = Ran_Seed.Next(-100000, 100000 ) + OffSet.y;
            Octave_OffSets[i] = new Vector2(OffSet_X, OffSet_Y);
        }

        FastNoise Noise_Generator = new FastNoise();
        
        //float testing = testt.GetWhiteNoise(Width,Height,0,1);
        //Debug.Log (testing);
        //Make sure scale is never dividing by 0
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        
        //to deteremine range
        float Max_Height = float.MinValue;
        float min_Height = float.MaxValue;


        //To make Noise_Scale zoom into center

        float Width_Half = Width /2f;
        float Height_Half = Height / 2f;

        //1 = perling, 2 = white, 3 = cubic



        //Debug.Log(lacunarit);
        
        //loop through noise map
        for (int y = 0; y < Height; y++){
           for (int x = 0; x < Width; x++)
            {
                float amp = 1;
                float freq = 1;
                float Noise_Height = 0;

                int Noise_Type = 1;

                //Loop through number of octaves / noisemaps
                for (int i = 0; i < octaves; i++)
                {

                    //Higher frequency = Further apart the sample points
                    //Height values will change more rapidly 
                    float SampX = (x-Width_Half) / scale * freq + Octave_OffSets[i].x;
                    float SampY = (y-Height_Half) / scale * freq + Octave_OffSets[i].y;



                    //generate noise map based on scale values
                    //can go from -1 to 1
                    //select type of noise from switch

                    float Perlin_Noise;
                    
                    switch (Noise_Type)
                    {
                        case 1:
                            Perlin_Noise = Mathf.PerlinNoise(SampX, SampY) * 2 - 1;
                            break;

                        case 2: 
                            Perlin_Noise = Noise_Generator.GetValue(SampX, SampY) * 2 - 1;
                            break;

                        case 3:
                            Perlin_Noise = Noise_Generator.GetSimplex(SampX, SampY) * 2 - 1;
                            break;

                        default:
                            Perlin_Noise = Mathf.PerlinNoise(SampX, SampY) * 2 - 1;
                            break;

                    }
                    

                   // float Perlin_Noise = Mathf.PerlinNoise(SampX, SampY) * 2 - 1;
                    Noise_Height += Perlin_Noise * amp;

                    //Amplitude decrerases each octave      //Persistance should be less than one 
                    //Frequency increases each octave       //Lacunarity should be more than 1
                    amp *= Persistance;
                    freq *= lacunarit;


                   Noise_Type += 1;

                    if (Noise_Type > 3)
                        Noise_Type = 1;





                }


                //Get range of height values
                if(Noise_Height > Max_Height)
                {
                    Max_Height = Noise_Height;
                }
                else if (Noise_Height < min_Height)
                {
                    min_Height = Noise_Height;
                }
                //Apply height to map
                Noise_Map[x, y] = Noise_Height;
            }

        }

        //loop through noise maps again 
        //Nomalize noise map
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                //returns value inbetween 0 and 1 E.G: if x,y == max height it return 1, min height == 0 and inbetween = 0.5
                
                Noise_Map[x, y] = Mathf.InverseLerp(min_Height, Max_Height, Noise_Map[x, y]);
            }
        }

        return Noise_Map;
    }

}

