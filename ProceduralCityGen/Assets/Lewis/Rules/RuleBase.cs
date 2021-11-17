using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RuleBase : MonoBehaviour
{
    public abstract BuildProcess GetNextProcess(BuildProcess input);
}
