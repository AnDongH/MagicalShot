using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginUI : UI_PopUp
{
    enum Buttons {
        RegisterBtn,
        LoginBtn
    }

    enum InputFields {
        ID_Input,
        PW_Input
    }

    enum Texts {
        ID_Input_Text,
        PW_Input_Text
    }
    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.LoginBtn).gameObject.BindEvent(OnLoginBtnClicked);
        GetButton((int)Buttons.RegisterBtn).gameObject.BindEvent(OnRegisterBtnClicked);
    }

    private void Start() {
        Init();
    }

    private void OnLoginBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PlayFabManager.Instance.Login(GetInputField((int)InputFields.ID_Input), GetInputField((int)InputFields.PW_Input));
    }

    private void OnRegisterBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("RegisterCanvas");
    }

}
