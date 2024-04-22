using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;


// 오브젝트에 직접 넣지 않고 다른 클래스에서 접근하는 클래스

[System.Serializable]
public class PRS {
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scale) {
        this.pos = pos;
        this.rot = rot;
        this.scale = scale;
    }
}


public class Utils {
    public static Quaternion QI => Quaternion.identity;

    public static Vector3 MousePos {
        get {
            Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            result.z = -10;
            return result;
        }
    }

    /// <summary>
    /// go의 자식 오브젝트 중에서 이름이 name인 것 중에서 T컴포넌트 가져오기
    /// recursive가 true 일 시 모든 자식을 다 훑어보고
    /// false일 시 가장 첫 자식만 본다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : Object {
        if (go == null)
            return null;

        if (recursive == false) {
            for (int i = 0; i < go.transform.childCount; i++) {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name) {
                    T componet = transform.GetComponent<T>();
                    if (componet != null)
                        return componet;
                }
            }
        }
        else {
            foreach (T component in go.GetComponentsInChildren<T>()) {
                if (string.IsNullOrEmpty(name) || component.name == name) {
                    return component;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// go에서 T 컴포넌트 가져오기
    /// 만약 없다면 붙여서라도 가져오기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : Component {
        T component = go.GetComponent<T>();
        if (component == null) component = go.AddComponent<T>();
        return component;
    }

    /// <summary>
    /// go의 자식 오브젝트 중에서 이름이 name인 것 중에서 게임 오브젝트 가져오기
    /// recursive가 true 일 시 모든 자식을 다 훑어보고
    /// false일 시 가장 첫 자식만 본다.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="name"></param>
    /// <param name="recursive"></param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false) {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null) return null;
        return transform.gameObject;
    }


}