using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyUI : UI_Scene
{
    enum Buttons {
        OptionBtn,
        SelectRunesBtn,
        SelectMarblesBtn,
        DisConnectBtn,
        RankingBtn,
        ShopBtn,
        EnterRoomBtn,
        PreviousBtn,
        NextBtn,
        UpdateRoomBtn,
        CreateRoomBtn,
        EnterRandomBtn,
        RoomCell1,
        RoomCell2,
        RoomCell3,
        RoomCell4,
        WarningOkBtn
    }

    enum InputFields {
        RoomInput
    }

    enum Texts {
        RoomInput_Text,
        PlayerNameText,
        WinLoseText,
        ScoreText,
        WarningText
    }

    enum Objects {
        WarningGRP
    }

    private int maxPage;
    private int currentPage = 1;
    private int multiple;
    private Button[] cellBtns = new Button[4];

    private PhotonLobbyManager lobbyManager;

    protected override void Init() {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));

        GetButton((int)Buttons.OptionBtn).gameObject.BindEvent(OnOptionBtnClicked);
        GetButton((int)Buttons.SelectRunesBtn).gameObject.BindEvent(OnSelectRunesBtnClicked);
        GetButton((int)Buttons.SelectMarblesBtn).gameObject.BindEvent(OnSelectMarblesBtnClicked);
        GetButton((int)Buttons.DisConnectBtn).gameObject.BindEvent(OnDisConnectBtnClicked);
        GetButton((int)Buttons.RankingBtn).gameObject.BindEvent(RankingBtnClicked);
        GetButton((int)Buttons.EnterRoomBtn).gameObject.BindEvent(OnEnterRoomBtnClicked);
        GetButton((int)Buttons.CreateRoomBtn).gameObject.BindEvent(OnCreateRoomBtnClicked);
        GetButton((int)Buttons.EnterRandomBtn).gameObject.BindEvent(OnEnterRandomBtnClicked);

        GetButton((int)Buttons.PreviousBtn).gameObject.BindEvent((data) => OnCellButtonClicked(data, -2));
        GetButton((int)Buttons.NextBtn).gameObject.BindEvent((data) => OnCellButtonClicked(data, -1));
        GetButton((int)Buttons.RoomCell1).gameObject.BindEvent((data) => OnCellButtonClicked(data, 0));
        GetButton((int)Buttons.RoomCell2).gameObject.BindEvent((data) => OnCellButtonClicked(data, 1));
        GetButton((int)Buttons.RoomCell3).gameObject.BindEvent((data) => OnCellButtonClicked(data, 2));
        GetButton((int)Buttons.RoomCell4).gameObject.BindEvent((data) => OnCellButtonClicked(data, 3));
        GetButton((int)Buttons.UpdateRoomBtn).gameObject.BindEvent(OnUpdateRoomBtnClicked);
        GetButton((int)Buttons.WarningOkBtn).gameObject.BindEvent(OnWarningOkBtnClicked);


        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
        cellBtns[0] = GetButton((int)Buttons.RoomCell1);
        cellBtns[1] = GetButton((int)Buttons.RoomCell2);
        cellBtns[2] = GetButton((int)Buttons.RoomCell3);
        cellBtns[3] = GetButton((int)Buttons.RoomCell4);
    }

    private void Start() {
        lobbyManager = PhotonLobbyManager.Instance;

        Init();
        
        // �̸� �ʱ�ȭ
        GetText((int)Texts.PlayerNameText).text = PhotonNetwork.LocalPlayer.NickName;
        GetText((int)Texts.ScoreText).text = "����: " + DataManager.Instance.userData.winScore.ToString();
        GetText((int)Texts.WinLoseText).text = "����: " + DataManager.Instance.userData.winCnt.ToString() + "�� " + DataManager.Instance.userData.loseCnt.ToString() + "��";
        
        // �� ����Ʈ �ʱ�ȭ
        MyListRenewal();

        SoundManager.Instance.PlayBackGroundSound("MainTheme");
    }

    private void OnOptionBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("MenuBar");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("OptionCanvas");
    }

    private void OnSelectRunesBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("MenuBar");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("RuneCanvas");
    }

    private void OnSelectMarblesBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("MenuBar");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("MarbleCanvas");
    }

    private void OnDisConnectBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("MenuBar");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("ExitPanelUI");
    }

    private void OnStoryModeBtnClicked(PointerEventData data) {
        //TODO
    }

    private void RankingBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("MenuBar");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("RankingUI");
        PlayFabManager.Instance.GetLeaderboard();
    }

    // �ݹ� �ʿ�
    private void OnEnterRoomBtnClicked(PointerEventData data) {
        PhotonLobbyManager.LobbyErrorCode code = lobbyManager.JoinRoom(GetInputField((int)InputFields.RoomInput).text);
        LobbyErrorExeption(code);
    }

    private void OnCreateRoomBtnClicked(PointerEventData data) {
        PhotonLobbyManager.LobbyErrorCode code = lobbyManager.CreateRoom(GetInputField((int)InputFields.RoomInput).text);
        LobbyErrorExeption(code);
    }

    // �ݹ� �ʿ�
    private void OnEnterRandomBtnClicked(PointerEventData data) {
        PhotonLobbyManager.LobbyErrorCode code = lobbyManager.JoinRandomRoom();
        LobbyErrorExeption(code);
    }

    private void OnCellButtonClicked(PointerEventData data, int i) {
        MyCellClick(i);
    }

    private void OnUpdateRoomBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        MyListRenewal();
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
        MyListRenewal();
    }

    private void WarningOn(string error) {
        SoundManager.Instance.PlaySFXSound("Warning");
        GetText((int)Texts.WarningText).text = error;
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(true);
    }

    // ����ư -2 , ����ư -1 , �� ����
    /// <summary>
    /// ����Ʈ�� �ִ� �� Ŭ��
    /// </summary>
    /// <param name="i"></param>
    public void MyCellClick(int i) {
        if (i == -2) {
            --currentPage;
            SoundManager.Instance.PlaySFXSound("ButtonClick");
        }
        else if (i == -1) {
            ++currentPage;
            SoundManager.Instance.PlaySFXSound("ButtonClick");
        }
        else {
            // ���⿡ �� ���°� �������� �� �˾� �ߵ���
            PhotonLobbyManager.LobbyErrorCode code = lobbyManager.JoinRoom(lobbyManager.myList[multiple + i].Name);
            LobbyErrorExeption(code);
        }
        MyListRenewal();
    }

    /// <summary>
    /// �� ����Ʈ ������Ʈ
    /// </summary>
    private void MyListRenewal() {
        // �ִ�������
        maxPage = (lobbyManager.myList.Count % cellBtns.Length == 0) ? lobbyManager.myList.Count / cellBtns.Length : lobbyManager.myList.Count / cellBtns.Length + 1;

        // ����, ������ư
        GetButton((int)Buttons.PreviousBtn).interactable = (currentPage <= 1) ? false : true;
        GetButton((int)Buttons.NextBtn).interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * cellBtns.Length;
        for (int i = 0; i < cellBtns.Length; i++) {
            cellBtns[i].interactable = (multiple + i < lobbyManager.myList.Count) ? true : false;
            cellBtns[i].gameObject.SetInteractable(cellBtns[i].interactable);

            cellBtns[i].transform.GetChild(0).GetComponent<Text>().text = 
                (multiple + i < lobbyManager.myList.Count) ? 
                 lobbyManager.myList[multiple + i].Name : "";

            cellBtns[i].transform.GetChild(1).GetComponent<Text>().text = 
                (multiple + i < lobbyManager.myList.Count) ? 
                lobbyManager.myList[multiple + i].PlayerCount + "/" + lobbyManager.myList[multiple + i].MaxPlayers : "";
        }
    }

    private void LobbyErrorExeption(PhotonLobbyManager.LobbyErrorCode code) {
        switch (code) {
            case PhotonLobbyManager.LobbyErrorCode.NONE_ERROR:
                SoundManager.Instance.PlaySFXSound("EnterRoom");
                break;
            case PhotonLobbyManager.LobbyErrorCode.NULL_MARBLE:
                WarningOn("�⹰�� 4�� �������ּ���");
                break;
            case PhotonLobbyManager.LobbyErrorCode.NULL_ROOM:
                WarningOn("������ ���� �������� �ʽ��ϴ�.");
                break;
            case PhotonLobbyManager.LobbyErrorCode.NULL_NAME_ROOM:
                WarningOn("�ش� �̸� ���� �����ϴ�.");
                break;
        }
    }
}
