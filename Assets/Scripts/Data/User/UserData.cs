using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 임시 유저 데이타 베이스로,
 * 나중에 구글 스프레드 시트로 백엔드 구축할 때 변경해준다.
 * 아이디에 따라 스프레드 시트에서 데이타 불러와서 SO에 저장해준다.
 * 그리고 해당 SO 정보를 사용해서 기물 데이타를 사용한다.
 */

public class UserData {
    public List<string> curMarblesId;
    public string[] marbleDeck;
    public List<string> curRunesId;
    public string[] runesDeck;
    public int winScore;
    public int winCnt;
    public int loseCnt;
    public int money;
    public List<StatisticValue> statistics;

    public UserData() {
        marbleDeck = new string[4];
        runesDeck = new string[20];
        curMarblesId = new List<string>() { "TA01", "TA02", "DD01", "DD02", "AD01", "AD02", "AP01", "AP02" };
    }

    public int SelectCount {
        get {
            int tem = 0;
            foreach (var item in marbleDeck) {
                if (item != null) tem++;
            }
            return tem;
        }
    }

}


