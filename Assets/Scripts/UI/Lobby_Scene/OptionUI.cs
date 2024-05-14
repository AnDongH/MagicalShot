using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionUI : UI_PopUp
{
    enum Buttons {
        ExitBtn,
        OkBtn,
        ExitGameBtn
    }

    enum Dropdowns {
        Resolution_Dropdown
    }

    enum Toggles {
        FullScreenToggle
    }


    protected override void Init() {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Dropdown>(typeof(Dropdowns));
        Bind<Toggle>(typeof(Toggles));

        OptionManager.Instance.InitUI(GetDropdown((int)Dropdowns.Resolution_Dropdown), GetToggle((int)Toggles.FullScreenToggle));


        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
        GetButton((int)Buttons.OkBtn).gameObject.BindEvent(OnOkBtnClicked);
        GetButton((int)Buttons.ExitGameBtn).gameObject.BindEvent(OnExitGameBtnClicked);

        GetDropdown((int)Dropdowns.Resolution_Dropdown).onValueChanged.AddListener(OnResolution_DropdownChanged);
        GetToggle((int)Toggles.FullScreenToggle).onValueChanged.AddListener(OnFullScreenToggleChanged);
    }

    private void Start() {
        Init();
    }

    private void OnExitBtnClicked(PointerEventData data) {
        ClosePopUpUI();
    }

    private void OnOkBtnClicked(PointerEventData data) {
        OptionManager.Instance.OkBtnClick();
    }

    private void OnResolution_DropdownChanged(int x) {
        OptionManager.Instance.DropboxOptionChange(x);
    }

    private void OnFullScreenToggleChanged(bool flag) {
        OptionManager.Instance.FullScreenBtn(flag);
    }

    private void OnExitGameBtnClicked(PointerEventData data) {
        GameManager.Instance.ExitGame();
    }

}
