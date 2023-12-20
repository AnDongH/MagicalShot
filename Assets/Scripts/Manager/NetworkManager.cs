using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks {

    [Header("���� ����")]
    [SerializeField] Text StatusText;
    [SerializeField] InputField NickNameInput;

    [Header("����,�κ�,�� UI")]
    [SerializeField] GameObject mainPannel;
    [SerializeField] GameObject lobbyPannel;
    [SerializeField] GameObject roomPannel;

    [Header("�κ� ����")]
    // room ����
    [SerializeField] GameObject marbleSelectUI;
    [SerializeField] InputField roomInput;
    [SerializeField] Text welcomeText;
    [SerializeField] Text lobbyInfoText;
    [SerializeField] Button[] cellBtn;
    [SerializeField] Button previousBtn;
    [SerializeField] Button nextBtn;

    [Header("��� �� ����")]
    [SerializeField] Button startBtn;
    [SerializeField] Text playerCnt;

    [Header("���� ����")]
    [SerializeField] Text nickName;
    [SerializeField] Text roomName;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;


    void Awake() {
        Screen.SetResolution(960, 540, false);
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
        lobbyInfoText.text = "�κ� : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�� / ������ : " + PhotonNetwork.CountOfPlayers + "��";
    }



    public void Connect() {

        if (NickNameInput.text == "") {
            print("�г��� �Է����ּ���");
            return;
        }

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinLobby();
        nickName.text = PhotonNetwork.LocalPlayer.NickName;
        mainPannel.SetActive(false);
        lobbyPannel.SetActive(true);
        MyListRenewal();
    }



    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) {
        print("�������");
        lobbyPannel.SetActive(false);
        mainPannel.SetActive(true);
    }

    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    public override void OnJoinedLobby() {
        print("�κ����ӿϷ�");
        mainPannel.SetActive(false); 
        lobbyPannel.SetActive(true);
        myList.Clear();
    }


    public void CreateRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRoom() {


        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinRoom(roomInput.text);
    }

    public void JoinOrCreateRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public void JoinRandomRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom() {

        SettingData.Instance.marbleDeck.Clear();

        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom() => print("�游���Ϸ�");

    public override void OnJoinedRoom() {
        print("�������Ϸ�");
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        startBtn.interactable = false;
        lobbyPannel.SetActive(false);
        roomPannel.SetActive(true);


        photonView.RPC("UpdateRoom", RpcTarget.All);
    }

    public override void OnLeftRoom() {
        print("�濡�� ����");
        roomPannel.SetActive(false);
        lobbyPannel.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        UpdateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        print("�游������");
        roomInput.text = "";
        CreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("�淣����������");
        roomInput.text = "";
        CreateRoom();
    }


    [ContextMenu("����")]
    void Info() {
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

    // ����ư -2 , ����ư -1 , �� ����
    public void MyListClick(int num) {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else {

            if (SettingManager.Instance.SelectCount != 4) {
                print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
                return;
            }
            SettingManager.Instance.SendMarbleData();

            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }
        MyListRenewal();
    }

    void MyListRenewal() {
        // �ִ�������
        maxPage = (myList.Count % cellBtn.Length == 0) ? myList.Count / cellBtn.Length : myList.Count / cellBtn.Length + 1;

        // ����, ������ư
        previousBtn.interactable = (currentPage <= 1) ? false : true;
        nextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
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

    [PunRPC]
    void CheckGameStart() {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {
           
            if (PhotonNetwork.IsMasterClient) startBtn.interactable = true;

        }
        else {
            if (PhotonNetwork.IsMasterClient) startBtn.interactable = false;
        }
    }

    [PunRPC]
    void UpdateRoom() {
        playerCnt.text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        CheckGameStart();
    } 

    public void GameStart() {
        print("���� ��ŸƮ");
        photonView.RPC("LevelLoadPRC", RpcTarget.All);
    }

    [PunRPC]
    void LevelLoadPRC() {
        PhotonNetwork.LoadLevel(1);
    }

}