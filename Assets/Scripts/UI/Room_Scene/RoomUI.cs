using Photon.Pun;
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
        ChatText1,
        ChatText2,
        ChatText3,
        ChatText4,
        ChatText5,
        ChatText6,
        ChatText7,
        ChatText8,
        ChatText9,
        ChatText10,
        ChatText11,
        ChatText12,
        ChatText13,
        ChatText14,
        GameStartBtnText,
        HostNameText,
        GuestNameText,
        RoomNameText,
        MapNameText
    }

    private bool isReady = false;

    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));


    }

    private void Start() {
        Init();

        // 방 초기화
        isReady = false;
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
            GetButton((int)Buttons.GameStartBtn).gameObject.BindEvent((data) => OnReadyBtnClicked(data, isReady));
        }
    }

    private void OnStartBtnClicked(PointerEventData data) {
        PhotonManager.Instance.GameStart();
    }

    private void OnReadyBtnClicked(PointerEventData data, bool isReady) {
        PhotonManager.Instance.GetReady(GetText((int)Texts.GameStartBtnText), isReady); 
    }

}
