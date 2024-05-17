using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomUI : UI_Scene
{
    enum Buttons {
        ExitRoomBtn,
        GameStartBtn,
        NextMapBtn,
        PrevMapBtn
    }

    enum Images {
        MapImage
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
        Bind<Image>(typeof(Images));

        texts = GetObject((int)Objects.TextContent).GetComponentsInChildren<Text>();
        GetButton((int)Buttons.ExitRoomBtn).gameObject.BindEvent(OnExitRoomBtnClicked);
        GetButton((int)Buttons.NextMapBtn).gameObject.BindEvent((data) => OnMapBtnClicked(data, true));
        GetButton((int)Buttons.PrevMapBtn).gameObject.BindEvent((data) => OnMapBtnClicked(data, false));

        GetImage((int)Images.MapImage).sprite = DataManager.Instance.Resource.mapImages.Find(x => x.name == DataManager.Instance.Resource.maps[DataManager.Instance.mapIndex].id + "_image");
        GetText((int)Texts.MapNameText).text = DataManager.Instance.Resource.maps[DataManager.Instance.mapIndex].name;
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

            GetButton((int)Buttons.NextMapBtn).interactable = false;
            GetButton((int)Buttons.NextMapBtn).gameObject.SetInteractable(GetButton((int)Buttons.NextMapBtn).interactable);

            GetButton((int)Buttons.PrevMapBtn).interactable = false;
            GetButton((int)Buttons.PrevMapBtn).gameObject.SetInteractable(GetButton((int)Buttons.PrevMapBtn).interactable);
        }

        PhotonRoomManager.OnReady += OnReady;
        PhotonRoomManager.OnClientSwitched += OnMasterClientSwitched;
        PhotonRoomManager.OnEntered += OnPlayerEnterRoom;
        PhotonRoomManager.OnLefted += OnPlayerLeftRoom;
        PhotonRoomManager.OnChatSended += OnChatSend;

        RoomSelect.OnMapButtonClicked += OnMapUpdated;
        
    }

    private void OnDestroy() {
        PhotonRoomManager.OnReady -= OnReady;
        PhotonRoomManager.OnClientSwitched -= OnMasterClientSwitched;
        PhotonRoomManager.OnEntered -= OnPlayerEnterRoom;
        PhotonRoomManager.OnLefted -= OnPlayerLeftRoom;
        PhotonRoomManager.OnChatSended -= OnChatSend;

        RoomSelect.OnMapButtonClicked -= OnMapUpdated;
    }

    private void Update() {
        if (PhotonNetwork.InRoom) {
            if (Input.GetKeyDown(KeyCode.Return) && GetInputField((int)InputFields.ChatInputField).text != "") PhotonRoomManager.Instance.Send(GetInputField((int)InputFields.ChatInputField), texts);
        }
    }

    private void OnExitRoomBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PhotonRoomManager.Instance.LeaveRoom();
    }

    private void OnStartBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("Start");
        PhotonRoomManager.Instance.GameStart();
    }

    private void OnReadyBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("Ready");
        PhotonRoomManager.Instance.GetReady(GetText((int)Texts.GameStartBtnText)); 
    }

    private void OnMapBtnClicked(PointerEventData data, bool flag) {
        RoomSelect.Instance.MapUpdate(flag);
    }

    private void OnMapUpdated() {
        GetImage((int)Images.MapImage).sprite = DataManager.Instance.Resource.mapImages.Find(x => x.name == DataManager.Instance.Resource.maps[DataManager.Instance.mapIndex].id + "_image"); ;
        GetText((int)Texts.MapNameText).text = DataManager.Instance.Resource.maps[DataManager.Instance.mapIndex].name;
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

        GetButton((int)Buttons.NextMapBtn).interactable = true;
        GetButton((int)Buttons.NextMapBtn).gameObject.SetInteractable(GetButton((int)Buttons.NextMapBtn).interactable);

        GetButton((int)Buttons.PrevMapBtn).interactable = true;
        GetButton((int)Buttons.PrevMapBtn).gameObject.SetInteractable(GetButton((int)Buttons.PrevMapBtn).interactable);
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
        SoundManager.Instance.PlaySFXSound("Chat");
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
