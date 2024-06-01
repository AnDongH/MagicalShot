using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoadingCanvasUI : UI_PopUp
{

    enum Texts {
        LoadingText
    }


    protected override void Init() {
        base.Init();
        Bind<Text>(typeof(Texts));

    }

    private void Start() {
        Init();     
    }

    private void OnEnable() {
        Init();
    }
}
