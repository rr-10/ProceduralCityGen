using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map_Generation : MonoBehaviour
{
    public static Map_Generation mapGen;

    //Values for map generation
    public int Width;
    public int Height;
    public float Scale_Noise;

    //Disable or enable these noise types
    public bool Perlin_Noise;
    public bool value_noise;
    public bool Simplex_noise;

    public int randomsA = 100;

    //Type of map to draw
    public enum Draw_Mode
    {
        NoiseMap,
        ColourMap,
        Mesh
    };

    public Draw_Mode DrawMap;

    //type of noise to use as the base noise map
    public enum Noise_Type
    {
        Perlin,
        Value,
        simplex
    }

    public Noise_Type Base_NoiseType;

    //Settings that affect overall outcome of terrain
    [Space(25)] public int Octaves;
    [Range(0, 1)] public float Amplitude;
    public float Frequency;
    public int Seed;
    public Vector2 OffSet; //move around map
    public float MeshHeight;
    public AnimationCurve MeshHeightCurve;


    [Space(25)]
    //enable showing bulding location and spawning them
    public bool Buildings;

    public bool BuildingsPrefabs;
    public bool Tree_prefabs;
    public bool erosion = false;
    public int Rain_iterations = 30000;
    public double FlatLand = 0.0008;

    public bool random = false;
    public bool Auto_Update;
    public bool screnshot = false;
    float[,] Map_Noise;
    Color[] Map_Colour;
    int[] bulidingMap;

    //Used in editor to create the colours of the map
    public Terrain[] Biomes;

    [System.Serializable]
    public struct Terrain //Struct for sorting colours on map
    {
        public string name;
        public float height;
        public Color colour;
    }

    public void Awake()
    {
        if (mapGen != null)
            Destroy(mapGen);
        else
            mapGen = this;
    }

    public void Generate_Map()
    {
        //Set base type of map
        int type = 1;
        if (Base_NoiseType == Noise_Type.Value)
            type = 2;
        else if (Base_NoiseType == Noise_Type.simplex)
            type = 3;

        //Generate a noise map
        Map_Noise = Noise_Maps.GenNoiseMap(Width, Height, Seed, Scale_Noise, Octaves, Amplitude, Frequency, OffSet,
            Perlin_Noise, value_noise, Simplex_noise, type);

        //colouring the map
        Map_Colour = new Color[Width * Height];


        if (random == true)
        {
            System.Random Ran_Seed = new System.Random();

            int truee = (Ran_Seed.Next(0, 2));

            if (truee == 1)
            {
                erosion = true;
            }
            else
                erosion = false;
        }

        if (erosion == true)
        {
            erode();
        }

        ColourMap();

        FindBuildings();


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
            Display.DrawMesh(MapMeshGenertion.GenerateMeshTerrain(Map_Noise, MeshHeight, MeshHeightCurve),
                Textures.TextureFromMap(Map_Colour, Width, Height));


            if (BuildingsPrefabs == true)
            {
                Buildings_gen.ClearBuildings();
                Buildings_gen.GenerateBuildings(Width, Height, Map_Noise, bulidingMap, MeshHeightCurve, MeshHeight);
            }
            else
                Buildings_gen.ClearBuildings();

            if (Tree_prefabs == true)
            {
                //veg.ClearVegation();
                // veg.GenerateVegation(Width, Height, Map_Noise, bulidingMap, MeshHeightCurve, MeshHeight, Seed);
                veg.GenerateVegation();
            }
            //else
            // veg.ClearVegation();
        }


        if (screnshot == true)
        {
            Erosion lol = FindObjectOfType<Erosion>();

            //todo fix data bugs

            //Debug.Log(Amplitude);
            var stampString = string.Format("Noises_{0}-{1:00}-{2:00}-", Perlin_Noise, value_noise, Simplex_noise);
            string sString;
            string ErodeeS = "";
            if (erosion == true)
            {
                ErodeeS = "Erosion/";
                sString = string.Format(
                    "{0}-{1:00}-{2:00}_{3:00}-{4:00}-{5:00}-{6:00}-{7:00}-{8:00}-{9:00}-{10:00}-{11:00}-{12:00}",
                    erosion, Rain_iterations, lol.inertia, lol.erosionRadius, lol.sediment_amount_capicty,
                    lol.sediment_amount_capicty_min, lol.disolve_rate, lol.deposit, lol.evaportion_rate, lol.gravity,
                    lol.max_DropletLife, lol.rain_rate, lol.inital_speed, lol.erodeSpeed);
            }
            else
                sString = string.Format("{0}", erosion);

            switch (type)
            {
                case 1:
                    ScreenCapture.CaptureScreenshot("ScreenShots/Perlin/" + ErodeeS + stampString + Amplitude + "-" +
                                                    Frequency + "- Erosion -" + sString + ".png");
                    break;
                case 2:
                    ScreenCapture.CaptureScreenshot("ScreenShots/Value/" + ErodeeS + stampString + Amplitude + "-" +
                                                    Frequency + "- Erosion -" + sString + ".png");
                    break;
                case 3:
                    ScreenCapture.CaptureScreenshot("ScreenShots/Simplex/" + ErodeeS + stampString + Amplitude + "-" +
                                                    Frequency + "- Erosion -" + sString + ".png");
                    break;
                default:
                    Debug.Log("taset");
                    break;
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

        if (Height != Width)
            Height = Width;
    }

    void erode()
    {
        float[] heightmap = new float[Width * Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                heightmap[y * Width + x] = Map_Noise[x, y];
            }
        }

        Erosion lol = FindObjectOfType<Erosion>();

        if (random == true)
        {
            System.Random Ran_Seed = new System.Random();

            lol.inertia = (float) Ran_Seed.Next(0, 100) / 100;
            lol.erosionRadius = (Ran_Seed).Next(3, 15);
            lol.sediment_amount_capicty = (float) (Ran_Seed).Next(1, 140) / 100;
            lol.sediment_amount_capicty_min = (float) (Ran_Seed).Next(0, 30) / 10;
            lol.disolve_rate = (float) (Ran_Seed).Next(0, 100) / 100;
            lol.deposit = (float) (Ran_Seed).Next(0, 30) / 10;
            lol.evaportion_rate = (float) (Ran_Seed).Next(0, 30) / 10;
            lol.gravity = (Ran_Seed).Next(1, 50);
            lol.max_DropletLife = (float) (Ran_Seed).Next(10, 50);
            lol.rain_rate = (Ran_Seed).Next(1, 10);
            lol.inital_speed = (Ran_Seed).Next(1, 10);
            lol.erodeSpeed = (float) (Ran_Seed).Next(1, 100) / 100;
        }


        lol.erosion(Seed, heightmap, Rain_iterations, Width);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Map_Noise[x, y] = heightmap[y * Width + x];
            }
        }
    }


    void ColourMap()
    {
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
    }

    void FindBuildings()
    {
        bulidingMap = new int[Width * Height];

        if (Buildings == true || BuildingsPrefabs == true)
        {
            for (int y = 0; y < Height - 3; y++)
            {
                for (int x = 0; x < Width - 3; x++)
                {
                    //loop through for building sizes
                    float CurrentHeight = Map_Noise[x, y];

                    if (CurrentHeight > Biomes[2].height && CurrentHeight < Biomes[5].height) //add max height
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
                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand &&
                                        check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
                                    {
                                        //Checking centre verticies to see if flat with out verticies
                                        float checkCenterLT = CurrentHeight - Map_Noise[x + i - 2, y + i - 1];
                                        float checkCenterLB = CurrentHeight - Map_Noise[x + i - 2, y + i - 2];
                                        float checkCenterRT = CurrentHeight - Map_Noise[x + i - 1, y + i - 1];
                                        float checkCenterRB = CurrentHeight - Map_Noise[x + i - 1, y + i - 2];

                                        if (checkCenterLT > -FlatLand && Check < checkCenterLT &&
                                            checkCenterLB > -FlatLand && checkCenterLB < FlatLand &&
                                            checkCenterRT > -FlatLand && checkCenterRT < FlatLand &&
                                            checkCenterRB > -FlatLand && checkCenterRB < FlatLand)
                                        {
                                            if (bulidingMap[(y + i) * Width + x] == 0 &&
                                                bulidingMap[(y + i) * Width + x + i] == 0 &&
                                                bulidingMap[y * Width + x + i] == 0)
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
                                    if (Check > -FlatLand && Check < FlatLand && check2 > -FlatLand &&
                                        check2 < FlatLand && Check3 > -FlatLand && Check3 < FlatLand)
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
                                            if (bulidingMap[(y + i) * Width + x] == 0 &&
                                                bulidingMap[(y + i) * Width + x + i] == 0 &&
                                                bulidingMap[y * Width + x + i] == 0 &&
                                                (CurrentHeight - Map_Noise[x + 1, y + 1]) > -FlatLand)
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
    }


    public void randomgen()
    {
        System.Random Ran_Seed = new System.Random();

        int xd = Ran_Seed.Next(1, 3);

        if (xd == 1)
            Perlin_Noise = true;
        else
            Perlin_Noise = false;

        xd = Ran_Seed.Next(3, 5);

        if (xd == 3)
            value_noise = true;
        else
            value_noise = false;

        xd = Ran_Seed.Next(4, 6);

        if (xd == 4)
            Simplex_noise = true;
        else
            Simplex_noise = false;


        xd = Ran_Seed.Next(1, 4);
        Rain_iterations = (Ran_Seed).Next(20000, 65000);
        switch (xd)
        {
            case 1:
                Base_NoiseType = Noise_Type.Perlin;

                break;

            case 2:
                Base_NoiseType = Noise_Type.simplex;

                break;

            case 3:
                Base_NoiseType = Noise_Type.Value;
                break;
            default:
                break;
        }

        Amplitude = (float) Ran_Seed.Next(15, 80) / 100;
        Frequency = (float) Ran_Seed.Next(5, 25) / 10;

        Generate_Map();
    }
}