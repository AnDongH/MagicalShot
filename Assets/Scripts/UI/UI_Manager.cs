using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : NormalSingleton<UI_Manager>
{

    private int _order = 10;

    private Stack<UI_PopUp> _popupStack = new Stack<UI_PopUp>();
    private UI_Scene _sceneUI = null;

    private Dictionary<string, GameObject> curUIObjs = new Dictionary<string, GameObject>(); 

    /// <summary>
    /// 모든 UI들이 들어가는 부모
    /// </summary>
    public GameObject Root {
        get {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    /// <summary>
    /// 캔버스(하나의 UI 묶음 단위) 생성, 초기화
    /// </summary>
    /// <param name="go"></param>
    /// <param name="sort"></param>
    public void SetCanvas(GameObject go, bool sort = true) {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        if (sort) {
            canvas.sortingOrder = _order;
            _order++;
        }
        else {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// 고정 UI 생성
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go;

        if (curUIObjs.ContainsKey(name))
            go = UI_Instantiate(name);
        else {
            go = UI_Instantiate($"UI/Scene", name);
            curUIObjs.Add(name, go);
        }

        T SceneUI = Utils.GetOrAddComponent<T>(go);
        _sceneUI = SceneUI;

        go.transform.SetParent(Root.transform);

        return SceneUI;
    }

    /// <summary>
    /// 팝업 UI 생성
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T ShowPopupUI<T>(string name = null) where T : UI_PopUp {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go;

        if (curUIObjs.ContainsKey(name))
            go = UI_Instantiate(name);
        else { 
            go = UI_Instantiate($"UI/PopUp", name); 
            curUIObjs.Add(name, go);
        }


        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    /// <summary>
    /// 팝업 UI 닫기 -> 닫으려는 것이 가장 마지막에 연건지 확인
    /// </summary>
    /// <param name="popup"></param>
    public void ClosePopupUI(UI_PopUp popup) {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup) {
            Debug.Log("Close Popup Failed");
            return;
        }

        ClosePopupUI();
    }

    /// <summary>
    /// 팝업 UI 닫기 -> 가장 마지막에 연 UI 닫기
    /// </summary>
    public void ClosePopupUI() {
        if (_popupStack.Count == 0)
            return;

        UI_PopUp popup = _popupStack.Pop();
        popup.gameObject.SetActive(false);

        _order--;
    }

    /// <summary>
    /// 모든 팝업 UI 닫기
    /// </summary>
    public void CloseAllPopupUI() {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    /// <summary>
    /// 리소스 폴더에서, path인 파일 가져오기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    private T Load<T>(string path) where T : Object {
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// UI가 생성되기 전, 새롭게 생성
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject UI_Instantiate(string path, string name, Transform parent = null) {
        GameObject prefab = Load<GameObject>($"Prefab/{path}/{name}");
        if (prefab == null) {
            Debug.Log($"Filed to load prefab : {name}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    /// <summary>
    /// UI가 이미 씬에 존재할 시 꺼져 있는 것 On
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject UI_Instantiate(string name, Transform parent = null) {
        curUIObjs[name].SetActive(true);
        return curUIObjs[name];
    }

    /// <summary>
    /// UI 파괴
    /// </summary>
    /// <param name="go"></param>
    private void UI_Destroy(GameObject go) {
        if (go == null)
            return;

        Destroy(go);
    }
}
