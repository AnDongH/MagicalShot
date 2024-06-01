using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NickNameUI : UI_PopUp
{
    enum Buttons {
        OkBtn
    }

    enum InputFields {
        NickName_Input
    }

    enum Texts {
        NickName_Input_Text
    }
    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));
        Bind<InputField>(typeof(InputFields));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.OkBtn).gameObject.BindEvent(OnOkBtnClicked);
    }

    private void OnEnable() {
        Init();
    }

    private void OnOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        PlayFabManager.Instance.UpdateDisplayName(GetInputField((int)InputFields.NickName_Input));
    }

}
