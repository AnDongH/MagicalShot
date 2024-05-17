using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLoginManager : NormalSingletonPunCallbacks<PhotonLoginManager> {
    protected override void Awake() {
        base.Awake();
    }

    /// <summary>
    /// 접속
    /// </summary>
    public void Connect() {
        PhotonNetwork.ConnectUsingSettings();
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
