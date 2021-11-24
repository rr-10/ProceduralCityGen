using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map_Generation))]
public class Map_Gen_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        Map_Generation Map_Gen = (Map_Generation)target;
        Building_Genertor Buildings = FindObjectOfType<Building_Genertor>();
        Vegation veg = FindObjectOfType<Vegation>();
        if (DrawDefaultInspector())
            if (Map_Gen.Auto_Update)
                Map_Gen.Generate_Map();

        if (GUILayout.Button("Generate"))
        {
            Map_Gen.Generate_Map();
        }
        if (GUILayout.Button("WipeMap"))
        {
            Buildings.clearBuildings();
            //veg.ClearVegation();
        }


        //test
    }
}
