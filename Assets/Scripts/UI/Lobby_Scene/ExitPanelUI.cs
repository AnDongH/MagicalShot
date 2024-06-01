using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitPanelUI : UI_PopUp
{
    enum Buttons {
        ServerExitBtn,
        GameExitBtn,
        ExitBtn
    }

    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ServerExitBtn).gameObject.BindEvent(OnServerExitBtnClicked);
        GetButton((int)Buttons.GameExitBtn).gameObject.BindEvent(OnGameExitBtnClicked);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
    }

    private void OnEnable() {
        Init();
    }

    private void OnServerExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        DataManager.Instance.SaveData(DisconectCallBack);
    }

    private void OnGameExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        DataManager.Instance.SaveData(ExitCallBack);
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }

    private void ExitCallBack(UpdateUserDataResult result) {
        GameManager.Instance.ExitGame();
    }

    private void DisconectCallBack(UpdateUserDataResult result) {
        PhotonLobbyManager.Instance.Disconnect();
    }
}
