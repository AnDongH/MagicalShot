using System.Collections;
using System.Collections.Generic;
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
}