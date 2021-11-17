using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRules : RuleBase
{
  public override BuildProcess GetNextProcess(BuildProcess input)
  {
    BuildProcess toReturn;
    switch (input)
    {
      case BuildProcess.NoChange:
        toReturn =  BuildProcess.ShrinkColumn;
        break;
      case BuildProcess.ShrinkColumn:
        toReturn = BuildProcess.NoChange;
        break;
      case BuildProcess.ShrinkRow:
        toReturn = BuildProcess.ApplyRoof;
        break;
      case BuildProcess.ShrinkRandom:
        toReturn =  BuildProcess.ApplyRoof;
        break;
      case BuildProcess.ApplyRoof:
        toReturn =  BuildProcess.ApplyRoof;
        break;
      default:
        toReturn = BuildProcess.ApplyRoof;
        break;
    }

    return toReturn;
  }
}
