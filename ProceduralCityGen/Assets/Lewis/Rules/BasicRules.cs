using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


[Serializable]
public struct ruleWithWeight
{
    public BuildProcess NextRule;
    [Range(0, 1)] public float SelectionChance;
}

[CreateAssetMenu(menuName = "Rules")]
public class BasicRules : RuleBase
{
    [SerializeField] private BuildProcess firstRule = BuildProcess.NoChange;
    [SerializeField] private ruleWithWeight[] fromNoChange;
    [SerializeField] private ruleWithWeight[] fromShrinkColumn;
    [SerializeField] private ruleWithWeight[] fromShrinkRow;
    [SerializeField] private ruleWithWeight[] fromShrinkRandom;

    public override BuildProcess GetNextProcess(BuildProcess input)
    {
        BuildProcess toReturn = BuildProcess.NoChange;

        // Determine the next rule from the rules
        switch (input)
        {
            case BuildProcess.NoChange:
                toReturn = ResolveSymbol(fromNoChange);
                break;
            case BuildProcess.ShrinkColumn:
                toReturn = ResolveSymbol(fromShrinkColumn);
                break;
            case BuildProcess.ShrinkRow:
                toReturn = ResolveSymbol(fromShrinkRow);
                break;
            case BuildProcess.ShrinkRandom:
                toReturn = ResolveSymbol(fromShrinkRandom);
                break;
            default:
                Debug.Log("Invalid Symbol!!!");
                break;
        }

        return toReturn;
    }

    public override BuildProcess GetFirstProcess()
    {
        return firstRule;
    }

    //Correct the weights of each chance by dividing by the number of choices 
    public override void CorrectWeights()
    {
        //From No Change 
        for (int i = 0; i < fromNoChange.Length; i++)
        {       
            fromNoChange[i].SelectionChance = fromNoChange[i].SelectionChance / fromNoChange.Length;
        }

        //From ShrinkColumn
        for (int i = 0; i < fromShrinkColumn.Length; i++)
        {
            fromShrinkColumn[i].SelectionChance = fromShrinkColumn[i].SelectionChance / fromShrinkColumn.Length;
        }

        //From ShrinkRow
        for (int i = 0; i < fromShrinkRow.Length; i++)
        {
            fromShrinkRow[i].SelectionChance = fromShrinkRow[i].SelectionChance / fromShrinkRow.Length;
        }

        //From ShrinkRandom
        for (int i = 0; i < fromShrinkRandom.Length; i++)
        {
            fromShrinkRandom[i].SelectionChance = fromShrinkRandom[i].SelectionChance / fromShrinkRandom.Length;
        }
    }

    private BuildProcess ResolveSymbol(ruleWithWeight[] previous)
    {
        //Determine the next rule from the chances provided by the user 
        foreach (var rule in previous)
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) <= rule.SelectionChance)
            {
                return rule.NextRule;
            }
        }

        //If all chances fail, return no chance 
        return BuildProcess.NoChange;
    }
}