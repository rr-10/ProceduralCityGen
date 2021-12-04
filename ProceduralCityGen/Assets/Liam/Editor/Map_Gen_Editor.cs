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
        Building_Generator Buildings = FindObjectOfType<Building_Generator>();
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
            Buildings.ClearBuildings();
            //veg.ClearVegation();
        }

        if (GUILayout.Button("random"))
        {
            Map_Gen.randomgen();
            
            
        }
        if (GUILayout.Button(" Random_amount_200"))
        {

            for (int y = 0; y < 200; y++)
            {
                Map_Gen.randomgen();
            }


        }
        //test
    }
}
