using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
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
    public RuneData Rune { get; set; } = null;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            DataManager.Instance.userData.marbleDeck[0] = DataManager.Instance.Resource.marbles.Find(x => x.id == "TA01").id;
            DataManager.Instance.userData.marbleDeck[1] = DataManager.Instance.Resource.marbles.Find(x => x.id == "TA02").id;
            DataManager.Instance.userData.marbleDeck[2] = DataManager.Instance.Resource.marbles.Find(x => x.id == "DD01").id;
            DataManager.Instance.userData.marbleDeck[3] = DataManager.Instance.Resource.marbles.Find(x => x.id == "DD02").id;
        }
    }


    public string SetMarblePos(Image image) {
        if (Marble == null || Marble.id.IsNullOrEmpty()) {
            SoundManager.Instance.PlaySFXSound("Warning");
            print("선택된 기물이 없습니다.");
            return "선택된 기물이 없습니다.";
        }

        if (!DataManager.Instance.userData.marbleDeck[SelectedMarblePos].IsNullOrEmpty()) {
            SoundManager.Instance.PlaySFXSound("Warning");
            print("기물이 이미 존재합니다.");
            return "기물이 이미 존재합니다.";
        }

        if (!Array.Find(DataManager.Instance.userData.marbleDeck, x => x == Marble.id).IsNullOrEmpty()) {
            SoundManager.Instance.PlaySFXSound("Warning");
            print("이미 해당 유닛이 존재합니다.");
            return "이미 해당 유닛이 존재합니다.";
        }

        SoundManager.Instance.PlaySFXSound("Select");
        DataManager.Instance.userData.marbleDeck[SelectedMarblePos] = Marble.id;
        MarblePosImgSet(true, image, Marble.id);
        return null;
    }

    public string DelMarblePos(Image image) {
        if (SelectedMarblePos == -1 || DataManager.Instance.userData.marbleDeck[SelectedMarblePos].IsNullOrEmpty()) {
            SoundManager.Instance.PlaySFXSound("Warning");
            print("제거할 기물이 없습니다.");
            return "제거할 기물이 없습니다.";
        }

        SoundManager.Instance.PlaySFXSound("Delete");
        DataManager.Instance.userData.marbleDeck[SelectedMarblePos] = null;
        MarblePosImgSet(false, image);
        return null;
    }

    public void MarblePosImgSet(bool flag, Image posImg, string id = null) {
        posImg.enabled = flag;
        if (flag) {
            Sprite sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == id + "_Image");
            posImg.sprite = sprite;
        }
        else posImg.sprite = null;
    }

    // 룬 선택 함수

    public string SetRuneOnDeck() {

        if (DataManager.Instance.userData.runesDeck.Count == 20) return "룬은 20개까지만 등록이 가능합니다.";

        if (DataManager.Instance.userData.runesDeck.FindAll(x => x == Rune.id).Count >= Rune.maxHasCount) return "이미 해당 룬의 최대 소지 개수입니다.";

        DataManager.Instance.userData.runesDeck.Add(Rune.id);

        return null;
    }
}
