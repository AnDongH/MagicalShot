using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : NormalSingleton<UI_Manager>
{

    private int _order = 10;

    private Stack<UI_PopUp> _popupStack = new Stack<UI_PopUp>();
    private UI_Scene _sceneUI = null;

    private Dictionary<string, GameObject> curUIObjs = new Dictionary<string, GameObject>(); 

    /// <summary>
    /// ��� UI���� ���� �θ�
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
    /// ĵ����(�ϳ��� UI ���� ����) ����, �ʱ�ȭ
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
    /// ���� UI ����
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
    /// �˾� UI ����
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
    /// �˾� UI �ݱ� -> �������� ���� ���� �������� ������ Ȯ��
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
    /// �˾� UI �ݱ� -> ���� �������� �� UI �ݱ�
    /// </summary>
    public void ClosePopupUI() {
        if (_popupStack.Count == 0)
            return;

        UI_PopUp popup = _popupStack.Pop();
        popup.gameObject.SetActive(false);

        _order--;
    }

    /// <summary>
    /// ��� �˾� UI �ݱ�
    /// </summary>
    public void CloseAllPopupUI() {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    /// <summary>
    /// ���ҽ� ��������, path�� ���� ��������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    private T Load<T>(string path) where T : Object {
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// UI�� �����Ǳ� ��, ���Ӱ� ����
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
    /// UI�� �̹� ���� ������ �� ���� �ִ� �� On
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    private GameObject UI_Instantiate(string name, Transform parent = null) {
        curUIObjs[name].SetActive(true);
        return curUIObjs[name];
    }

    /// <summary>
    /// UI �ı�
    /// </summary>
    /// <param name="go"></param>
    private void UI_Destroy(GameObject go) {
        if (go == null)
            return;

        Destroy(go);
    }
}
