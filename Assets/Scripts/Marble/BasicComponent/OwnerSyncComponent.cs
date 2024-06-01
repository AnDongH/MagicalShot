using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OwnerSyncComponent : MonoBehaviourPun
{
    // 말 생성할때 클라와 호스트 전부에게 소유권을 알려줘야 한다.
    [field: SerializeField] public bool IsHost { get; private set; }

    private void Start() {
        TurnManager.OnTurnChanged += ChangeOwner;
    }

    private void OnDestroy() {
        TurnManager.OnTurnChanged -= ChangeOwner;
    }


    // 동기화 소유권 변경 동기화
    [PunRPC]
    private void Change(bool flag) {
        if (flag)
            photonView.TransferOwnership(InGameManager.Instance.HostID);
        else
            photonView.TransferOwnership(InGameManager.Instance.GuestID);

        /*
         * A클라에서 기물을 움직이고, B에서 턴 종료를 하면 안되고 무조건 A에서 종료를 해줘야 한다.
         * 만약 그렇게 안하면, 소유권이 애초에 B에 없었어서, 소유권 이전이 안된다.
         */


    }

    // 기물 소유권 동기화
    [PunRPC]
    private void Set(bool flag) {
        IsHost = flag;
    }

    // 동기화 소유권 변경
    public void ChangeOwner(bool flag) {
        Change(flag);
    }

    // 기물 소유권 설정
    public void SetControlOwner(bool flag) {
        photonView.RPC("Set", RpcTarget.All, flag);
    }
}
