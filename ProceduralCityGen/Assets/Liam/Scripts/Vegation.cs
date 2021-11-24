using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

#region Old Code
/*
public class Vegation : MonoBehaviour
{

    public GameObject Grass;
    public GameObject Sand;
    public GameObject MainMesh;
    GameObject[] Index_Vegation;
    public void GenerateVegation(int width, int height, float[,] heightmap, int[] BuldingMap, AnimationCurve HeightCurve, float mesh_Height, int seed) //add references  for your prefabs here or watever if you want or just include them
    {


        Vector3 RelativePosition = MainMesh.transform.position;

        int size = 0;
        //array for buildings to be put into, 
        for (int i = 0; i < BuldingMap.Length; i++)
        {
            if (BuldingMap[i] == 0 )
                size++;
        }

        Index_Vegation = new GameObject[size];
        int size_index = 0;


        var Meshh2 = MainMesh.GetComponent<MeshFilter>().sharedMesh;

        Vector3[] verticies = Meshh2.normals;
        Vector3[] MeshV = Meshh2.vertices;
       // for (var i = 0; i < verticies.Length; i++)
     //   {
       //     Vector3 vertPosition = transform.TransformPoint(MeshV[i]) * 10;
        //    Vector3 vertNormal = transform.TransformDirection(verticies[i]);
         //   if (Vector3.Dot(vertNormal, Vector3.up) > 0.98)
             //   Debug.DrawRay(vertPosition, vertNormal * 10, Color.red, 10, true);
        //}





        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {


                //reset relative position
                RelativePosition.x = (-width * MainMesh.transform.localScale.x) / 2 + 5;
                RelativePosition.z = (height * MainMesh.transform.localScale.z) / 2 - 5;
   
                //algorithm for using the seed to create a "random" pattern of spawns
                System.Random Ran_Seed = new System.Random(seed);
                seed = seed + Ran_Seed.Next(seed, seed);
                seed++;
                seed = seed + Ran_Seed.Next(21, 45);

                Vector3 vertPosition = transform.TransformPoint(MeshV[y * width + x]) * 10;
                Vector3 vertNormal = transform.TransformDirection(verticies[y * width + x]);
                if (Vector3.Dot(vertNormal, Vector3.up) > 0.98 && BuldingMap[y * width + x] == 0)
                    BuldingMap[y * width + x] = 10;

                //Spawn an object in sandy area on random
                if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.3 && heightmap[x, y] < 0.45)
                {

                    float OffSet_X = Ran_Seed.Next(1, 10); //1 in 10 chance 
                    
                    if (OffSet_X <= 1) // lower == less chance
                    {
                        //Get location in world
                        RelativePosition.x += x * MainMesh.transform.localScale.x + MainMesh.transform.localScale.x / 2; 
                        RelativePosition.z -= y * MainMesh.transform.localScale.z + MainMesh.transform.localScale.z / 2; //Z is Y axis in a 3d Plane
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Sand.transform.localScale.y / 2.2f);

                        //Make sure index goes up so can store all the objects 
                        Index_Vegation[size_index] = Instantiate(Sand, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

                //Spawn a tree in grass at random
                else if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.45 && heightmap[x, y] < 0.60)
                {

                    float OffSet_X = Ran_Seed.Next(1, 1);
                    if (OffSet_X <= 1)
                    {
                        RelativePosition.x += x * MainMesh.transform.localScale.x   ;
                        RelativePosition.z -= y * MainMesh.transform.localScale.z  ;
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Grass.transform.localScale.y / 1.8f);


                        Index_Vegation[size_index] = Instantiate(Grass, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

            }

        }


        //TODO, import biomomes array and use those values instead so the is no with values




    }


    public void ClearVegation()
    {
        if (Index_Vegation != null)
        {
            for (int i = 0; i < Index_Vegation.Length; i++)
            {

                DestroyImmediate(Index_Vegation[i]);
            }

        }
    }
}
*/
#endregion

/*
 * OutputCmd, App, ImportManager
 * EditorWindowParameters and WindowManager
 * are of Blendity file's (Anas Einea)
 * 
 * TODO: 
 * Correctly pass parameters in (somewhat successfull)
 * Add more trees and shrubs in the selection (almost)
 * Do a call where on start of project, the presets and python code is added to the respective folders
 * Add Git LFS to the project for Blendity to work (done)
 * Integrate the code into the main project (doing right now)
 * Change the whole editor window as part of Vegatation (doing atm)
 * Generation code depending on biomes value
 * Change the scaling of trees to be more appropriate
 * Pull material from project to the generated trees (USe Resources.Load)
 * Test cases on it with the environment like modifiable values
 * Give ranged values for genrating trees e.g. treeLength (3-7 in height for this biome
 * Finish
 */

// Class made to contain values if command output
public class OutputCmd
{
    public string resultStr, cmdOutputFile, errorStr = "";
    public override string ToString() => resultStr;
    public void Print()
    {
        Debug.Log(resultStr);
        if (errorStr.Length > 0)
        {
            Debug.LogError(errorStr);
        }
    }
}

// Loads up Blender instance path and functions for running commands
public class App
{
    private static string BlenderApp
    {
        get
        {
            return $@"""{ImportManager.GetPathOfPackage()}\blender~\blender.exe""";
        }
    }

    public static OutputCmd RunCmd(string cmd, Dictionary<string, string> envelope = null, string nameOfApp = null, bool isThreaded = false)
    {
        if (nameOfApp == null)
        {
            nameOfApp = BlenderApp;
        }

        //if (!isThreaded)
        //{
        //    EditorUtility.DisplayProgressBar("Executing The Command", cmd, .25f);
        //}

        ProcessStartInfo processStartInfo = new ProcessStartInfo(nameOfApp, cmd)
        {
            WorkingDirectory = ImportManager.GetPathOfPackage(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (envelope != null)
        {
            foreach (var variable in envelope)
            {
                processStartInfo.EnvironmentVariables[variable.Key] = variable.Value;
            }
        }

        Process process = new Process
        {
            StartInfo = processStartInfo
        };

        try
        {
            process.Start();
            process.WaitForExit();
        }

        catch (Exception e)
        {
            Debug.LogException(e);
        }

        //if (!isThreaded)
        //{
        //    EditorUtility.ClearProgressBar();
        //}

        string resultStr = process.StandardOutput.ReadToEnd();
        string errorStr = process.StandardError.ReadToEnd();

        OutputCmd outputCmd = new OutputCmd
        {
            cmdOutputFile = processStartInfo.EnvironmentVariables["output"],
            resultStr = resultStr,
            errorStr = errorStr
        };

        return outputCmd;
    }

    public static List<OutputCmd> RunCmdOnSelected(string cmd, Func<string, int, Dictionary<string, string>> envelopeCreator = null, string nameOfApp = null)
    {
        List<OutputCmd> outputCmd = new List<OutputCmd>();
        List<string> selectedNameOfFiles = ImportManager.GetValidImports();
        List<Task<OutputCmd>> tasks = new List<Task<OutputCmd>>();

        if (nameOfApp == null)
        {
            nameOfApp = BlenderApp;
        }

        for (int i = 0; i < selectedNameOfFiles.Count; i++)
        {
            string fileName = selectedNameOfFiles[i];
            fileName = ImportManager.GetWindowsPath(fileName);

            tasks.Add(Task.Run(() => RunCmd(cmd, envelopeCreator == null ? null : envelopeCreator(fileName, i * 10), nameOfApp, true)));
        }

        Task.WaitAll(tasks.ToArray());
        tasks.ForEach(t => outputCmd.Add(t.Result));
        return outputCmd;
    }

    public static List<OutputCmd> RunCmdTimesN(string cmd, int n, Func<string, int, Dictionary<string, string>> envelopeCreator = null, string nameOfApp = null)
    {
        List<OutputCmd> outputCmd = new List<OutputCmd>();
        List<Task<OutputCmd>> tasks = new List<Task<OutputCmd>>();
        string path = ImportManager.GetNameOfFile();
        path = ImportManager.GetWindowsPath(path);

        if (nameOfApp == null)
        {
            nameOfApp = BlenderApp;
        }

        for (int i = 0; i < n; i++)
        {
            tasks.Add(Task.Run(() => RunCmd(cmd, envelopeCreator == null ? null : envelopeCreator(path, i * 10), nameOfApp, true)));
        }

        Task.WaitAll(tasks.ToArray());
        tasks.ForEach(t => outputCmd.Add(t.Result));
        return outputCmd;
    }
}

public class ImportManager
{
    static readonly string[] acceptedExt = new string[] { ".fbx", ".obj", ".x3d", "gltf" };
    static readonly Func<string, bool> IsValidExtension = fileName => acceptedExt.Any((extension) => fileName.EndsWith(extension));
    public static string GetPathOfPackage() => Path.GetFullPath("Packages/com.ae.blendity");
    public static string GetNameOfFile() => Path.GetFullPath(AssetDatabase.GetAssetPath(Selection.activeInstanceID));
    private static string[] GetNamesOfSelectedFiles() => Array.ConvertAll(Selection.objects, obj => System.IO.Path.GetFullPath(AssetDatabase.GetAssetPath(obj.GetInstanceID())));
    public static bool IsValidImports()
    {
        string[] nameOfFiles = GetNamesOfSelectedFiles();
        return nameOfFiles.Any(IsValidExtension);
    }

    public static List<string> GetValidImports()
    {
        string[] nameOfFiles = GetNamesOfSelectedFiles();
        return nameOfFiles.Aggregate(new List<string>(), (a, b) => { if (IsValidExtension(b)) a.Add(b); return a; });
    }

    public static string GetWindowsPath(string path, string suffix = "")
    {
        string[] pathPieces = path.Split('/');

        string[] fileNamePieces = pathPieces.Last().Split('.');
        string extension = "";

        if (fileNamePieces.Length > 1)
        {
            extension = fileNamePieces.Last();
            pathPieces[pathPieces.Length - 1] = string.Join(".", fileNamePieces.Take(fileNamePieces.Length - 1));
        }

        string outputStr = $@"{string.Join(@"\", pathPieces)}{suffix}" + (extension.Length > 0 ? $".{extension}" : "");
        return outputStr;
    }

    // https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Prefabs/PrefabUtility.cs#L131
    public static void ExtractAssetMaterials(AssetImporter importer, string destPath)
    {
        var assetsToReload = new HashSet<string>();
        var materials = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath).Where(x => x.GetType() == typeof(Material)).ToArray();

        foreach (var material in materials)
        {
            var newAssetPath = destPath + "/" + material.name + ".mat";
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

            var error = AssetDatabase.ExtractAsset(material, newAssetPath);

            if (string.IsNullOrEmpty(error))
            {
                assetsToReload.Add(importer.assetPath);
            }
        }

        foreach (var path in assetsToReload)
        {
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    public static string GetPathOfImporter(string assetPath)
    {
        assetPath = assetPath.Replace("\\", "/");

        if (assetPath.StartsWith(Application.dataPath))
        {
            assetPath = "Assets" + assetPath.Substring(Application.dataPath.Length);
        }

        return assetPath;
    }

    public static void ExtractTexAndMat(string assetPath)
    {
        assetPath = GetPathOfImporter(assetPath);

        AssetImporter importAsset = AssetImporter.GetAtPath(assetPath);
        ModelImporter importModel = importAsset as ModelImporter;
        string[] cmdOutputFilePieces = assetPath.Split('/');
        string outputDir = string.Join("/", cmdOutputFilePieces.Take(cmdOutputFilePieces.Length - 1));

        importModel.ExtractTextures(outputDir);
        AssetDatabase.Refresh();
        ExtractAssetMaterials(importAsset, outputDir);
    }

    public static void SearchAndRemapMaterials(string assetPath)
    {
        assetPath = GetPathOfImporter(assetPath);

        ModelImporter importModel = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        importModel.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Local);
        importModel.SaveAndReimport();
    }

    public static void CreateMaterial(string path)
    {
        string relativePath = ImportManager.GetPathOfImporter(path);

        if (AssetDatabase.LoadAssetAtPath(relativePath, typeof(Material)) == null)
        {
            Material mat = new Material(Shader.Find("Standard"));

            AssetDatabase.CreateAsset(mat, relativePath);
        }
    }
}

public class EditorWindowParameters
{
    public string editorKey, editorValue, editorConfig;
    public bool hasCreated = false;
}

public class WindowManager : Editor
{
    private string newVariableKey = "";
    public string[,] defaultVariables;
    //public bool canAddCustomVariables = false;
    private List<EditorWindowParameters> editorParams;
    private ReorderableList reorderableList;

    private string[] GetMinMaxValue(string editorConfig)
    {
        return editorConfig.Split(':')[1].Split(',');
    }

    //public static Vector4 StringToVec4(string editorVal)
    //{
    //    if (editorVal.StartsWith("(") && editorVal.EndsWith(")"))
    //    {
    //        editorVal = editorVal.Substring(1, editorVal.Length - 2);
    //    }

    //    string[] editorVec4Val = editorVal.Split(',');

    //    Vector4 vec4Val = new Vector4(
    //        float.Parse(editorVec4Val[0]),
    //        float.Parse(editorVec4Val[1]),
    //        float.Parse(editorVec4Val[2]),
    //        float.Parse(editorVec4Val[3]));

    //    return vec4Val;
    //}

    //public static Vector3 StringToVec3(string editorVal)
    //{
    //    if (editorVal.StartsWith("(") && editorVal.EndsWith(")"))
    //    {
    //        editorVal = editorVal.Substring(1, editorVal.Length - 2);
    //    }

    //    string[] editorVec4Val = editorVal.Split(',');

    //    Vector4 vec3Val = new Vector4(
    //        float.Parse(editorVec4Val[0]),
    //        float.Parse(editorVec4Val[1]),
    //        float.Parse(editorVec4Val[2]));

    //    return vec3Val;
    //}

    private void InitialiseList()
    {
        editorParams = new List<EditorWindowParameters>();

        for (int i = 0; i < defaultVariables.GetLength(0); i++)
        {
            editorParams.Add(new EditorWindowParameters { editorKey = defaultVariables[i, 0], editorValue = defaultVariables[i, 1], editorConfig = defaultVariables[i, 2] });
        };

        reorderableList = new ReorderableList(editorParams, typeof(Dictionary<string, string>), false, false, true, true);

        //reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        //{
        //    EditorGUIUtility.labelWidth = 200f;
        //    EditorWindowParameters item = editorParams[index];

        //    if (item.editorConfig.StartsWith("float"))
        //    {
        //        string[] minMax = GetMinMaxValue(item.editorConfig);
        //        item.editorValue = "" + EditorGUI.Slider(
        //        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, float.Parse(item.editorValue), float.Parse(minMax[0]), float.Parse(minMax[1]));
        //    }

        //    else if (item.editorConfig.StartsWith("int"))
        //    {
        //        string[] minMax = GetMinMaxValue(item.editorConfig);
        //        item.editorValue = "" + EditorGUI.IntSlider(
        //        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, int.Parse(item.editorValue), int.Parse(minMax[0]), int.Parse(minMax[1]));
        //    }

        //    else if (item.editorConfig.StartsWith("vec4"))
        //    {
        //        //string[] minMax = GetMinMaxValue(item.editorConfig);
        //        item.editorValue = "" + EditorGUI.Vector4Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, StringToVec4(item.editorValue));
        //    }

        //    else if (item.editorConfig.StartsWith("vec3"))
        //    {
        //        string[] minMax = GetMinMaxValue(item.editorConfig);
        //        item.editorValue = "" + EditorGUI.Vector3Field(
        //        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, StringToVec3(item.editorValue));
        //    }

        //    else if (item.editorConfig.StartsWith("bool"))
        //    {
        //        item.editorValue = EditorGUI.Toggle(
        //        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, item.editorValue == "True" || item.editorValue == "true") ? "True" : "False";
        //    }

        //    else if (item.editorConfig.StartsWith("dropdown"))
        //    {
        //        string[] options = item.editorConfig.Split(':')[1].Split(',');
        //        item.editorValue = options[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, Array.IndexOf(options, item.editorValue), options)];
        //    }

        //    // Create an enum command where, it pulls values from enum and GUI settings here
        //    //else if (item.editorConfig.StartsWith("enum"))
        //    //{
        //    //    enum options {item.editorConfig.Split(':')[1].Split(',')}
        //    //    item.editorValue = options[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, Array.IndexOf(options, item.editorValue), options)];
        //    //}

        //};

        //reorderableList.onCanRemoveCallback = (ReorderableList rlist) =>
        //{
        //    return editorParams[rlist.index].hasCreated;
        //};

        //reorderableList.onCanAddCallback = (ReorderableList rlist) =>
        //{
        //    return canAddCustomVariables && newVariableKey.Length > 0 && editorParams.All((variable) => variable.editorKey != newVariableKey);
        //};

        //reorderableList.onAddCallback = (ReorderableList l) =>
        //{
        //    editorParams.Add(new EditorWindowParameters { editorKey = newVariableKey, hasCreated = true });
        //    newVariableKey = "";
        //    focusControl = () => GUI.FocusControl("New Variable Key");
        //};
    }

    //Action focusControl;

    /*private void OnGUI()
    {
        if (reorderableList == null)
        {
            InitialiseList();
            //position = new Rect(Event.current.mousePosition - new Vector2(200, 200), new Vector2(400, 400));
        }

        //GUILayout.BeginVertical(GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));
        //GUILayout.BeginVertical(GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));
        //reorderableList.DoLayoutList();
        //GUILayout.EndVertical();

        //if (canAddCustomVariables)
        //{
        //    GUI.SetNextControlName("New Variable Key");
        //    newVariableKey = EditorGUILayout.TextField("New Variable Key", newVariableKey);
        //}

        //EditorGUILayout.BeginHorizontal();
        //if (GUILayout.Button("Cancel"))
        //{
        //    Close();
        //}

        if (GUILayout.Button("Generate Tree"))
        {
            //Close();
            OnStart(editorParams);
        }

        EditorGUILayout.EndHorizontal();

        //if (focusControl != null)
        //{
        //    focusControl();
        //    focusControl = null;
        //}
    }*/

    public Action<List<EditorWindowParameters>> OnStart;
    //private void OnInspectorUpdate()
    //{
    //    Repaint();
    //}
}

public class TreeGenEditor
{
    private Texture cypressLeaf, cypressNormal;

    private void Awake()
    {
        //cypressLeaf = Resources.Load<Texture>("cypressLeaf"); 
        //cypressNormal = Resources.Load<Texture>("Normal");
    }

    //[MenuItem("Assets/Blendity/Generate/Trees 2", true)]
    public static bool GenerateTreeValid()
    {
        string activeName = ImportManager.GetNameOfFile();
        return !activeName.EndsWith("Assets") && Directory.Exists(activeName);
    }

    //[MenuItem("Assets/Blendity/Generate/Trees 2")]
    public static void GenerateTree()
    {
        string presetsPath = $@"{ImportManager.GetPathOfPackage()}\blender~\2.92\scripts\presets\operator\add_curve_sapling\";
        string[] presets = Array.ConvertAll(Directory.GetFiles(presetsPath), (path) => path.Split('\\').Last());
        string options = string.Join(",", presets);

        WindowManager windowManager = ScriptableObject.CreateInstance<WindowManager>();

        string[,] defaultVariables = {
            { "Number of Trees", "1","int:1,20" },
            { "tree_type", presets[0], $"dropdown:{options}"},
            { "Bevel", "True", "bool" },
            { "Levels", "2", "int:1,4" },
            { "Scale", "5", "int:5,10" },
            //{ "Leaf Density", "180", "int:40,500" },
            //{ "Leaf Scale", "0.3", "float:0.2,0.6" },
            { "generate_LODs", "True", "bool" }
            };

        windowManager.defaultVariables = defaultVariables;
        windowManager.OnStart = (List<EditorWindowParameters> editorParams) =>
        {
            //EditorUtility.DisplayProgressBar("Creating Trees!", "Generating Trees", .1f);

            int numOfTrees = int.Parse(editorParams[0].editorValue);
            //int treeLevels = int.Parse(editorParams[2].editorValue);
            int treeScale = int.Parse(editorParams[4].editorValue);
            //int treeLeavesCount = int.Parse(editorParams[3].editorValue);

            editorParams.RemoveAt(0);
            //editorParams.RemoveAt(2);
            editorParams.RemoveAt(4);
            //editorParams.RemoveAt(3); 

            Func<string, int, Dictionary<string, string>> EnvelopeCreator = (string path, int threadSeed) =>
            {
                int seed = (int)Stopwatch.GetTimestamp() + threadSeed;
                string outputStr = $@"{path}\";
                Dictionary<string, string> envelopeVars = new Dictionary<string, string>{
                    {"seed",$"{seed}"},
                    {"output",$"{outputStr}"}
                    };

                editorParams.ForEach((variable) => envelopeVars.Add(variable.editorKey, variable.editorValue));
                return envelopeVars;
            };

            Func<string, int, Dictionary<string, string>> ScaleEnvelopeCreator = (string path, int scaleNum) =>
            {
                int scale = scaleNum;
                string outputStr = $@"{path}\";
                Dictionary<string, string> scaleEnvelopeVars = new Dictionary<string, string>{
                    {"scale",$"{scale}"},
                    {"output", $"{outputStr}" }
                    };

                editorParams.ForEach((variable) => scaleEnvelopeVars.Add(variable.editorKey, variable.editorValue));
                return scaleEnvelopeVars;
            };

            List<OutputCmd> processOutputs = App.RunCmdTimesN(
              $@"-b -P py_scripts~\generate_tree_v3.py",
              numOfTrees,
              EnvelopeCreator
                );

            List<OutputCmd> scaleOutput = App.RunCmdTimesN(
                $@"-b -P py_scripts~\generate_tree_v3.py",
                treeScale,
                ScaleEnvelopeCreator
                );

            //EditorUtility.DisplayProgressBar("Creating Trees!", "Importing Models", .5f);

            float progressPerLoop = 0.4f / processOutputs.Count;
            float progress = 0.55f;
            int i = 1;

            AssetDatabase.Refresh();

            //if (presets[3] == "Med Cypress")
            //{
            //    string materialDir = processOutputs[0].cmdOutputFile + "materials";
            //    Directory.CreateDirectory(materialDir);

            //    string barkMaterial = materialDir + @"\bark material.mat";
            //    Debug.Log("Bark material path: " + barkMaterial);
            //    ImportManager.CreateMaterial(barkMaterial);
            //    string leavesMaterial = materialDir + @"\leaves material.mat";
            //    ImportManager.CreateMaterial(leavesMaterial);
            //}


            // Work on loading textures on creation

            // Then integrate the files to Vegetation.cs

            // Spawn certain types in biomes

            Texture cypressLeaf, cypressNormal;

            string materialDir = processOutputs[0].cmdOutputFile + "materials";
            Directory.CreateDirectory(materialDir);

            string barkMaterial = materialDir + @"\bark material.mat";
            Debug.Log("Bark material path: " + barkMaterial);
            Material barkMat = new Material(Shader.Find("Standard"));
            barkMat.color = new Color32(87, 62, 34, 255);
            Debug.Log("Bark mat colour" + barkMat.color);

            string leavesMaterial = materialDir + @"\leaves material.mat";
            //ImportManager.CreateMaterial(leavesMaterial);
            //Material leafMat = new Material(Shader.Find("Nature/SpeedTree8"));

            cypressLeaf = Resources.Load<Texture>("cypressLeaf");
            cypressNormal = Resources.Load<Texture>("Normal");

            if (presets[0] == "Med Cypress")
            {
                bool hasChosen = true;
                Debug.Log("Med Cypress" + presets[0] + hasChosen);
                Material leafMat = new Material(Shader.Find("Nature/SpeedTree8"));
                leafMat.SetTexture("_MainTex", cypressLeaf);
                leafMat.EnableKeyword("_NORMALMAP");
                leafMat.SetTexture("_BumpMap", cypressNormal);
            }


            processOutputs.ForEach((processOutput) =>
            {
                UnityEngine.Debug.Log(processOutput);
                string s = Array.Find(processOutput.resultStr.Split('\n'), (str) => str.StartsWith("FBX export starting... '")).Split('\'')[1].Replace(@"\\", @"\");
                //ImportManager.SearchAndRemapMaterials(s);

                progress += progressPerLoop;
                //EditorUtility.DisplayProgressBar("Creating Trees!", "Searching for Materials #" + i++, progress);
            });

            progress += progressPerLoop;
            //EditorUtility.DisplayProgressBar("Creating Trees !", "Searching for Materials #" + i++, progress);

            //EditorUtility.ClearProgressBar();
        };

        //windowManager.ShowModalUtility();
    }
}

public class Vegation : MonoBehaviour
{
    #region Old Code
    /*
    public GameObject Grass;
    public GameObject Sand;
    public GameObject MainMesh;
    GameObject[] Index_Vegation;
    public void GenerateVegation(int width, int height, float[,] heightmap, int[] BuldingMap, AnimationCurve HeightCurve, float mesh_Height, int seed) //add references  for your prefabs here or watever if you want or just include them
    {
        // Call the tree generation code then the bush generation as well.


        Vector3 RelativePosition = MainMesh.transform.position;

        int size = 0;
        //array for buildings to be put into, 
        for (int i = 0; i < BuldingMap.Length; i++)
        {
            if (BuldingMap[i] == 0 )
                size++;
        }

        Index_Vegation = new GameObject[size];
        int size_index = 0;


        var Meshh2 = MainMesh.GetComponent<MeshFilter>().sharedMesh;

        Vector3[] verticies = Meshh2.normals;
        Vector3[] MeshV = Meshh2.vertices;
       // for (var i = 0; i < verticies.Length; i++)
     //   {
       //     Vector3 vertPosition = transform.TransformPoint(MeshV[i]) * 10;
        //    Vector3 vertNormal = transform.TransformDirection(verticies[i]);
         //   if (Vector3.Dot(vertNormal, Vector3.up) > 0.98)
             //   Debug.DrawRay(vertPosition, vertNormal * 10, Color.red, 10, true);
        //}





        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {


                //reset relative position
                RelativePosition.x = (-width * MainMesh.transform.localScale.x) / 2 + 5;
                RelativePosition.z = (height * MainMesh.transform.localScale.z) / 2 - 5;
   
                //algorithm for using the seed to create a "random" pattern of spawns
                System.Random Ran_Seed = new System.Random(seed);
                seed = seed + Ran_Seed.Next(seed, seed);
                seed++;
                seed = seed + Ran_Seed.Next(21, 45);

                Vector3 vertPosition = transform.TransformPoint(MeshV[y * width + x]) * 10;
                Vector3 vertNormal = transform.TransformDirection(verticies[y * width + x]);
                if (Vector3.Dot(vertNormal, Vector3.up) > 0.98 && BuldingMap[y * width + x] == 0)
                    BuldingMap[y * width + x] = 10;

                //Spawn an object in sandy area on random
                if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.3 && heightmap[x, y] < 0.45)
                {

                    float OffSet_X = Ran_Seed.Next(1, 10); //1 in 10 chance 
                    
                    if (OffSet_X <= 1) // lower == less chance
                    {
                        //Get location in world
                        RelativePosition.x += x * MainMesh.transform.localScale.x + MainMesh.transform.localScale.x / 2; 
                        RelativePosition.z -= y * MainMesh.transform.localScale.z + MainMesh.transform.localScale.z / 2; //Z is Y axis in a 3d Plane
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Sand.transform.localScale.y / 2.2f);

                        //Make sure index goes up so can store all the objects 
                        Index_Vegation[size_index] = Instantiate(Sand, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

                //Spawn a tree in grass at random
                else if (BuldingMap[y * width + x] == 10 && heightmap[x, y] > 0.45 && heightmap[x, y] < 0.60)
                {

                    float OffSet_X = Ran_Seed.Next(1, 1);
                    if (OffSet_X <= 1)
                    {
                        RelativePosition.x += x * MainMesh.transform.localScale.x   ;
                        RelativePosition.z -= y * MainMesh.transform.localScale.z  ;
                        RelativePosition.y = HeightCurve.Evaluate(heightmap[x, y]) * mesh_Height * MainMesh.transform.localScale.y + (Grass.transform.localScale.y / 1.8f);


                        Index_Vegation[size_index] = Instantiate(Grass, RelativePosition, transform.rotation);
                        size_index++;
                    }
                }

            }

        }
        //TODO, import biomomes array and use those values instead so the is no with values
    }


    public void ClearVegation()
    {
        if (Index_Vegation != null)
        {
            for (int i = 0; i < Index_Vegation.Length; i++)
            {

                DestroyImmediate(Index_Vegation[i]);
            }

        }
    }
    */
    #endregion

    public void GenerateVegation()
    {
        //WindowManager windowManager = ScriptableObject.CreateInstance<WindowManager>();
        //windowManager.OnStart(editorParams);

        TreeGenEditor.GenerateTree();
        print("Generated trees");
    }
}