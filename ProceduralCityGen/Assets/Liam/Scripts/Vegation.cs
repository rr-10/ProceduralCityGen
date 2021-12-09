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

/*
 * TODO: 
 * Correctly pass parameters in (somewhat successfull)
 * Add more trees and shrubs in the selection (almost)
 * Do a call where on start of project, the presets and python code is added to the respective folders
 * Add Git LFS to the project for Blendity to work (done)
 * Integrate the code into the main project (doing right now)
 * Change the whole editor window as part of Vegatation (doing atm)
 * Generation code depending on biomes value (done
 * Change the scaling of trees to be more appropriate (
 * Pull material from project to the generated trees (USe Resources.Load) (done)
 * Finish
 */

public class Vegation : MonoBehaviour
{
    private int treeCreateIterations;
    private int bushCreateIterations;

    public void GenerateVegation()
    {
        for (treeCreateIterations = 0; treeCreateIterations < 5; treeCreateIterations++)
        {
            TreeGenEditor.GenerateTree();
            //TreeGenEditor.MoveFiles();
        }
        print("Generated trees");

        for (bushCreateIterations = 0; bushCreateIterations < 3; bushCreateIterations++)
        {
            TreeGenEditor.GenerateTree();
        }
        print("Generated bushes");
    }
}