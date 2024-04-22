using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension
{
    // 확장 메서드
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component {
        return Utils.GetOrAddComponent<T>(go);
    }

    // 확장메서드
    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click) {
        UI_Base.BindEvent(go, action, type);
    }

    public static void SetInteractable(this GameObject go, bool flag) {
        UI_Base.SetInteractable(go, flag);
    }
}
