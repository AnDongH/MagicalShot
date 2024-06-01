using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RuneSkill/EndCondition/BoundCondition", fileName = "BoundCondition")]
public class BoundCondition : RuneEndCondition {

    public int curBoundCount;

    public override bool IsEndable() {
        return curBoundCount <= 0;
    }

    public override void RuneEnter(BasicMarble marble) {
        if (marble != null) marble.OnMarbleCollision += OnMarbleCollision;
    }

    public override void RuneExit(BasicMarble marble) {
        if (marble != null) marble.OnMarbleCollision -= OnMarbleCollision;
    }

    private void OnMarbleCollision(BasicMarble marble) {
        curBoundCount--;
    }
}
