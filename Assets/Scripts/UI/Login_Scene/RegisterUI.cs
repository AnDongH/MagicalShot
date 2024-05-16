using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisterUI : UI_PopUp
{
    enum Buttons {
        OkBtn,
        ExitBtn
    }

    enum InputFields {
        ID_Input,
        PW_Input,
        AccountNameInput
    }

    enum Texts {
        ID_Input_Text,
        PW_Input_Text,
        AccountNameText
    }
    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.OkBtn).gameObject.BindEvent(OnOkBtnClicked);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
    }

    private void Start() {
        Init();
    }

    private void OnOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PlayFabManager.Instance.Register(GetInputField((int)InputFields.ID_Input), GetInputField((int)InputFields.PW_Input), GetInputField((int)InputFields.AccountNameInput));
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }
}
