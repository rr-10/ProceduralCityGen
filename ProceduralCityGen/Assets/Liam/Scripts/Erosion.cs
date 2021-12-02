using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erosion : MonoBehaviour
{
    // Start is called before the first frame update

    

    //Affect errosion
    [Range(0, 1)]
    public float inertia = .05f;
    public int erosionRadius = 3;
    public float sediment_amount_capicty = 4; 
    public float sediment_amount_capicty_min = .01f;
    [Range(0, 1)] 
    public float disolve_rate = .25f;
    public float deposit = .25f;
    public float evaportion_rate = .1f;

    public float gravity = 4;
    public float max_DropletLife = 30;
    public float rain_rate = 1;
    public float inital_speed = 1;
    [Range(0, 1)]
    public float erodeSpeed = .3f;
    System.Random RandomMapSeed;
    
    int TempErosionRadius;
    int currentMapSize;
    int[][] Erosion_Indicies;
    float[][] Erosion_Weights;


    public void erosion(int seed, float[]HeightMap, int Iterations, int Width)
    {

        

        if (Erosion_Indicies == null)
        {
            InitializeErosionIndex(Width, erosionRadius);
            TempErosionRadius = erosionRadius;
            currentMapSize = Width;
       }


        RandomMapSeed = new System.Random(seed);

        for (int interation = 0; interation < Iterations; interation++)
        {
            //creating a Water_amount droplet at a random place on my heightmap

            //end droplet life if on edge of HeightMap maybe?
            //Water_amount DROPLET VALUES
            float DropletX = RandomMapSeed.Next(0, Width - 1);
            float DropletY = RandomMapSeed.Next(0, Width - 1);
            float DirX =0;
            float DirY =0;
            float currentSpeed = inital_speed;
            float Water_amount = rain_rate;
            float sediment_amount = 0;

            //Simulate drolets life


            for (int life_droplet = 0; life_droplet < max_DropletLife; life_droplet++)
            {
                //convert random point on HeightMap into a grid space
                int X = (int)DropletX;
                int Y = (int)DropletY;
                int IndexDroplet = Y * Width + X;

                float OffsetX = DropletX - X;
                float OffsetY = DropletY - Y;

               

                    Gradient_Height CellGradientHeight = CalHeight_Gradient(HeightMap, Width, DropletX, DropletY);

                    //update driection and position

                    DirX = (DirX * inertia - CellGradientHeight.HgradientX * (1 - inertia));
                    DirY = (DirY * inertia - CellGradientHeight.HgradientY * (1 - inertia));

                    //normalize
                    float l = Mathf.Sqrt(DirX * DirX + DirY * DirY);
                    if (l != 0)
                    {
                        DirX /= l;
                        DirY /= l;
                    }


                    DropletX += DirX;
                    DropletY += DirY;

                    //stop if fall off HeightMap (change more origionhal)
                    if ((DirY == 0 && DirX == 0) || DropletY < 0  || DropletX < 0 || DropletX >= Width - 1 || DropletY >= Width - 1)
                    {
                        break;
                    }

                    //find new height
                    float NewHeight = CalHeight_Gradient(HeightMap, Width, DropletX, DropletY).height;
                    float DeltHeight = NewHeight - CellGradientHeight.height;

                    //calculate sediment_amount capacity
                    float sediment_amountCap = Mathf.Max(-DeltHeight * currentSpeed * Water_amount * sediment_amount_capicty, sediment_amount_capicty_min);

                    //carrying too much sediment_amount or going down
                  
                    if (sediment_amount > sediment_amountCap || DeltHeight > 0)
                    {
                        // If moving uphill (DeltHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment_amount
                        float amountToDeposit = (DeltHeight > 0) ? Mathf.Min(DeltHeight, sediment_amount) : (sediment_amount - sediment_amountCap) * deposit;
                        sediment_amount -= amountToDeposit;

                    // Add the sediment_amount to the verticies that surrond the corrent cell
                    // Deposition is not done over a Radus so that the samller pits can be filled in
                        HeightMap[IndexDroplet + 1] += amountToDeposit * OffsetX * (1 - OffsetY);
                        HeightMap[IndexDroplet + Width] += amountToDeposit * (1 - OffsetX) * OffsetY;
                        HeightMap[IndexDroplet] += amountToDeposit * (1 - OffsetX) * (1 - OffsetY);
                        HeightMap[IndexDroplet + Width + 1] += amountToDeposit * OffsetX * OffsetY;

                    }
                else 
                     
                {
                    
                    // calulated the amount that needs to errode when taking into account hiehgt and cap so that it dosent go too far down
                    float Eroded = Mathf.Min((sediment_amountCap - sediment_amount) * erodeSpeed, -DeltHeight);

                        // erode  all cells inside the erosion radius
                        for (int Point_Index = 0; Point_Index < Erosion_Indicies[IndexDroplet].Length; Point_Index++)
                        {
                        int cell_index_C = Erosion_Indicies[IndexDroplet][Point_Index];
                        float weighedErodeAmount = Eroded * Erosion_Weights[IndexDroplet][Point_Index];
                        float deltasediment_amount = (HeightMap[cell_index_C] < weighedErodeAmount) ? HeightMap[cell_index_C] : weighedErodeAmount;
                        HeightMap[cell_index_C] -= deltasediment_amount;
                        sediment_amount += deltasediment_amount;

                    }
                    }
                    
                    // Update droplet settings
                    currentSpeed = Mathf.Sqrt(currentSpeed * currentSpeed + DeltHeight * gravity);
                    Water_amount *= (1 - evaportion_rate);








                
            }


        }



    }


    Gradient_Height CalHeight_Gradient(float[] HeightMap, int Width, float x, float y)
    {
        //(0,0) on HeightMap = droplet is the centre of cell based off 0,1
        int CellX = (int)x;
        int CellY = (int)y;

        //get offset inside cell (4 verticies) on HeightMap of the droplet
        float Newx = x - CellX;
        float Newy = y - CellY;


        //Debug.Log(Width);
        //Debug.Log(Cell_Index_TopLeft);

        int Cell_Index_TopLeft = (CellY * Width) + CellX;
        //Check all the verticies / points on the heightmap based on the cell
        float heightbottomLeft = HeightMap[Cell_Index_TopLeft + Width];
        float heightbottomRight = HeightMap[Cell_Index_TopLeft + Width + 1];
        float heightTopLeft = HeightMap[Cell_Index_TopLeft];
        float heightTopRight = HeightMap[Cell_Index_TopLeft + 1];

        //Calulate the gradients
        float HgradientY = (heightbottomLeft - heightTopLeft) * (1 - Newx) + (heightbottomRight - heightTopRight) * Newx;
        float HgradientX = (heightTopRight - heightTopLeft) * (1 - Newy) + (heightbottomRight - heightbottomLeft) * Newy;
        
        float height = heightTopLeft * (1 - Newx) * (1 - Newy) + heightTopRight * Newx * (1 - Newy) + heightbottomLeft * (1 - Newx) * Newy + heightbottomRight * Newx * Newy;

        //return the height based on gradient of the cell and over values
        return new Gradient_Height() { height = height, HgradientX = HgradientX, HgradientY = HgradientY };

    }



    void InitializeErosionIndex(int mapSize, int ErosionRadus)
    {

        Erosion_Indicies = new int[mapSize * mapSize][];
        Erosion_Weights = new float[mapSize * mapSize][];

        int Index = 0;
        float[] ErosionWeights = new float[ErosionRadus * ErosionRadus * 4];
        float weightSum = 0;
        int[] xOffsets = new int[ErosionRadus * ErosionRadus * 4];
        int[] yOffsets = new int[ErosionRadus * ErosionRadus * 4];
        

        for (int i = 0; i < Erosion_Indicies.GetLength(0); i++)
        {
            int centreY = i / mapSize;
            int centreX = i % mapSize;
            

            if (centreY >= mapSize - ErosionRadus ||  centreY <= ErosionRadus || centreX >= mapSize - ErosionRadus  || centreX <= ErosionRadus + 1 )
            {
                weightSum = 0;
                Index = 0;
                for (int y = -ErosionRadus; y <= ErosionRadus; y++)
                {
                    for (int x = -ErosionRadus; x <= ErosionRadus; x++)
                    {
                        float squareDest = x * x + y * y;
                        if (squareDest < ErosionRadus * ErosionRadus)
                        {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                            {
                                //set the values
                                float weight = 1 - Mathf.Sqrt(squareDest) / ErosionRadus;
                                weightSum += weight;
                                xOffsets[Index] = x;
                                ErosionWeights[Index] = weight;
                                yOffsets[Index] = y;
                                Index++;
                            }
                        }
                    }
                }
            }

            int numEntries = Index;
            Erosion_Weights[i] = new float[numEntries];
            Erosion_Indicies[i] = new int[numEntries];
            

            for (int j = 0; j < numEntries; j++)
            {
                Erosion_Weights[i][j] = ErosionWeights[j] / weightSum;
                Erosion_Indicies[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                
            }
        }
    }


    struct Gradient_Height
    {
        public float HgradientX;
        public float HgradientY;
        public float height;
        
    }



}
