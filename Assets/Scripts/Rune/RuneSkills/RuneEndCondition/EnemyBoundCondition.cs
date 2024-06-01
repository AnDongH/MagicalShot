using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RuneSkill/EndCondition/EnemyBoundCondition", fileName = "EnemyBoundCondition")]
public class EnemyBoundCondition : RuneEndCondition {

    public int curBoundCount;

    public override bool IsEndable() {
        return curBoundCount <= 0;
    }

    public override void RuneEnter(BasicMarble marble) {
        if (marble != null) marble.OnMarbleEnemyCollision += OnMarbleEnemyCollision;
    }

    public override void RuneExit(BasicMarble marble) {
        if (marble != null) marble.OnMarbleEnemyCollision -= OnMarbleEnemyCollision;
    }

    private void OnMarbleEnemyCollision(BasicMarble marble) {
        curBoundCount--;
    }
}
