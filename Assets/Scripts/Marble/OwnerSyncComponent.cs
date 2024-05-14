using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OwnerSyncComponent : MonoBehaviourPun
{
    // �� �����Ҷ� Ŭ��� ȣ��Ʈ ���ο��� �������� �˷���� �Ѵ�.
    [field: SerializeField] public bool IsHost { get; private set; }

    private void Start() {
        TurnManager.OnTurnChanged += ChangeOwner;
    }

    private void OnDestroy() {
        TurnManager.OnTurnChanged -= ChangeOwner;
    }


    // ����ȭ ������ ���� ����ȭ
    [PunRPC]
    private void Change(bool flag) {
        if (flag)
            photonView.TransferOwnership(InGameManager.Instance.HostID);
        else
            photonView.TransferOwnership(InGameManager.Instance.GuestID);

        /*
         * AŬ�󿡼� �⹰�� �����̰�, B���� �� ���Ḧ �ϸ� �ȵǰ� ������ A���� ���Ḧ ����� �Ѵ�.
         * ���� �׷��� ���ϸ�, �������� ���ʿ� B�� �����, ������ ������ �ȵȴ�.
         */


    }

    // �⹰ ������ ����ȭ
    [PunRPC]
    private void Set(bool flag) {
        IsHost = flag;
    }

    // ����ȭ ������ ����
    public void ChangeOwner(bool flag) {
        Change(flag);
    }

    // �⹰ ������ ����
    public void SetControlOwner(bool flag) {
        photonView.RPC("Set", RpcTarget.All, flag);
    }
}
