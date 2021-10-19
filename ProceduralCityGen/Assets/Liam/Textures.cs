using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textures 
{
   //take in one dimensonal colour map

    public static Texture2D TextureFromMap(Color[] map_colour, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels (map_colour);
        texture.Apply();
        return texture;


    }


    public static Texture2D textureHeightMap( float[,] Map_Heights)
    {
        //Get the height and width of the map
        int Width = Map_Heights.GetLength(0);
        int Height = Map_Heights.GetLength(1);

        //faster to loop throuhg all colors at once
        Color[] Map_Colour = new Color[Width * Height];

        //loop through Map_colour to set all the pixels to the correct colour
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Map_Colour[y * Width + x] = Color.Lerp(Color.black, Color.white, Map_Heights[x, y]);
            }
        }
        //apply the changes to the texture
        return TextureFromMap(Map_Colour, Width, Height);
    }


}
