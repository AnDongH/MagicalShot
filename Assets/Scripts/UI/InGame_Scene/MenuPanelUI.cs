using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuPanelUI : UI_PopUp {
    enum Buttons {
        OptionBtn,
        GiveUpBtn,
        ExitBtn
    }

    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.OptionBtn).gameObject.BindEvent(OnOptionBtnCliked);
        GetButton((int)Buttons.GiveUpBtn).gameObject.BindEvent(OnGiveUpBtnCliked);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
    }

    private void OnEnable() {
        Init();
    }

    private void OnOptionBtnCliked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("OptionCanvas");
    }

    private void OnGiveUpBtnCliked(PointerEventData data) {
        ClosePopUpUI();
        InGameManager.Instance.GiveUp();
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }
}
