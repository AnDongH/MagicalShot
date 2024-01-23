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


// ������ �ӽ÷� �̷� ��������
// �⹰�� Ŭ������ �ɷ�ġ ��ȭ�ϸ鼭 �����ͺ� SO �����, �� �ȿ� ������ �־���� �ɷ� ����
