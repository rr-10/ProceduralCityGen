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

    //Random Chances and max roof
    private SerializedProperty m_doorPercentChance;
    private SerializedProperty m_windowPercentChance;
    private SerializedProperty m_balconyPercentChance;
    private SerializedProperty m_maximumFloors;
    
    //Rule
    private SerializedProperty m_rule;

    protected static bool showPrefabs = true;
    protected static bool showWallPrefabs = true;

    public void OnEnable()
    {
        m_object = new SerializedObject(target);
        m_normalWallPrefab = m_object.FindProperty("NormalWallPrefab");
        m_windowWallPrefab = m_object.FindProperty("WindowWallPrefab");
        m_doorWallPrefab = m_object.FindProperty("DoorWallPrefab");
        m_balconyWallPrefab = m_object.FindProperty("BalconyWallPrefab");
        m_floorPrefab = m_object.FindProperty("FloorPrefab");
        m_roofPrefab = m_object.FindProperty("RoofPrefab");

        m_doorPercentChance = m_object.FindProperty("setDoorChance");
        m_windowPercentChance = m_object.FindProperty("setWindowChance");
        m_balconyPercentChance = m_object.FindProperty("setBalconyChance");
        m_maximumFloors = m_object.FindProperty("MaximumFloors");

        m_rule = m_object.FindProperty("Rule");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_rule);
        
        
        EditorGUILayout.Slider(m_doorPercentChance, 0.0f, 1.0f);
        EditorGUILayout.Slider(m_windowPercentChance, 0.0f, 1.0f);
        EditorGUILayout.Slider(m_balconyPercentChance, 0.0f, 1.0f);

        EditorGUILayout.PropertyField(m_maximumFloors);
     
        //Building Prefabs
        showPrefabs = EditorGUILayout.Foldout(showPrefabs, "Building Prefabs");
      
        if (showPrefabs)
        {
            showWallPrefabs = EditorGUILayout.Foldout(showWallPrefabs, "Wall Prefabs");
            if (showWallPrefabs)
            {
                EditorGUILayout.PropertyField(m_normalWallPrefab);
                EditorGUILayout.PropertyField(m_windowWallPrefab);
                EditorGUILayout.PropertyField(m_doorWallPrefab);
                EditorGUILayout.PropertyField(m_balconyWallPrefab);
            }

            EditorGUILayout.PropertyField(m_roofPrefab);

            EditorGUILayout.PropertyField(m_floorPrefab);
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
