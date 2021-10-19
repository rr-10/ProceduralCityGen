using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display_Map : MonoBehaviour
{

    public Renderer tex_Renderer;

    //draw 2D nosie map
    public void Drawtextures(Texture2D texture)
    {
        

        //set texture size to size of map
        tex_Renderer.sharedMaterial.mainTexture = texture;
        tex_Renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }


}
