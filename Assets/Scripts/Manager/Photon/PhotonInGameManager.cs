using Photon.Pun;
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

    public override void OnConnectedToMaster() {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = PlayFabManager.Instance.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("�κ����ӿϷ�");
    }
}
