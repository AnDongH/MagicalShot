using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomUI : UI_Scene
{
    enum Buttons {
        ExitRoomBtn,
        GameStartBtn
    }

    enum InputFields {
        ChatInputField
    }

    enum Texts {
        GameStartBtnText,
        HostNameText,
        GuestNameText,
        RoomNameText,
        MapNameText
    }

    enum Objects {
        TextContent
    }


    private Text[] texts;

    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));

        texts = GetObject((int)Objects.TextContent).GetComponentsInChildren<Text>();
        GetButton((int)Buttons.ExitRoomBtn).gameObject.BindEvent(OnExitRoomBtnClicked);
    }

    private void Start() {
        Init();
        // 방 초기화
        PhotonRoomManager.Instance.isReady = false;
        GetText((int)Texts.RoomNameText).text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient) {
            GetText((int)Texts.HostNameText).text = PhotonNetwork.LocalPlayer.NickName; ;
            GetText((int)Texts.GuestNameText).text = "";
            GetButton((int)Buttons.GameStartBtn).interactable = false;
            GetButton((int)Buttons.GameStartBtn).gameObject.SetInteractable(GetButton((int)Buttons.GameStartBtn).interactable);

            GetText((int)Texts.GameStartBtnText).text = "Start";
            GetButton((int)Buttons.GameStartBtn).gameObject.BindEvent(OnStartBtnClicked);
        }
        else {
            GetText((int)Texts.HostNameText).text = PhotonNetwork.MasterClient.NickName;
            GetText((int)Texts.GuestNameText).text = PhotonNetwork.LocalPlayer.NickName;
            GetButton((int)Buttons.GameStartBtn).interactable = true;
            GetButton((int)Buttons.GameStartBtn).gameObject.SetInteractable(GetButton((int)Buttons.GameStartBtn).interactable);
            GetText((int)Texts.GameStartBtnText).text = "Ready";
            GetButton((int)Buttons.GameStartBtn).gameObject.BindEvent(OnReadyBtnClicked);
        }

        PhotonRoomManager.OnReady += OnReady;
        PhotonRoomManager.OnClientSwitched += OnMasterClientSwitched;
        PhotonRoomManager.OnEntered += OnPlayerEnterRoom;
        PhotonRoomManager.OnLefted += OnPlayerLeftRoom;
        PhotonRoomManager.OnChatSended += OnChatSend;
    }

    private void OnDestroy() {
        PhotonRoomManager.OnReady -= OnReady;
        PhotonRoomManager.OnClientSwitched -= OnMasterClientSwitched;
        PhotonRoomManager.OnEntered -= OnPlayerEnterRoom;
        PhotonRoomManager.OnLefted -= OnPlayerLeftRoom;
        PhotonRoomManager.OnChatSended -= OnChatSend;
    }

    private void Update() {
        if (PhotonNetwork.InRoom) {
            if (Input.GetKeyDown(KeyCode.Return) && GetInputField((int)InputFields.ChatInputField).text != "") PhotonRoomManager.Instance.Send(GetInputField((int)InputFields.ChatInputField), texts);
        }
    }

    private void OnExitRoomBtnClicked(PointerEventData data) {
        PhotonRoomManager.Instance.LeaveRoom();
    }

    private void OnStartBtnClicked(PointerEventData data) {
        PhotonRoomManager.Instance.GameStart();
    }

    private void OnReadyBtnClicked(PointerEventData data) {
        PhotonRoomManager.Instance.GetReady(GetText((int)Texts.GameStartBtnText)); 
    }

    private void OnReady() {

        if (!PhotonNetwork.InRoom) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {

            if (PhotonNetwork.IsMasterClient &&  PhotonRoomManager.Instance.isReady) GetButton((int)Buttons.GameStartBtn).interactable = true;
            else GetButton((int)Buttons.GameStartBtn).interactable = false;

            GetButton((int)Buttons.GameStartBtn).gameObject.SetInteractable(GetButton((int)Buttons.GameStartBtn).interactable);
        }

    }

    private void OnMasterClientSwitched(Player newMasterClient) {

        if (PhotonNetwork.MasterClient != newMasterClient) return;

        GetText((int)Texts.HostNameText).text = newMasterClient.NickName;
        GetText((int)Texts.GuestNameText).text = "";
        GetButton((int)Buttons.GameStartBtn).interactable = false;
        GetButton((int)Buttons.GameStartBtn).gameObject.SetInteractable(GetButton((int)Buttons.GameStartBtn).interactable);
        GetText((int)Texts.GameStartBtnText).text = "Start";
        GetButton((int)Buttons.GameStartBtn).gameObject.BindEvent(OnStartBtnClicked);

    }

    private void OnPlayerEnterRoom(Player newPlayer) {
        GetText((int)Texts.GuestNameText).text = newPlayer.NickName;
    }

    private void OnPlayerLeftRoom(Player otherPlayer) {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonRoomManager.Instance.isReady = false;
        GetText((int)Texts.GuestNameText).text = "";
        GetButton((int)Buttons.GameStartBtn).interactable = false;
        GetButton((int)Buttons.GameStartBtn).gameObject.SetInteractable(GetButton((int)Buttons.GameStartBtn).interactable);
    }

    private void OnChatSend(string msg) {
        bool isInput = false;
        for (int i = 0; i < texts.Length; i++)
            if (texts[i].text == "") {
                isInput = true;
                texts[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < texts.Length; i++) texts[i - 1].text = texts[i].text;
            texts[texts.Length - 1].text = msg;
        }
    }

}
