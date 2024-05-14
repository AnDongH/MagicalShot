using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonRoomManager : NormalSingletonPunCallbacks<PhotonRoomManager> {
    #region room delegate
    public delegate void OnReadyHandler();
    public delegate void OnMasterClientSwitchedHandler(Player newMasterClient);
    public delegate void OnEnterHandler(Player newPlayer);
    public delegate void OnLeftHandler(Player otherPlayer);
    public delegate void OnChatSendHandler(string msg);
    #endregion

    public bool isReady;

    #region room event
    public static event OnReadyHandler OnReady;
    public static event OnMasterClientSwitchedHandler OnClientSwitched;
    public static event OnEnterHandler OnEntered;
    public static event OnLeftHandler OnLefted;
    public static event OnChatSendHandler OnChatSended;
    #endregion

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

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        OnLefted.Invoke(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        OnClientSwitched.Invoke(newMasterClient);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        OnEntered.Invoke(newPlayer);
    }

    public void GameStart() {
        print("���� ��ŸƮ");
        photonView.RPC("LevelLoadPRC", RpcTarget.All);
    }

    public void GetReady(Text readyText) {
        isReady = !isReady;
        readyText.text = isReady ? "Wait" : "Ready";
        photonView.RPC("SetReadyInfo", RpcTarget.Others, isReady);
    }

    [PunRPC]
    private void SetReadyInfo(bool flag) {
        isReady = flag;
        OnReady.Invoke();
    }

    [PunRPC]
    private void LevelLoadPRC() {
        PhotonNetwork.LoadLevel(3);
    }

    // �� ä��
    public void Send(InputField input, Text[] texts) {
        photonView.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + input.text);
        input.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    private void ChatRPC(string msg) {
        OnChatSended.Invoke(msg);
    }
}
