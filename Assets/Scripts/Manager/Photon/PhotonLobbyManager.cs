using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonLobbyManager : NormalSingletonPunCallbacks<PhotonLobbyManager> {
    public List<RoomInfo> myList { get; private set; } = new List<RoomInfo>();

    public enum LobbyErrorCode {
        NONE_ERROR,
        NULL_MARBLE,
        NULL_ROOM,
        NULL_NAME_ROOM
    }

    private void Start() {
        myList.Clear();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void Disconnect() {
        DataManager.Instance.SaveData();
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        print("�������");
        PlayFabManager.Instance.Logout();
        SceneManager.LoadScene("01Login");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++) {
            if (!roomList[i].RemovedFromList) {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
    }

    /// <summary>
    /// �κ� ������
    /// </summary>
    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    /// <summary>
    /// �ش� �̸� �� ����
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode CreateRoom(string roomName) {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        PhotonNetwork.CreateRoom(roomName == "" ? "Room" + Random.Range(0, 100) : roomName, new RoomOptions { MaxPlayers = 2 });

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// �ش� �̸� �� ����
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public LobbyErrorCode JoinRoom(string roomName) {
        // �� ���� ���п� ���� �ݹ� �Լ��� UI ó�� �������

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        if (!myList.Exists(x => x.Name == roomName)) {
            return LobbyErrorCode.NULL_NAME_ROOM;
        }

        if (!PhotonNetwork.JoinRoom(roomName)) return LobbyErrorCode.NULL_NAME_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// �ش� �̸� �� ����
    /// ������ ����
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode JoinOrCreateRoom(string roomName) {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// ���� �� ����
    /// </summary>
    public LobbyErrorCode JoinRandomRoom() {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        if (!PhotonNetwork.JoinRandomRoom()) return LobbyErrorCode.NULL_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    public override void OnCreatedRoom() => print("�游���Ϸ�");

    public override void OnJoinedRoom() {
        print("�������Ϸ�");
        SceneManager.LoadScene("03Room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        print("�游������");
        CreateRoom("");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("�淣����������");
        CreateRoom("");
    }

    [ContextMenu("����")]
    private void Info() {
        if (PhotonNetwork.InRoom) {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� �濡 �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }

    protected override void OnApplicationQuit() {
        DataManager.Instance.LocalSaveData();
        base.OnApplicationQuit();
    }
}
