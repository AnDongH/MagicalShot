using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FindPassWordCanvasUI : UI_PopUp
{
    enum Buttons {
        OkBtn,
        ExitBtn,
        WarningOkBtn
    }

    enum InputFields {
        ID_Input,
    }

    enum Texts {
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

        PlayFabManager.OnFindFailed += OnFindFailed;
        PlayFabManager.OnFindSuccess += OnFindSuccess;
        
    }

    private void OnEnable() {
        Init();
    }

    private void OnDestroy() {
        PlayFabManager.OnFindFailed -= OnFindFailed;
        PlayFabManager.OnFindSuccess -= OnFindSuccess;
    }

    private void OnOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        FindPassWord(GetInputField((int)InputFields.ID_Input));
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }

    private void OnFindFailed(string error) {
        GetObject((int)Objects.WarningGRP).SetActive(true);
        GetText((int)Texts.WarningText).text = error;
    }

    private void OnFindSuccess(string error) {
        GetObject((int)Objects.WarningGRP).SetActive(true);
        GetText((int)Texts.WarningText).text = error;
    }

    private void FindPassWord(InputField inputField) {
        PlayFabManager.Instance.FindPassWordOnEmail(inputField);
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
        ClosePopUpUI();
    }
}
