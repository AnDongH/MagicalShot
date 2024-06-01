using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : UI_Scene
{
    enum Buttons {
        ExitGameBtn
    }

    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));
        GetButton((int)Buttons.ExitGameBtn).gameObject.BindEvent(OnExitGameBtnClicked);

        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("LoginCanvas");
    }

    private void OnEnable() {
        Init();
    }

    private void OnExitGameBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GameManager.Instance.ExitGame();
    }
}
