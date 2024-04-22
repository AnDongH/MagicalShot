using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks {

    [Header("���� ����")]
    [SerializeField] Text nickName;

    public List<RoomInfo> myList { get; private set; } = new List<RoomInfo>();

    public enum LobbyErrorCode {
        NONE_ERROR,
        NULL_MARBLE,
        NULL_ROOM,
        NULL_NAME_ROOM
    }

    // ���� �Ŵ��� ���� �̱���
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
    /// ����
    /// </summary>
    public void Connect() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        print("�������ӿϷ�");
        PhotonNetwork.LocalPlayer.NickName = GetComponent<PlayFabManager>().nickName;
        PhotonNetwork.JoinLobby();
    }


    /// <summary>
    /// ���� ����
    /// </summary>
    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause) {
        print("�������");
    }

    public override void OnJoinedLobby() {
        SceneManager.LoadScene("02Lobby");
        print("�κ����ӿϷ�");
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
    /// �κ� ������
    /// </summary>
    public void LeaveLobby() => PhotonNetwork.LeaveLobby();

    /// <summary>
    /// �ش� �̸� �� ����
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode CreateRoom(string roomName) {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

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

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        if (!PhotonNetwork.JoinRoom(roomName)) return LobbyErrorCode.NULL_NAME_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// �ش� �̸� �� ����
    /// ������ ����
    /// </summary>
    /// <param name="roomName"></param>
    public LobbyErrorCode JoinOrCreateRoom(string roomName) {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2 }, null);

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// ���� �� ����
    /// </summary>
    public LobbyErrorCode JoinRandomRoom() {

        if (SettingManager.Instance.SelectCount != 4) {
            print("�⹰�� 4�� �����ؾ� �濡 ������ �� �ֽ��ϴ�.");
            return LobbyErrorCode.NULL_MARBLE;
        }
        SettingManager.Instance.SendMarbleData();

        if (!PhotonNetwork.JoinRandomRoom()) return LobbyErrorCode.NULL_ROOM;

        return LobbyErrorCode.NONE_ERROR;
    }

    /// <summary>
    /// �� ������
    /// </summary>
    public void LeaveRoom() {

        SettingData.Instance.marbleDeck.Clear();

        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom() => print("�游���Ϸ�");

    public override void OnJoinedRoom() {
        print("�������Ϸ�");
        SceneManager.LoadScene("03Room");
    }

    public override void OnLeftRoom() {
        print("�濡�� ����");
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
        print("�游������");
        CreateRoom("");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) => print("����������");

    public override void OnJoinRandomFailed(short returnCode, string message) {
        print("�淣����������");
        CreateRoom("");
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

    private void CheckGameStart() {
        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {
           
            if (PhotonNetwork.IsMasterClient && isReady) startBtn.interactable = true;
            else startBtn.interactable = false;

        }

    }



    public void GameStart() {
        print("���� ��ŸƮ");
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

    // �� ä��
    private void Send() {
        photonView.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    void ChatRPC(string msg) {
        bool isInput = false;
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "") {
                isInput = true;
                chatText[i].text = msg;
                break;
            }
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

}