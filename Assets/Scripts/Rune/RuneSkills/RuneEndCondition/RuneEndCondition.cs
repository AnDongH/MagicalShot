using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RuneEndCondition : ScriptableObject
{
    public abstract void RuneEnter(BasicMarble marble);
    public abstract bool IsEndable();
    public abstract void RuneExit(BasicMarble marble);
}
