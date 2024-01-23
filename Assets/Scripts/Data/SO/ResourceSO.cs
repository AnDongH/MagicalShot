using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UserMarble {

    public enum MarbleType {
        TA, DD, AD, AP
    }

    public MarbleType Type;
    public string name;
    public string id;
    public GameObject prefab;
    public Sprite sprite;
    public string explain;
}


[CreateAssetMenu(fileName = "ResourceSO", menuName = "Scriptable Object/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    public List<UserMarble> marbles;
}


// 지금은 임시로 이런 식이지만
// 기물들 클래스로 능력치 분화하면서 데이터별 SO 만들고, 그 안에 프리팹 넣어놓는 걸로 하자
