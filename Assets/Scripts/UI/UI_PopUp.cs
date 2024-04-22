using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UI_PopUp : UI_Base
{
    protected virtual void Init() {
        UI_Manager.Instance.SetCanvas(gameObject, true);
    }
    protected virtual void ClosePopUpUI() {
        UI_Manager.Instance.ClosePopupUI(this);
    }

}
