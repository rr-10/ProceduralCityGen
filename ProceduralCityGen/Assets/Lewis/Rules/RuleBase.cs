using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleBase : ScriptableObject
{
    public abstract BuildProcess GetNextProcess(BuildProcess input);
}
