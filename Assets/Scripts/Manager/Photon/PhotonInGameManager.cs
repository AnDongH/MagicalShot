using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonInGameManager : NormalSingletonPunCallbacks<PhotonInGameManager>
{
    /// <summary>
    /// 방 나가기
    /// </summary>
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        print("방에서 나감");
    }

    public override void OnConnectedToMaster() {
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = PlayFabManager.Instance.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("로비접속완료");
    }
}
