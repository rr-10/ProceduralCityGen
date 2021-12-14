using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Title: ImportFiles
/// Author: Remo Reji Thomas
/// Date: 28/11/2021
/// 
/// This function is called as soon as trees are created
/// There was a known issue with Blendity as that the filepaths
/// for generating the foliage was local and that it would not generate
/// if the project was in a different computer. I found a way to get 
/// the name of the tree(s) generated and the directory so that
/// the trees when created would use the correct path (MoveFiles)
/// to generate which is Resources
/// </summary>
[InitializeOnLoad]
static class ImportFiles
{
    public static string PackagePath;
    public static string ResourcesPath;

    static ImportFiles()
    {
        EditorApplication.update += Update;
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

}


