using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRuneSkill : ScriptableObject
{
    public enum RuneState {
        Ready,
        Activate,
        Used
    }

    [Header("Rune Info")]
    public RuneData runeData;
    public RuneState runeState;
    public int curBoundCount;

    [Header("Rune End Info")]
    public List<RuneEndCondition> endConditionsSO;

    public List<RuneEndCondition> endConditions { get; private set; }

    public virtual void Init(RuneData data) {
        runeData = data;
        curBoundCount = runeData.boundCount;

        endConditions = new List<RuneEndCondition>();

        foreach (var condition in endConditionsSO) {
            endConditions.Add(Instantiate(condition));
        }
    }
    public virtual void Enter(BasicMarble marble) {
        foreach (var condition in endConditions) condition.RuneEnter(marble);
    }
    public abstract IEnumerator Activate(BasicMarble marble);
    public virtual void Exit(BasicMarble marble) {
        foreach (var condition in endConditions) condition.RuneExit(marble);
        foreach (var condition in endConditions) Destroy(condition);
    }
    public virtual void ActOnUpdate() {

    }
    public virtual void ActOnFixedUpdate() {

    }
}
