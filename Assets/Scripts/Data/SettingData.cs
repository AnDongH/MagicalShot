using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingData : Singleton<SettingData>
{

    public string envPath = "SPUM/SPUM_Units/";
    public string prefab_envPath = "Prefab/";

    // 선택한 기물들
    [Header("기물 세팅")]
    public List<GameObject> marbleDeck;
    

    // 선택한 룬들
    // TODO


    

}
