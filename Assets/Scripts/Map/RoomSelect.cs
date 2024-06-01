using Photon.Pun;
using Photon.Realtime;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class RoomSelect : NormalSingletonPunCallbacks<RoomSelect> {

    public delegate void OnMapButtonClickedHandler();
    public static event OnMapButtonClickedHandler OnMapButtonClicked;

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        photonView.RPC("RPCMapUpdate", RpcTarget.All, DataManager.Instance.mapIndex);
    }

    public void MapUpdate(bool flag) {
        if (flag) DataManager.Instance.mapIndex++;
        else DataManager.Instance.mapIndex--;
        DataManager.Instance.mapIndex %= DataManager.Instance.Resource.maps.Count;
        if (DataManager.Instance.mapIndex < 0) DataManager.Instance.mapIndex *= (-1);
        photonView.RPC("RPCMapUpdate", RpcTarget.All, DataManager.Instance.mapIndex);
    }

    [PunRPC]
    private void RPCMapUpdate(int i) {
        DataManager.Instance.mapIndex = i;
        OnMapButtonClicked?.Invoke();
    }

}
