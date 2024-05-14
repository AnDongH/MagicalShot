using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SettingManager : DontDestroySingleton<SettingManager>
{

    public string envPath = "SPUM/SPUM_Units/";
    public string prefab_envPath = "Prefab/";

    [field: SerializeField, Header("선택 기물, 데이타")]
    public int SelectedMarblePos { get; set; } = -1;

    public MarbleData Marble { get; set; } = null;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            DataManager.Instance.userData.marbleDeck[0] = DataManager.Instance.Resource.marbles.Find(x => x.id == "TA01").id;
            DataManager.Instance.userData.marbleDeck[1] = DataManager.Instance.Resource.marbles.Find(x => x.id == "TA02").id;
            DataManager.Instance.userData.marbleDeck[2] = DataManager.Instance.Resource.marbles.Find(x => x.id == "DD01").id;
            DataManager.Instance.userData.marbleDeck[3] = DataManager.Instance.Resource.marbles.Find(x => x.id == "DD02").id;
        }
    }


    public string SetPos(Image image) {
        if (Marble == null || Marble.id.IsNullOrEmpty()) {
            print("선택된 기물이 없습니다.");
            return "선택된 기물이 없습니다.";
        }

        if (!DataManager.Instance.userData.marbleDeck[SelectedMarblePos].IsNullOrEmpty()) {
            print("기물이 이미 존재합니다.");
            return "기물이 이미 존재합니다.";
        }

        if (!Array.Find(DataManager.Instance.userData.marbleDeck, x => x == Marble.id).IsNullOrEmpty()) {
            print("이미 해당 유닛이 존재합니다.");
            return "이미 해당 유닛이 존재합니다.";
        }

        DataManager.Instance.userData.marbleDeck[SelectedMarblePos] = Marble.id;
        PosImgSet(true, image, Marble.id);
        return null;
    }

    public string DelPos(Image image) {
        if (SelectedMarblePos == -1 || DataManager.Instance.userData.marbleDeck[SelectedMarblePos].IsNullOrEmpty()) {
            print("제거할 기물이 없습니다.");
            return "제거할 기물이 없습니다.";
        }
        DataManager.Instance.userData.marbleDeck[SelectedMarblePos] = null;
        PosImgSet(false, image);
        return null;
    }

    public void PosImgSet(bool flag, Image posImg, string id = null) {
        posImg.enabled = flag;
        if (flag) {
            Sprite sprite = DataManager.Instance.Resource.images.Find(x => x.name == id + "_image");
            posImg.sprite = sprite;
        }
        else posImg.sprite = null;
    }


    // 기물 선택 함수

    // 룬 선택 함수
}
