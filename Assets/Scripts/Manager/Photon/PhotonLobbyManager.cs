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
    /// 접속 끊기
    /// </summary>
    public void Disconnect() {
        DataManager.Instance.SaveData();
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        print("연결끊김");
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
    /// 로비 나가기
    /// </summary>
    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    /// <summary>
    /// 해당 이름 방 만듦
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode CreateRoom(string roomName) {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        PhotonNetwork.CreateRoom(roomName == "" ? "Room" + Random.Range(0, 100) : roomName, new RoomOptions { MaxPlayers = 2 });

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 해당 이름 방 입장
    /// </summary>
    /// <param name="roomName"></param>
    /// <returns></returns>
    public LobbyErrorCode JoinRoom(string roomName) {
        // 방 참가 실패에 따른 콜백 함수로 UI 처리 해줘야함

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        if (!myList.Exists(x => x.Name == roomName)) {
            return LobbyErrorCode.NULL_NAME_ROOM;
        }

        if (!PhotonNetwork.JoinRoom(roomName)) return LobbyErrorCode.NULL_NAME_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 해당 이름 방 입장
    /// 없으면 만듬
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode JoinOrCreateRoom(string roomName) {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 랜덤 방 입장
    /// </summary>
    public LobbyErrorCode JoinRandomRoom() {

        if (DataManager.Instance.userData.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }

        if (!PhotonNetwork.JoinRandomRoom()) return LobbyErrorCode.NULL_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    public override void OnCreatedRoom() => print("방만들기완료");

    public override void OnJoinedRoom() {
        print("방참가완료");
        SceneManager.LoadScene("03Room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        print("방만들기실패");
        CreateRoom("");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("방랜덤참가실패");
        CreateRoom("");
    }

    [ContextMenu("정보")]
    private void Info() {
        if (PhotonNetwork.InRoom) {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            print(playerStr);
        }
        else {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }

    protected override void OnApplicationQuit() {
        DataManager.Instance.LocalSaveData();
        base.OnApplicationQuit();
    }
}
