using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDMarble : BasicMarble
{
    [PunRPC]
    protected override void RegisterRune_RPC(string runeDataSer) {
        base.RegisterRune_RPC(runeDataSer);
    }
}
