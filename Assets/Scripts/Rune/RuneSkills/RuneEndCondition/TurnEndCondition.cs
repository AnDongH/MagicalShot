using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuneSkill/EndCondition/TurnEndCondition", fileName = "TurnEndCondition")]
public class TurnEndCondition : RuneEndCondition {

    public int curTurnCount;

    public override bool IsEndable() {
        return curTurnCount <= 0;
    }

    private void OnTurnEnded(bool flag) {
        curTurnCount--;
    }

    public override void RuneEnter(BasicMarble marble) {
        TurnManager.OnTurnEnded += OnTurnEnded;
    }

    public override void RuneExit(BasicMarble marble) {
        TurnManager.OnTurnEnded -= OnTurnEnded;
    }
}
