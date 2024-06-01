using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisterUI : UI_PopUp
{
    enum Buttons {
        OkBtn,
        ExitBtn,
        WarningOkBtn
    }

    enum InputFields {
        ID_Input,
        PW_Input,
        AccountNameInput
    }

    enum Texts {
        ID_Input_Text,
        PW_Input_Text,
        AccountNameText,
        WarningText
    }

    enum Objects {
        WarningGRP
    }

    protected override void Init() {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));

        GetButton((int)Buttons.OkBtn).gameObject.BindEvent(OnOkBtnClicked);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
        GetButton((int)Buttons.WarningOkBtn).gameObject.BindEvent(OnWarningOkBtnClicked);

        GetObject((int)Objects.WarningGRP).SetActive(false);
    }

    private void Start() {
        Init();

        PlayFabManager.OnRegisterFailed += OnRegisterFailed;
    }

    private void OnEnable() {
        Init();
    }

    private void OnDestroy() {
        PlayFabManager.OnRegisterFailed -= OnRegisterFailed;
    }

    private void OnOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PlayFabManager.Instance.Register(GetInputField((int)InputFields.ID_Input), GetInputField((int)InputFields.PW_Input), GetInputField((int)InputFields.AccountNameInput));
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }

    private void OnRegisterFailed(string error) {
        GetObject((int)Objects.WarningGRP).SetActive(true);
        GetText((int)Texts.WarningText).text = error;
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
    }
}
