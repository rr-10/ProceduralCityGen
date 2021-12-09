using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

/// <summary>
/// Should be called something else then call it FoliageGridGen
/// </summary>

public class FoliageGridGen : MonoBehaviour
{
    public static FoliageGridGen gridGen;

    private Vector3 rotAngle = new Vector3(-90, 0, 0);
    //private Vector3 spawnPoint;
    //[SerializeField] private GameObject mesh;
    [SerializeField] private GameObject[] trees;
    
    //public GameObject tree;
    public GameObject terrain;
    public Mesh terrainMesh;

    //[SerializeField] private List<GameObject> notTrees = new List<GameObject>();
    [SerializeField] private List<GameObject> grids = new List<GameObject>();

    public float gridX;
    public float gridZ;
    public float total;
    public float rootTotal;
    public float spacing = 2f;
    public GameObject gridPrefab;

    private bool isTestScene;

    public void Awake()
    {
        if (gridGen != null)
            Destroy(gridGen);
        else
            gridGen = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        terrainMesh = terrain.GetComponent<MeshFilter>().sharedMesh;
        float objectSizeX = terrainMesh.bounds.size.x;
        float objectSizeZ = terrainMesh.bounds.size.z;
        float objectScaleX = terrain.transform.localScale.x;
        float objectScaleZ = terrain.transform.localScale.z;
        print("terrain mesh d - X: " + objectSizeX + ", Z: " + objectSizeZ);
        print("terrain scale x: " + objectScaleX + ", z: " + objectScaleZ);

        gridX = objectSizeX * objectScaleX;
        gridZ = objectSizeZ * objectScaleZ;
        print("gridX: " + gridX + ", gridZ: " + gridZ);
        //print("Current dimensions of grid - X: " + gridX + ", Z: " + gridZ);

        Scene currentScene = SceneManager.GetActiveScene();

        string sceneName = currentScene.name;

        if (sceneName == "TreeTest")
        {
            isTestScene = true;
        }

        else
            isTestScene = false;

        //print("Is Test Scene? " + isTestScene);
    }

    // Update is called once per frame
    void Update()
    {
        CreateGrids();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridX, 0, gridZ));
    }

    /// <summary>
    /// Take the length and width of the mesh and create grids (10 by 10) grids
    /// </summary>
    void CreateGrids()
    {
        //total = 25;
        rootTotal = Mathf.Sqrt(total);
        //print("Root Total: " + rootTotal);
        spacing = gridX / rootTotal;
        //print("Spacing: " + spacing);

        var startPosX = spacing / 100;
        var startPosZ = spacing / 100;

        print("startX: " + startPosX);

        // Start with one grid
        for (int z = 0; z < rootTotal; z++)
        {
            for (int x = 0; x < rootTotal; x++)
            {
                bool hasSpawned = true;

                if(hasSpawned)
                {
                    if (grids.Count < total)
                    {
                        Vector3 pos = new Vector3(2 - x, 0, 2 - z) * spacing; // Terrain
                        //Vector3 pos = new Vector3(startPosX - x, 0, startPosZ - z) * spacing;// testplane
                        //print("x:" + x + ", z:" + z);
                        GameObject go = Instantiate(gridPrefab, pos, Quaternion.identity);
                        float sizeOfSubgridX = gridX / rootTotal;
                        float sizeOfSubgridZ = gridZ / rootTotal;
                        go.GetComponent<TreeBushGen>().gridX = sizeOfSubgridX;
                        go.GetComponent<TreeBushGen>().gridZ = sizeOfSubgridZ;
                        go.transform.SetParent(gameObject.transform, false);
                        grids.Add(go);
                    }

                    else
                        hasSpawned = !hasSpawned;
                }


            }
        }
    }
}


