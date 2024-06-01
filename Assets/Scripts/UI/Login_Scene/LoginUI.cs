using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUI : UI_PopUp
{
    enum Buttons {
        RegisterBtn,
        LoginBtn,
        WarningOkBtn,
        FindPassWordBtn
    }

    enum InputFields {
        ID_Input,
        PW_Input
    }

    enum Texts {
        ID_Input_Text,
        PW_Input_Text,
        WarningText
    }

    // WarningGRP 경고창 나중에 따로 빼야한다.
    enum Objects {
        WarningGRP
    }

    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));

        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginBtnClicked);
        GetButton((int)Buttons.RegisterBtn).gameObject.BindEvent(OnRegisterBtnClicked);
        GetButton((int)Buttons.WarningOkBtn).gameObject.BindEvent(OnWarningOkBtnClicked);
        GetButton((int)Buttons.FindPassWordBtn).gameObject.BindEvent(OnFindPassWordBtnClicked);

        GetObject((int)Objects.WarningGRP).SetActive(false);
    }

    private void Start() {
        Init();

        PlayFabManager.OnLoginFailed += OnLoginFailed;
    }

    private void OnEnable() {
        Init();
    }

    private void OnDestroy() {
        PlayFabManager.OnLoginFailed -= OnLoginFailed;
    }

    private void OnLoginBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PlayFabManager.Instance.Login(GetInputField((int)InputFields.ID_Input), GetInputField((int)InputFields.PW_Input));
    }

    private void OnRegisterBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("RegisterCanvas");
    }

    private void OnLoginFailed(string error) {
        GetObject((int)Objects.WarningGRP).SetActive(true);
        GetText((int)Texts.WarningText).text = error;
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
    }

    private void OnFindPassWordBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("FindPassWordCanvas");
    }

}
