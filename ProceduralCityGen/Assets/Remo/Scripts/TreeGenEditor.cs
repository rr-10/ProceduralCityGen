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

// This is the editor code for the interface
// for creating trees.

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

public class App : Editor
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
            
        if (!isThreaded)
        {
            EditorUtility.DisplayProgressBar("Executing The Command", cmd, .25f);
        }

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

        if (!isThreaded)
        {
            EditorUtility.ClearProgressBar();
        }  

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

public class ImportManager : Editor
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
        //importModel.SearchAndRemapMaterials(ModelImporterMaterialName.BasedOnMaterialName, ModelImporterMaterialSearch.Local);
        importModel.SaveAndReimport();
    }

    //public static void CreateMaterial(string path)
    //{
    //    string relativePath = ImportManager.GetPathOfImporter(path);
    //    if (AssetDatabase.LoadAssetAtPath(relativePath, typeof(Material)) == null)
    //    {
    //        Material mat = new Material(Shader.Find("Standard"));

    //        AssetDatabase.CreateAsset(mat, relativePath);
    //    }
    //}
}

public class EditorWindowParameters
{
    public string editorKey, editorValue, editorConfig;
    public bool hasCreated = false;
}

public class WindowManager : EditorWindow
{
    private string newVariableKey = "";
    public string[,] defaultVariables;
    public bool canAddCustomVariables = false;
    private List<EditorWindowParameters> editorParams;
    private ReorderableList reorderableList;

    private string[] GetMinMaxValue(string editorConfig)
    {
        return editorConfig.Split(':')[1].Split(',');
    }

    private void InitialiseList()
    {
        editorParams = new List<EditorWindowParameters>();

        for (int i = 0; i < defaultVariables.GetLength(0); i++)
        {
            editorParams.Add(new EditorWindowParameters { editorKey = defaultVariables[i, 0], editorValue = defaultVariables[i, 1], editorConfig = defaultVariables[i, 2] });
        }

        reorderableList = new ReorderableList(editorParams, typeof(Dictionary<string, string>), false, false, true, true);

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            EditorGUIUtility.labelWidth = 200f;
            EditorWindowParameters item = editorParams[index];

            if (item.editorConfig.StartsWith("float"))
            {
                string[] minMax = GetMinMaxValue(item.editorConfig);
                item.editorValue = "" + EditorGUI.Slider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, float.Parse(item.editorValue), float.Parse(minMax[0]), float.Parse(minMax[1]));
            }

            else if (item.editorConfig.StartsWith("int"))
            {
                string[] minMax = GetMinMaxValue(item.editorConfig);
                item.editorValue = "" + EditorGUI.IntSlider(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, int.Parse(item.editorValue), int.Parse(minMax[0]), int.Parse(minMax[1]));
            }

            else if (item.editorConfig.StartsWith("bool"))
            {
                item.editorValue = EditorGUI.Toggle(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, item.editorValue == "True" || item.editorValue == "true") ? "True" : "False";
            }

            else if (item.editorConfig.StartsWith("dropdown"))
            {
                string[] options = item.editorConfig.Split(':')[1].Split(',');
                item.editorValue = options[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, Array.IndexOf(options, item.editorValue), options)];
            }

            // Create an enum command where, it pulls values from enum and GUI settings here
            //else if (item.editorConfig.StartsWith("enum"))
            //{
            //    enum options {item.editorConfig.Split(':')[1].Split(',')}
            //    item.editorValue = options[EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), item.editorKey, Array.IndexOf(options, item.editorValue), options)];
            //}

        };

        reorderableList.onCanRemoveCallback = (ReorderableList rlist) =>
        {
            return editorParams[rlist.index].hasCreated;
        };

        reorderableList.onCanAddCallback = (ReorderableList rlist) =>
        {
            return canAddCustomVariables && newVariableKey.Length > 0 && editorParams.All((variable) => variable.editorKey != newVariableKey);
        };

        reorderableList.onAddCallback = (ReorderableList l) =>
        {
            editorParams.Add(new EditorWindowParameters { editorKey = newVariableKey, hasCreated = true });
            newVariableKey = "";
            focusControl = () => GUI.FocusControl("New Variable Key");
        };
    }

    Action focusControl;

    private void OnGUI()
    {
        if (reorderableList == null)
        {
            InitialiseList();
            position = new Rect(Event.current.mousePosition - new Vector2(200, 200), new Vector2(400, 400));
        }

        GUILayout.BeginVertical(GUILayout.MinHeight(position.height - EditorGUIUtility.singleLineHeight * 3));
        reorderableList.DoLayoutList();
        GUILayout.EndVertical();

        if (canAddCustomVariables)
        {
            GUI.SetNextControlName("New Variable Key");
            newVariableKey = EditorGUILayout.TextField("New Variable Key", newVariableKey);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
            
        if (GUILayout.Button("Start"))
        {
            Close();
            OnStart(editorParams);
        }
        EditorGUILayout.EndHorizontal();

        if (focusControl != null)
        {
            focusControl();
            focusControl = null;
        }
    }

    public Action<List<EditorWindowParameters>> OnStart;
    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

public class TreeGenEditor : EditorWindow
{
    Font titleFont;
    Texture oliveTreeImg;
    Texture cypressTreeImg;
    Texture pineTreeImg;

    bool isOliveTree = false;
    bool isCypressTree = false;
    bool isPineTree = false;

    int btnWidth = 75;
    int btnHeight = 75;

    #region Old code
    //[MenuItem("Assets/Create/FoliageGen/Tree Generator")]
    //public static void ShowWindow()
    //{
    //    EditorWindow.GetWindow(typeof(TreeGenEditor));
    //}
    #endregion

    //void OnGUI()
    //{
    //    #region Old Code
    //    //GUILayout.Space(10f);

    //    //var style = new GUIStyle(GUI.skin.label);
    //    //style.alignment = TextAnchor.MiddleCenter;
    //    //style.fontStyle = FontStyle.Bold;
    //    //style.fontSize = 20;
    //    ////titleFont = Resources.Load<Texture>
    //    ////style.font = 
    //    //EditorGUILayout.LabelField("Tree Generator", style);
    //    //GUILayout.Space(20f);

    //    //var btnStyle = new GUIStyle(GUI.skin.label);
    //    //btnStyle.alignment = TextAnchor.MiddleCenter;

    //    //GUILayout.Label("Tree Selection");
    //    ////[Tooltip("Please choose the tree to generate")]
    //    //GUILayout.Space(10f);     
    //    ////Rect btnRect = new Rect(0, 0, 75, 75);
    //    ////GUIContent guiContent;
    //    //GUILayout.BeginHorizontal();

    //    ////if (GUIButtons.GUIButton(oliveTreeImg = Resources.Load<Texture>("oliveTreeSelection"), oliveTreeImg.width, oliveTreeImg.height, "Olive Tree"))
    //    ////{
    //    ////    UnityEngine.Debug.Log("Olive tree selected");
    //    ////}

    //    //if (GUILayout.Button(oliveTreeImg = Resources.Load<Texture>("oliveTreeSelection"),
    //    //    GUILayout.Width(oliveTreeImg.width), GUILayout.Height(oliveTreeImg.height)))
    //    //{
    //    //    isOliveTree = true;
    //    //    UnityEngine.Debug.Log("Olive tree selected");
    //    //}

    //    //if (GUILayout.Button(cypressTreeImg = Resources.Load<Texture>("cypressTreeSelection"),
    //    //    GUILayout.Width(cypressTreeImg.width), GUILayout.Height(cypressTreeImg.height)))
    //    //{
    //    //    isCypressTree = true;
    //    //    UnityEngine.Debug.Log("Cypress tree selected");
    //    //}

    //    //if (GUILayout.Button(pineTreeImg = Resources.Load<Texture>("pineTreeSelection"),
    //    //    GUILayout.Width(pineTreeImg.width), GUILayout.Height(pineTreeImg.height)))
    //    //{
    //    //    isPineTree = true;
    //    //    UnityEngine.Debug.Log("Pine tree selected");
    //    //}

    //    //GUILayout.EndHorizontal();
    //    //GUILayout.Space(20f);
    //    #endregion

    //}

    [MenuItem("Assets/Blendity/Generate/Trees 2", true)]
    public static bool GenerateTreeValid()
    {
        string activeName = ImportManager.GetNameOfFile();
        return !activeName.EndsWith("Assets") && Directory.Exists(activeName);
    }

    [MenuItem("Assets/Blendity/Generate/Trees 2")]
    public static void GenerateTree()
    {
        string presetsPath = $@"{ImportManager.GetPathOfPackage()}\blender~\2.92\scripts\presets\operator\add_curve_sapling\";
        string[] presets = Array.ConvertAll(Directory.GetFiles(presetsPath), (path) => path.Split('\\').Last());
        string options = string.Join(",", presets); 

        WindowManager windowManager = ScriptableObject.CreateInstance<WindowManager>();
        
        string[,] defaultVariables = {
            { "Number of Trees", "1","int:1,20" },
            { "tree_type", presets[0], $"dropdown:{options}"},
            { "Bevel", "False", "bool" },        
            { "Leaves", "True", "bool" },
            { "Levels", "2", "int:1,5" },
            //{ "Leaf Distribution", "1", "enum"},
            //{ "Leaf Scale", "0.4", "float" },
            { "generate_LODs", "True", "bool" }
            };

        windowManager.defaultVariables = defaultVariables;
        windowManager.OnStart = (List<EditorWindowParameters> editorParams) =>
        {
            EditorUtility.DisplayProgressBar("Creating Trees !", "Generating Trees", .1f);

            int numOfTrees = int.Parse(editorParams[0].editorValue);
            //int branchRatio = int.Parse(editorParams[2].editorValue);
            editorParams.RemoveAt(0);

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

            List<OutputCmd> processOutputs = App.RunCmdTimesN(
              $@"-b -P py_scripts~\generate_tree_v2.py",
              numOfTrees,
              EnvelopeCreator
                );

            EditorUtility.DisplayProgressBar("Creating Trees!", "Importing Models", .5f);

            float progressPerLoop = 0.4f / processOutputs.Count;
            float progress = 0.55f;
            int i = 1;

            AssetDatabase.Refresh();

            //string materialDir = processOutputs[0].cmdOutputFile + "materials";
            //Directory.CreateDirectory(materialDir);

            //string barkMaterial = materialDir + @"\bark material.mat";
            //ImportManager.CreateMaterial(barkMaterial);
            //string leavesMaterial = materialDir + @"\leaves material.mat";
            //ImportManager.CreateMaterial(leavesMaterial);

            processOutputs.ForEach((processOutput) =>
            {
                UnityEngine.Debug.Log(processOutput);
                string s = Array.Find(processOutput.resultStr.Split('\n'), (str) => str.StartsWith("FBX export starting... '")).Split('\'')[1].Replace(@"\\", @"\");
                ImportManager.SearchAndRemapMaterials(s);

                progress += progressPerLoop;
                EditorUtility.DisplayProgressBar("Creating Trees!", "Searching for Materials #" + i++, progress);
            });

            //progress += progressPerLoop;
            //EditorUtility.DisplayProgressBar("Creating Trees !", "Searching for Materials #" + i++, progress);

            EditorUtility.ClearProgressBar();
        };

        windowManager.ShowModalUtility();
    }
}
