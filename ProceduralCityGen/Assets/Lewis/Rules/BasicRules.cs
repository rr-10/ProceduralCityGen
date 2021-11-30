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