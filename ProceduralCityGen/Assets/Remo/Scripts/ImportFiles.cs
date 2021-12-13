using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 1. Replace the Blendity tgz file with your files
/// 2. On Start, get path of resources
/// 3. 
/// </summary>

//[InitializeOnLoad]
//public class ImportFiles : MonoBehaviour
//{
//    public static string SaplingPath;

//    static ImportFiles()
//    {
//        EditorApplication.update += RunOnce;
//    }

//    static void RunOnce()
//    {
//        Debug.Log("RunOnce!");
//        EditorApplication.update -= RunOnce;
//    }


//}

[InitializeOnLoad]
static class ImportFiles
{
    //public static ImportFiles startUp;
    public static string PackagePath;
    public static string ResourcesPath;

    static ImportFiles()
    {
        EditorApplication.update += Update;
        //string folderPath = $@"\BasilBush";
        //print(PackagePath + folderPath);
    }

    static void Update()
    {
        string folderSearch = "com.ae.blendity";

        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"Library\PackageCache");
        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + folderSearch + "*.*");
        DirectoryInfo[] dirsInDir = hdDirectoryInWhichToSearch.GetDirectories("*" + folderSearch + "*.*");
        foreach (FileInfo foundFile in filesInDir)
        {
            string fullName = foundFile.FullName;
            Debug.Log("File full name: " + fullName);
        }

        foreach (DirectoryInfo foundDir in dirsInDir)
        {
            string fullName = foundDir.FullName;
            Debug.Log("Directiory fyll name: " + fullName);
            fullName = PackagePath;
        }

        ResourcesPath = Path.GetFullPath("Assets/Resources");
        Debug.Log(ResourcesPath);

        EditorApplication.update -= Update;
    }


    //Works with normal class
    //private void Start()
    //{
    //    //if (!EditorApplication.isPlayingOrWillChangePlaymode)
    //    //{
    //    //    PackagePath = Path.GetFullPath("Packages");
    //    //    print(PackagePath);
    //    //    //string folderPath = $@"\BasilBush";
    //    //    //print(PackagePath + folderPath);
    //    //    FindBlendityPackage();
    //    //}

    //}

    /*private void FindSaplingPath()
    {
        string folderSearch = "com.ae.blendity";

        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"Library\PackageCache");
        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + folderSearch + "*.*");
        DirectoryInfo[] dirsInDir = hdDirectoryInWhichToSearch.GetDirectories("*" + folderSearch + "*.*");
        foreach (FileInfo foundFile in filesInDir)
        {
            string fullName = foundFile.FullName;
            print("File full name: " + fullName);
        }

        foreach (DirectoryInfo foundDir in dirsInDir)
        {
            string fullName = foundDir.FullName;
            print("Directiory fyll name: " + fullName);
            fullName = PackagePath;
        }
    }*/

}
//public static class FindBlendityPackage
//{
    
//    static FindBlendityPackage()
//    {
//        string folderSearch = "com.ae.blendity";

//        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"Library\PackageCache");
//        FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + folderSearch + "*.*");
//        DirectoryInfo[] dirsInDir = hdDirectoryInWhichToSearch.GetDirectories("*" + folderSearch + "*.*");
//        foreach (FileInfo foundFile in filesInDir)
//        {
//            string fullName = foundFile.FullName;
//            Debug.Log("File full name: " + fullName);
//        }

//        foreach (DirectoryInfo foundDir in dirsInDir)
//        {
//            string fullName = foundDir.FullName;
//            Debug.Log("Directiory fyll name: " + fullName);
//            fullName = PackagePath;
//        }
//    }
//}

