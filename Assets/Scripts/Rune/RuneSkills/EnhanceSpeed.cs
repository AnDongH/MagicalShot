using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "RuneSkill/EnhanceSpeed", fileName = "MAB_BUF02_RUNE")]
public class EnhanceSpeed : BaseRuneSkill {

    private BasicMarble marble;

    public override IEnumerator Activate(BasicMarble marble) {
        marble.AdditionalSpeed += runeData.value[0];

        yield return new WaitUntil(() => endConditions.Exists(x => x.IsEndable()) == true);
    }

    public override void Enter(BasicMarble marble) {
        base.Enter(marble);
        this.marble = marble;
    }

    public override void Exit(BasicMarble marble) {
        base.Exit(marble);
        marble.AdditionalSpeed -= runeData.value[0];
        this.marble = null;
    }
}
