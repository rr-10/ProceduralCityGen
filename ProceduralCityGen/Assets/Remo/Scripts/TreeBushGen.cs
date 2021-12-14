using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Title: TreeBushGen
/// Author: Remo Reji Thomas
/// Date: 28/11/2021
/// 
/// This class is asssigned to a prefab, which is spawned
/// n (number of grids) times by FoliageGridGen. This class'
/// responsiblity is to get a name of the biome (e.g. Mountain),
/// get specific models (Sicilian Fir) from the named folder in
/// resources, assign materials, rotation and spawn them in within
/// the prefab. Get's its grid x and z values from FoliageGridGen
/// </summary>
public class TreeBushGen : MonoBehaviour
{
    public float gridX;
    public float gridZ;
    public bool hasCreated = false;
    public string gridBiome;
    private Vector3 rotAngle = new Vector3(-90, 0, 0);

    public Map_Generation mapGeneration;
    public string[] terrainBiomes;

    [Header("List of Trees and Bushes")]
    public List<GameObject> oliveTreeList = new List<GameObject>();
    public List<GameObject> cypressTreeList = new List<GameObject>();
    public List<GameObject> palmTreeList = new List<GameObject>();
    public List<GameObject> pineTreeList = new List<GameObject>();
    public List<GameObject> firTreeList = new List<GameObject>();

    public List<GameObject> basilBushList = new List<GameObject>();
    public List<GameObject> juniperBushList = new List<GameObject>();
    public List<GameObject> myrtleBushList = new List<GameObject>();

    public Material[] leafMaterials;
    public Material[] branchMaterials;

    private bool isTestScene;

    private void Awake()
    {
        oliveTreeList = Resources.LoadAll<GameObject>("OliveTrees").Cast<GameObject>().ToList();
        cypressTreeList = Resources.LoadAll<GameObject>("MedCypress").Cast<GameObject>().ToList();
        palmTreeList = Resources.LoadAll<GameObject>("PalmTree").Cast<GameObject>().ToList();
        pineTreeList = Resources.LoadAll<GameObject>("StonePine").Cast<GameObject>().ToList();
        firTreeList = Resources.LoadAll<GameObject>("SicilianFir").Cast<GameObject>().ToList();

        basilBushList = Resources.LoadAll<GameObject>("BasilBush").Cast<GameObject>().ToList();
        juniperBushList = Resources.LoadAll<GameObject>("JuniperBush").Cast<GameObject>().ToList();
        myrtleBushList = Resources.LoadAll<GameObject>("MyrtleBush").Cast<GameObject>().ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        if (sceneName == "TreeTest")
        {
            isTestScene = true;
        }

        else
            isTestScene = false;
    }

    // Update is called once per frame
    void Update()
    {
        CreateGrids();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridX, 0, gridZ));
    }

    /// <summary>
    /// Creation of grids
    /// </summary>
    void CreateGrids()
    {
        for (int z = 0; z < gridZ; z++)
        {
            for (int x = 0; x < gridX; x++)
            {
                if (!hasCreated)
                {
                    SetGridType();
                    SpawnTrees();
                    hasCreated = true;
                }
            }
        }
    }

    /// <summary>
    /// Assign biome value to respective grids randomly
    /// </summary>
    void SetGridType()
    {
        // DEBUG: This code is only executed in TreeTest scene
        if (isTestScene)
        {
            var random = new System.Random();
            int rand = Random.Range(0, 8);
            string index = terrainBiomes[rand].ToString();

            switch (index) //terrainBiomes[index]
            {
                case "Deep_Water":
                    {
                        gridBiome = index;
                    }
                    break;

                case "Water":
                    {
                        gridBiome = index;
                    }
                    break;

                case "beach":
                    {
                        gridBiome = index; //index.ToString();
                    }
                    break;

                case "Land_Low":
                    {
                        gridBiome = index;
                    }
                    break;

                case "Land":
                    {
                        gridBiome = index;
                    }
                    break;

                case "Mountain_Low":
                    {
                        gridBiome = index;
                    }
                    break;

                case "Mountain":
                    {
                        gridBiome = index;
                    }
                    break;

                case "Mountain_Top":
                    {
                        gridBiome = index;
                    }
                    break;
            }
        }

        else
        {
            int rand = Random.Range(0, 8);
            string terr = Map_Generation.mapGen.Biomes[rand].name;
            print(terr);

            switch (terr)
            {
                case "Deep_Water":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Water":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "beach":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Land_Low":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Land":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Mountain_Low":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Mountain":
                    {
                        gridBiome = terr;
                    }
                    break;

                case "Mountain_Top":
                    {
                        gridBiome = terr;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// After getting all trees to each list, foreach loop it
    /// and spawn in random locations within the grid
    /// </summary>
    void SpawnTrees()
    {
        switch (gridBiome)
        {
            case "Deep_Water":
                break;

            case "Water":
                break;

            case "beach":
                {
                    // Spawn Palm Trees
                    foreach (GameObject t in palmTreeList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        print("location of tree" + go.transform.position);
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Palm Tree");
                    }
                }
                break;

            case "Land_Low":
                {
                    // Basil Bushes
                    foreach (GameObject t in basilBushList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Basil Bush");
                    }

                    // Myrtle Bushes
                    foreach (GameObject t in myrtleBushList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Myrtle Bush");
                    }
                }
                break;

            case "Land":
                {
                    // Olive Trees
                    foreach (GameObject t in oliveTreeList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Olive Tree");
                    }

                    // Mediterranean Cypress
                    foreach (GameObject t in cypressTreeList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Med Cypress");
                    }
                }
                break;

            case "Mountain_Low":
                {
                    // Juniper Bush
                    foreach (GameObject t in juniperBushList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Juniper Bush");
                    }

                    // Stone Pine
                    foreach (GameObject t in pineTreeList)
                    {
                        float spawnPointX = Random.Range(-gridX, gridX);
                        float spawnPointZ = Random.Range(-gridZ, gridZ);
                        Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                        GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                        go.transform.SetParent(gameObject.transform, false);
                        AssignMaterialsAndTags(go, "Stone Pine");
                    }
                }
                break;

            case "Mountain":
                // Sicilian Firs
                foreach (GameObject t in firTreeList)
                {
                    float spawnPointX = Random.Range(-gridX, gridX);
                    float spawnPointZ = Random.Range(-gridZ, gridZ);
                    Vector3 pos = new Vector3(spawnPointX, 0, spawnPointZ);
                    GameObject go = Instantiate(t, pos, Quaternion.Euler(rotAngle));
                    go.transform.SetParent(gameObject.transform, false);
                    AssignMaterialsAndTags(go, "Sicilian Fir");
                }
                break;

            case "Mountain_Top":
                break;
        }
    }

    /// <summary>
    /// After spawning them in, immediately set the materials associated with the trees
    /// as well as setting the Tags
    /// </summary>
    /// <param name="tree"></param>
    /// <param name="treeType"></param>
    void AssignMaterialsAndTags(GameObject tree, string treeType)
    {
        GameObject branchMat = tree.transform.GetChild(0).gameObject;

        switch (treeType)
        {
            case "Olive Tree":
                { //branch then leaf
                    tree.GetComponent<Renderer>().material = branchMaterials[0];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[0];
                    //tree.tag = "Olive Tree";
                }
                break;

            case "Palm Tree":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[1];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[1];
                }
                break;

            case "Basil Bush":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[2];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[2];
                }
                break;

            case "Myrtle Bush":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[0];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[3];
                }
                break;

            case "Med Cypress":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[0];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[4];
                }
                break;

            case "Juniper Bush":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[3];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[5];
                }
                break;

            case "Stone Pine":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[0];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[6];
                }
                break;

            case "Sicilian Fir":
                {
                    tree.GetComponent<Renderer>().material = branchMaterials[0];
                    branchMat.GetComponent<Renderer>().material = leafMaterials[7];
                }
                break;

        }
    }


}
