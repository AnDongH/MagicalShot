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

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.OptionBtn).gameObject.BindEvent(OnOptionBtnCliked);
        GetButton((int)Buttons.GiveUpBtn).gameObject.BindEvent(OnGiveUpBtnCliked);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
    }

    private void Start() {
        Init();
    }

    private void OnOptionBtnCliked(PointerEventData data) {
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("OptionCanvas");
    }

    private void OnGiveUpBtnCliked(PointerEventData data) {
        ClosePopUpUI();
        InGameManager.Instance.GiveUp();
    }

    private void OnExitBtnClicked(PointerEventData data) {
        ClosePopUpUI();
    }
}
