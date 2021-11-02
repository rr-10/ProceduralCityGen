using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display_Map : MonoBehaviour
{

    public Renderer tex_Renderer;
    public MeshFilter Filter;
    public MeshRenderer Renderer;
    public MeshCollider COllider;
    //draw 2D nosie map
    public void Drawtextures(Texture2D texture)
    {
        
        //set texture size to size of map
        tex_Renderer.sharedMaterial.mainTexture = texture;
        tex_Renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh (DataMesh MeshD, Texture2D texture)
    {
        
        Filter.sharedMesh = MeshD.CreateNewMesh(); //shared material so we can render outside of game
        Renderer.sharedMaterial.mainTexture = texture;
        COllider.sharedMesh = MeshD.CreateNewMesh();







    }
    

}
