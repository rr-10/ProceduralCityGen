/// <summary>
///-----------------------------------------------------------------------
///
///     Title: Blendity/CommandOutput(OutputCmd), Core (App), Utils (ImportManager), 
///     KeyValueConfig (EditorWindowParameters), ParamsModel(WindowManager),
///     TreeGen (TreeGenEditor)
///     Author: Anas Einea (AETuts_
///     Date: 28/11/2021
///     Source Code to Blendity in Unity and TreeGen code
///     Available at: https://github.com/anaseinea/Blendity
/// 
///     This class combines all relevant classes for foliage generation and 
///     is modified to generate the desired results
///-----------------------------------------------------------------------
/// </summary>


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
    public static string createPath;

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
        //string path = ImportManager.GetNameOfFile();
        //string path = $@"E:\ProceduralCityGen\ProceduralCityGen\Assets\Resources";
        string path = Path.GetFullPath("Assets/Resources");
        //path = ImportManager.GetWindowsPath(path);
        //createPath = path;
        //Debug.Log("RUNCMD RESOURCES: " + path);

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
    //public static string GetNameOfFile() => $@"E:\ProceduralCityGen\ProceduralCityGen\Assets\Resources";
    public static string GetNameOfFile() => Path.GetFullPath("Assets/Resources");
    private static string[] GetNamesOfSelectedFiles() => Array.ConvertAll(Selection.objects, obj => Path.GetFullPath(AssetDatabase.GetAssetPath(obj.GetInstanceID())));
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
    public static bool treesAssigned = false;
    public static string activePath;
    public static string[] treePresets;
    public enum PresetValues
    {
        OliveTree,
        MedCypress,
        PalmTree,
        StonePine,
        SicilianFir,
        BasilBush,
        JuniperBush,
        MyrtleBush
    }

    public static PresetValues presetValues;

    public static bool GenerateTreeValid()
    {
        activePath = Path.GetFullPath("Assets/Resources");
        //activePath = $@"E:\ProceduralCityGen\ProceduralCityGen\Assets\Resources";
        return !activePath.EndsWith("Assets") && Directory.Exists(activePath);
    }

    public static void GenerateTree()
    {
        string presetsPath = $@"{ImportManager.GetPathOfPackage()}\blender~\2.92\scripts\presets\operator\add_curve_sapling\";
        string[] presets = Array.ConvertAll(Directory.GetFiles(presetsPath), (path) => path.Split('\\').Last());
        treePresets = presets;
        string options = string.Join(",", presets);
        var oldPath = ImportManager.GetNameOfFile();

        WindowManager windowManager = ScriptableObject.CreateInstance<WindowManager>();

        string[,] defaultVariables = {
            { "Number of Trees", "1","int:1,50" },
            { "tree_type", presets[0], $"dropdown:{options}"},
            { "scale", "1", "float:1,25"},
            { "levels", "2", "int:1,5"},
            //{ "lef density", "120", "int:0,300"},
            { "generate_LODs", "False", "bool" }
            };

        windowManager.defaultVariables = defaultVariables;
        windowManager.OnStart = (List<EditorWindowParameters> editorParams) =>
        {
            EditorUtility.DisplayProgressBar("Creating Trees !", "Generating Trees", .1f);

            int numOfTrees = int.Parse(editorParams[0].editorValue);

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
              $@"-b -P py_scripts~\generate_tree_final.py",
              numOfTrees,
              EnvelopeCreator
                );

            EditorUtility.DisplayProgressBar("Creating Trees!", "Importing Models", .5f);

            float progressPerLoop = 0.4f / processOutputs.Count;
            float progress = 0.55f;
            int i = 1;

            processOutputs.ForEach((processOutput) =>
            {
                UnityEngine.Debug.Log(processOutput);
                string s = Array.Find(processOutput.resultStr.Split('\n'), (str) => str.StartsWith("FBX export starting... '")).Split('\'')[1].Replace(@"\\", @"\");
                //ImportManager.SearchAndRemapMaterials(s);
                Debug.Log("s: " + s);

                progress += progressPerLoop;
                EditorUtility.DisplayProgressBar("Creating Trees!", "Searching for Materials #" + i++, progress);
            });

            EditorUtility.ClearProgressBar();
        };

        windowManager.ShowModalUtility();

        MoveFiles();
    }

    /// <summary>
    /// This function would get the recently created files from Resources
    /// and move them to dedicated files. For this to work, it has to follow
    /// through each preset in order as if there was a chance of making it so
    /// that anyone can choose to create from the list, it would make the code
    /// unreachable
    /// </summary>
    public static void MoveFiles()
    {
        string rootPath = ImportManager.GetNameOfFile();

        if (treePresets[0] == "Basil Bush.py")
        {
            string folderPath = @"\BasilBush";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Basil Bush*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Basil Bush"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[1] == "Juniper Bush.py")
        {
            string folderPath = @"\JuniperBush";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Juniper Bush*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Juniper Bush"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[2] == "Med Cypress.py")
        {
            string folderPath = @"\MedCypress";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Med Cypress*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Med Cypress"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[3] == "Myrtle Bush.py")
        {
            string folderPath = @"\MyrtleBush";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Myrtle Bush*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Myrtle Bush"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[4] == "Olive Tree.py")
        {
            string folderPath = @"\OliveTrees";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Olive Tree*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Olive Tree"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[5] == "Palm Tree.py")
        {
            string folderPath = @"\PalmTree";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Palm Tree*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Palm Tree"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[6] == "Sicilian Fir.py")
        {
            string folderPath = @"\SicilianFir";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Sicilian Fir*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Sicilian Fir"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        if (treePresets[7] == "Stone Pine.py")
        {
            string folderPath = @"\StonePine";
            string newPath = rootPath + folderPath;
            string filesToDelete = @"Stone Pine*.fbx";
            string[] matches = System.IO.Directory.GetFiles(rootPath, filesToDelete);

            foreach (string match in matches)
            {
                var matcher = Path.GetFileName(match);
                Debug.Log(matcher);

                if (matcher.Contains("Stone Pine"))
                {
                    File.Move(rootPath + $@"\{ matcher}", newPath + $@"\{matcher}");
                }
            }
        }

        AssetDatabase.Refresh();

        Debug.Log("Finished assigning");
        treesAssigned = true;
    }
}
