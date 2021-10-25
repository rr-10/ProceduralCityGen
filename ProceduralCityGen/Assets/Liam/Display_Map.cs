using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display_Map : MonoBehaviour
{

    public Renderer tex_Renderer;
    public MeshFilter Filter;
    public MeshRenderer Renderer;
    public GameObject cube;
    public GameObject Meshh;
    //draw 2D nosie map
    public void Drawtextures(Texture2D texture)
    {
        

        //set texture size to size of map
        tex_Renderer.sharedMaterial.mainTexture = texture;
        tex_Renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh (DataMesh MeshD, Texture2D texture, int lol)
    {
        
        Filter.sharedMesh = MeshD.CreateNewMesh(); //shared material so we can render outside of game
        
        Vector3 RelativePosition = Meshh.transform.position;
        RelativePosition.x -= 745;
        RelativePosition.z += 745;
        RelativePosition.y += 1;
        
        Instantiate(cube, RelativePosition, transform.rotation);
        Debug.Log(RelativePosition);

        //now simply write code to convert position of the map things to spawn objects


        Renderer.sharedMaterial.mainTexture = texture;

    }


}
