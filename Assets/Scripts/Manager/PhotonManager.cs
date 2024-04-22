using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks {

    [Header("개인 정보")]
    [SerializeField] Text nickName;

    public List<RoomInfo> myList { get; private set; } = new List<RoomInfo>();

    public enum LobbyErrorCode {
        NONE_ERROR,
        NULL_MARBLE,
        NULL_ROOM,
        NULL_NAME_ROOM
    }

    // 포톤 매니져 전용 싱글톤
    private static PhotonManager _instance;
    public static PhotonManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<PhotonManager>();

                if (_instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PhotonManager).Name;
                    _instance = obj.AddComponent<PhotonManager>();
                }
            }

            return _instance;
        }
    }


    void Awake() {
        Screen.SetResolution(1600, 900, false);

        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    } 

    void Start() {
        if (PlayerPrefs.HasKey("mainScene")) {
            LeaveRoom();
            //nickName.text = PhotonNetwork.LocalPlayer.NickName;
           // MyListRenewal();
            PlayerPrefs.DeleteKey("mainScene");
        }
    }

    void Update() {
        if (PhotonNetwork.InRoom) {
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "") Send();
        }
    }


    /// <summary>
    /// 접속
    /// </summary>
    public void Connect() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = GetComponent<PlayFabManager>().nickName;
        PhotonNetwork.JoinLobby();
    }


    /// <summary>
    /// 접속 끊기
    /// </summary>
    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) {
        print("연결끊김");
    }

    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("로비접속완료");
        myList.Clear();
    }

    public override void OnLeftLobby() {

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

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

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

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        if (!PhotonNetwork.JoinRoom(roomName)) return LobbyErrorCode.NULL_NAME_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 해당 이름 방 입장
    /// 없으면 만듬
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode JoinOrCreateRoom(string roomName) {

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 랜덤 방 입장
    /// </summary>
    public LobbyErrorCode JoinRandomRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        if (!PhotonNetwork.JoinRandomRoom()) return LobbyErrorCode.NULL_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// 방 나가기
    /// </summary>
    public void LeaveRoom() {

        SettingData.Instance.marbleDeck.Clear();

        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom() => print("방만들기완료");

    public override void OnJoinedRoom() {
        print("방참가완료");
        SceneManager.LoadScene("03Room");
    }

    public override void OnLeftRoom() {
        print("방에서 나감");
        SceneManager.LoadScene("02Lobby");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {

        if (!PhotonNetwork.IsMasterClient) return;

        isReady = false;
        guestName.text = "";
        startBtn.interactable = false;

    }

    public override void OnMasterClientSwitched(Player newMasterClient) {

        if (PhotonNetwork.MasterClient != newMasterClient) return;

        hostName.text = newMasterClient.NickName;
        guestName.text = "";
        startBtn.interactable = false;
        readyText.text = "Start";
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(() => GameStart());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        guestName.text = newPlayer.NickName;
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
    void Info() {
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

    private void CheckGameStart() {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {
           
            if (PhotonNetwork.IsMasterClient && isReady) startBtn.interactable = true;
            else startBtn.interactable = false;

        }

    }



    public void GameStart() {
        print("게임 스타트");
        photonView.RPC("LevelLoadPRC", RpcTarget.All);
    }

    public bool GetReady(Text readyText, bool flag) {
        bool isReady = !flag;
        readyText.text = isReady ? "Wait" : "Ready";
        photonView.RPC("SetReadyInfo", RpcTarget.Others, isReady);
        return isReady;
    }

    [PunRPC]
    private void SetReadyInfo(bool flag) {
        isReady = flag;
        CheckGameStart();
    }

    [PunRPC]
    void LevelLoadPRC() {
        PhotonNetwork.LoadLevel(1);
    }

    // 룸 채팅
    private void Send() {
        photonView.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg) {
        bool isInput = false;
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "") {
                isInput = true;
                chatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

}