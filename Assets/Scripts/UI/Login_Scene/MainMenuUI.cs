using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUI : UI_Scene
{
    protected override void Init() {
        base.Init();
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("LoginCanvas");
    }

    private void Start() {
        Init();
    }
}
