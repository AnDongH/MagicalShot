using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RuneSkill/EnhancePower", fileName = "MAB_BUF01_RUNE")]
public class EnhancePower : BaseRuneSkill {

    private BasicMarble marble;

    public override IEnumerator Activate(BasicMarble marble) {
        marble.FinalDmg += runeData.value[0];

        yield return new WaitUntil(() => endConditions.Exists(x => x.IsEndable()) == true);
    }

    public override void Enter(BasicMarble marble) {
        base.Enter(marble);
        this.marble = marble;
    }

    public override void Exit(BasicMarble marble) {
        base.Exit(marble);
        marble.FinalDmg -= runeData.value[0];
        this.marble = null;
    }
}
