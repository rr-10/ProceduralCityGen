using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateBuilding))]
public class EditorBuildingGen : Editor
{
    private SerializedObject m_object;
    
    //Prefabs
    private SerializedProperty m_normalWallPrefab;
    private SerializedProperty m_windowWallPrefab;
    private SerializedProperty m_doorWallPrefab;
    private SerializedProperty m_balconyWallPrefab;
    private SerializedProperty m_floorPrefab;
    private SerializedProperty m_roofPrefab;
    
    
    //TODO - Random Chances and max roof should be in here 
    
    
    
    private SerializedProperty m_bounds;

    protected static bool showPrefabs = true;
    public void OnEnable()
    {
        m_object = new SerializedObject(target);
        m_normalWallPrefab = m_object.FindProperty("NormalWallPrefab");
        m_windowWallPrefab = m_object.FindProperty("WindowWallPrefab");
        m_doorWallPrefab = m_object.FindProperty("DoorWallPrefab");
        m_balconyWallPrefab = m_object.FindProperty("BalconyWallPrefab");
        m_floorPrefab = m_object.FindProperty("FloorPrefab");
        m_roofPrefab = m_object.FindProperty("RoofPrefab");
        m_bounds = m_object.FindProperty("Bounds");
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        //EditorGUILayout.PropertyField(m_bounds);
     
        //Building Prefabs
        showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Building Prefabs");
        if (showPrefabs)
        {
            EditorGUILayout.PropertyField(m_normalWallPrefab);
            EditorGUILayout.PropertyField(m_windowWallPrefab);
            EditorGUILayout.PropertyField(m_doorWallPrefab);
            EditorGUILayout.PropertyField(m_balconyWallPrefab);

            EditorGUILayout.PropertyField(m_floorPrefab);
            EditorGUILayout.PropertyField(m_roofPrefab);
        }
        
        
        //Generate Building on button press or settings change
        GenerateBuilding generationScript = (GenerateBuilding)target;
        if (GUILayout.Button("Generate Building"))
        {
            generationScript.Generate();
        }

        //Applies all the editor settings to the script
        m_object.ApplyModifiedProperties();
    }


}
