using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingDemo))]
public class EditorBuildingGen : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingDemo generationScript = (BuildingDemo)target;

        if (GUILayout.Button("Generate Building"))
        {
            generationScript.StartGeneration();
        }
    }


}
