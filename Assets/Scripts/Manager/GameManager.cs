using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameManager : DontDestroySingleton<GameManager>
{
    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public static void ShowLoadingUI() {
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("LoadingCanvas");
    }

    public static void CloseLoadingUI() {
        UI_Manager.Instance.ClosePopupUI();
    }
}
