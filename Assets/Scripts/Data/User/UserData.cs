using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �ӽ� ���� ����Ÿ ���̽���,
 * ���߿� ���� �������� ��Ʈ�� �鿣�� ������ �� �������ش�.
 * ���̵� ���� �������� ��Ʈ���� ����Ÿ �ҷ��ͼ� SO�� �������ش�.
 * �׸��� �ش� SO ������ ����ؼ� �⹰ ����Ÿ�� ����Ѵ�.
 */

[System.Serializable]
public class UserData {
    public List<string> curMarblesId;
    public string[] marbleDeck;
    public List<string> curRunesId;
    public List<string> runesDeck;
    public int winScore;
    public int winCnt;
    public int loseCnt;
    public int money;
    public List<StatisticValue> statistics;

    public UserData() {
        marbleDeck = new string[4];
        runesDeck = new List<string>(20);
        curMarblesId = new List<string>() { "TA01", "TA02", "DD01", "DD02" /*"AD01", "AD02", "AP01", "AP02" */};
        curRunesId = new List<string>() { "MAB_BUF01_RUNE", "MAB_BUF02_RUNE", "MAB_BUF03_RUNE", "MAB_BUF04_RUNE",
                                          "MAB_BUF05_RUNE", "MAB_BUF06_RUNE", "MAB_BUF07_RUNE", "MAB_BUF08_RUNE",
                                          "MAB_BUF09_RUNE", "MAB_BUF10_RUNE", "MAB_BUF11_RUNE", "MAB_ATK01_RUNE" 
                                          /*"MAB_ATK02_RUNE", "MAB_ATK03_RUNE", "MAB_ATK04_RUNE", "MAB_ATK05_RUNE", 
                                          "MAB_ATK06_RUNE", "MAB_ATK07_RUNE", "MAB_ATK08_RUNE", "MAB_ATK09_RUNE", 
                                          "MAB_ATK10_RUNE", "MAB_DEF01_RUNE", "MAB_DEF02_RUNE", "MAB_DEF03_RUNE",
                                          "MAB_DEF04_RUNE", "OBJ_WALL01_RUNE", "OBJ_WALL02_RUNE", "OBJ_TRAP01_RUNE",
                                          "OBJ_TRAP02_RUNE", "OBJ_TRAP03_RUNE", "OBJ_TRAP04_RUNE", "OBJ_TURRET01_RUNE",
                                          "OBJ_TURRET02_RUNE", "OBJ_TURRET03_RUNE", "OBJ_TURRET04_RUNE"*/};
    }

    public int SelectCount {
        get {
            int tem = 0;
            foreach (var item in marbleDeck) {
                if (!string.IsNullOrEmpty(item)) tem++;
            }
            return tem;
        }
    }

}


