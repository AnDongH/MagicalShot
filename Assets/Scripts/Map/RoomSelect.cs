using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class RoomSelect : NormalSingletonPun<RoomSelect> {

    public delegate void OnMapButtonClickedHandler();
    public static event OnMapButtonClickedHandler OnMapButtonClicked;

    public void MapUpdate(bool flag) {
        photonView.RPC("RPCMapUpdate", RpcTarget.All, flag);
    }

    [PunRPC]
    private void RPCMapUpdate(bool flag) {
        if (flag) DataManager.Instance.mapIndex++;
        else DataManager.Instance.mapIndex--;
        DataManager.Instance.mapIndex %= DataManager.Instance.Resource.maps.Count;
        if (DataManager.Instance.mapIndex < 0) DataManager.Instance.mapIndex *= (-1);

        OnMapButtonClicked?.Invoke();
    }

}
