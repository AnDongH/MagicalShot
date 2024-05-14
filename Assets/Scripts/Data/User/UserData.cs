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


