using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonInGameManager : NormalSingletonPunCallbacks<PhotonInGameManager>
{
    /// <summary>
    /// �� ������
    /// </summary>
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        print("�濡�� ����");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        InGameManager.Instance.OtherPlayerEscape();
    }

    public override void OnConnectedToMaster() {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = PlayFabManager.Instance.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("�κ����ӿϷ�");
    }

    protected override void OnApplicationQuit() {
        DataManager.Instance.userData.loseCnt++;
        DataManager.Instance.userData.winScore = (DataManager.Instance.userData.winScore - 5) >= 0 ? DataManager.Instance.userData.winScore - 5 : 0;
        DataManager.Instance.userData.money += 20;
        DataManager.Instance.LocalSaveData();
        base.OnApplicationQuit();
    }
}
