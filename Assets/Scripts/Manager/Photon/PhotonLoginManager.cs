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
    /// ����
    /// </summary>
    public void Connect() {
        GameManager.ShowLoadingUI();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        GameManager.CloseLoadingUI();
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = PlayFabManager.Instance.NickName;
        PhotonNetwork.JoinLobby();

    }


    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("�κ����ӿϷ�");
    }
}
