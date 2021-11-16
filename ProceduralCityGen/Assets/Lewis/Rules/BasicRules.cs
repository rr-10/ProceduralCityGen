using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRules : RuleBase
{
  public override Process GetNextProcess(Process input)
  {
    Process toReturn;
    switch (input)
    {
      case Process.NoChange:
        toReturn =  Process.ShrinkColumn;
        break;
      case Process.ShrinkColumn:
        toReturn = Process.NoChange;
        break;
      case Process.ShrinkRow:
        toReturn = Process.ApplyRoof;
        break;
      case Process.ShrinkRandom:
        toReturn =  Process.ApplyRoof;
        break;
      case Process.ApplyRoof:
        toReturn =  Process.ApplyRoof;
        break;
      default:
        toReturn = Process.ApplyRoof;
        break;
    }

    return toReturn;
  }
}
