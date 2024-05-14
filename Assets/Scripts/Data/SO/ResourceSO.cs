using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class MarbleData {

    public enum MarbleType {
        TA, DD, AD, AP
    }

    public string id;
    public string name;
    public MarbleType Type;
    public string explain;
    public int hp;
    public int damage;
}

[System.Serializable]
public class RuneData {

    public enum RuneType {
        
    }
    
}


[CreateAssetMenu(fileName = "ResourceSO", menuName = "Scriptable Object/ResourceSO")]
public class ResourceSO : ScriptableObject
{
    public List<MarbleData> marbles;
    public List<RuneData> runes;
    public List<Sprite> images;
}


