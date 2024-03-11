using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks {

    [Header("개인 정보")]
    [SerializeField] Text StatusText;
    [SerializeField] InputField NickNameInput;

    [Header("서버,로비,방 UI")]
    [SerializeField] GameObject mainPannel;
    [SerializeField] GameObject lobbyPannel;
    [SerializeField] GameObject roomPannel;

    [Header("로비 정보")]
    // room 관련
    [SerializeField] GameObject marbleSelectUI;
    [SerializeField] InputField roomInput;
    [SerializeField] Button[] cellBtn;
    [SerializeField] Button previousBtn;
    [SerializeField] Button nextBtn;

    [Header("대기 룸 정보")]
    [SerializeField] Button startBtn;
    [SerializeField] Text readyText;
    [SerializeField] Text[] chatText;
    [SerializeField] InputField chatInput;
    [SerializeField] Text hostName;
    [SerializeField] Text guestName;
    [SerializeField] Text roomName;
    [SerializeField] Text mapName;

    [Header("개인 정보")]
    [SerializeField] Text nickName;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    private bool isReady = false;


    void Awake() {
        Screen.SetResolution(800, 450, false);
        mainPannel.SetActive(true);
        lobbyPannel.SetActive(false);
        roomPannel.SetActive(false);
        marbleSelectUI.SetActive(false);
    } 

    void Start() {
        if (PlayerPrefs.HasKey("mainScene")) {
            LeaveRoom();
            NickNameInput.text = PhotonNetwork.LocalPlayer.NickName;
            mainPannel.SetActive(false);
            lobbyPannel.SetActive(true);
            MyListRenewal();
            PlayerPrefs.DeleteKey("mainScene");
        }



    }

    void Update() {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();

        if (PhotonNetwork.InRoom) {
            if (Input.GetKeyDown(KeyCode.Return) && chatInput.text != "") Send();
        }
    }



    public void Connect() {

        if (NickNameInput.text == "") {
            print("닉네임 입력해주세요");
            return;
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinLobby();
        nickName.text = PhotonNetwork.LocalPlayer.NickName;
        mainPannel.SetActive(false);
        lobbyPannel.SetActive(true);
        MyListRenewal();
    }



    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) {
        print("연결끊김");
        lobbyPannel.SetActive(false);
        mainPannel.SetActive(true);
    }

    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    public override void OnJoinedLobby() {
        print("로비접속완료");
        mainPannel.SetActive(false); 
        lobbyPannel.SetActive(true);
        myList.Clear();
    }


    public void CreateRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRoom() {


        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinRoom(roomInput.text);
    }

    public void JoinOrCreateRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public void JoinRandomRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom() {

        SettingData.Instance.marbleDeck.Clear();

        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom() => print("방만들기완료");

    public override void OnJoinedRoom() {
        isReady = false;
        print("방참가완료");
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        lobbyPannel.SetActive(false);
        roomPannel.SetActive(true);

        if (PhotonNetwork.IsMasterClient) {
            hostName.text = PhotonNetwork.LocalPlayer.NickName; ;
            guestName.text = "";
            startBtn.interactable = false;
            readyText.text = "Start";
            startBtn.onClick.RemoveAllListeners();
            startBtn.onClick.AddListener(() => GameStart());
        }
        else {
            hostName.text = PhotonNetwork.MasterClient.NickName;
            guestName.text = PhotonNetwork.LocalPlayer.NickName;
            startBtn.interactable = true;
            readyText.text = "Ready";
            startBtn.onClick.RemoveAllListeners();
            startBtn.onClick.AddListener(() => GetReady(isReady));
        }
    }

    public override void OnLeftRoom() {
        isReady = false;
        print("방에서 나감");
        roomPannel.SetActive(false);
        lobbyPannel.SetActive(true);
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
        roomInput.text = "";
        CreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => print("방참가실패");

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("방랜덤참가실패");
        roomInput.text = "";
        CreateRoom();
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

    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num) {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else {

            if (SettingManager.Instance.SelectCount != 4) {
                print("기물을 4개 선택해야 방에 참가할 수 있습니다.");
                return;
            }
            SettingManager.Instance.SendMarbleData();

            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }
        MyListRenewal();
    }

    void MyListRenewal() {
        // 최대페이지
        maxPage = (myList.Count % cellBtn.Length == 0) ? myList.Count / cellBtn.Length : myList.Count / cellBtn.Length + 1;

        // 이전, 다음버튼
        previousBtn.interactable = (currentPage <= 1) ? false : true;
        nextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * cellBtn.Length;
        for (int i = 0; i < cellBtn.Length; i++) {
            cellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            cellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            cellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
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
        MyListRenewal();
    }

    private void CheckGameStart() {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {
           
            if (PhotonNetwork.IsMasterClient && isReady) startBtn.interactable = true;
            else startBtn.interactable = false;

        }

    }



    private void GameStart() {
        print("게임 스타트");
        photonView.RPC("LevelLoadPRC", RpcTarget.All);
    }

    private void GetReady(bool flag) {
        isReady = !flag;
        readyText.text = isReady ? "Wait" : "Ready";
        photonView.RPC("SetReadyInfo", RpcTarget.Others, isReady);
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