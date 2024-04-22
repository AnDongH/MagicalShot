using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }
    

    [field: SerializeField, Header("���ҽ� ����Ÿ���̽�")]
    public ResourceSO resource { get; private set; }

    [field: SerializeField, Header("���� �⹰, ����Ÿ")]
    public UserDataSO userData { get; private set; }
    public List<GameObject> selectedMarbles { get; private set; }
    public int selectedMarblePos { get; set; } = -1;

    public int SelectCount { 
        get {
            int tem = 0;
            foreach (var item in selectedMarbles) {
                if (item != null) tem++;
            }
            return tem;
        } 
    }

    public UserMarble marble { get; set; }



    private void Awake() {
        Instance = this;

        
        selectedMarbles = new List<GameObject>();
        for (int i = 0; i < 4; i++) { 
            selectedMarbles.Add(null);
        }

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            selectedMarbles[0] = resource.marbles.Find(x => x.id == "TA01").prefab;
            selectedMarbles[1] = resource.marbles.Find(x => x.id == "TA02").prefab;
            selectedMarbles[2] = resource.marbles.Find(x => x.id == "DD01").prefab;
            selectedMarbles[3] = resource.marbles.Find(x => x.id == "DD02").prefab;
        }
    }


    public void SetPos() {
        if (marble == null || marble.prefab == null) {
            print("���õ� �⹰�� �����ϴ�.");
            return;
        }

        if (selectedMarbles[selectedMarblePos] != null) {
            print("�⹰�� �̹� �����մϴ�.");
            return;
        }

        if (selectedMarbles.Find(x => x == marble.prefab)) {
            print("�̹� �ش� ������ �����մϴ�.");
            return;
        }

        selectedMarbles[selectedMarblePos] = marble.prefab;
    }

    public void DelPos() {
        if (selectedMarblePos == -1 || selectedMarbles[selectedMarblePos] == null) {
            print("������ �⹰�� �����ϴ�.");
            return;
        }
        selectedMarbles[selectedMarblePos] = null;
    }

    public void PosImgSet(bool flag, Image posImg) {
        posImg.enabled = flag;
        if (flag) posImg.sprite = marble.sprite;
        else posImg.sprite = null;
    }

    public void SendMarbleData() {
        // �̺κ� ���� �� ���� ������ �� ����.
        foreach (GameObject obj in selectedMarbles) {
            if (obj != null && !SettingData.Instance.marbleDeck.Contains(obj)) SettingData.Instance.marbleDeck.Add(obj);
        }
    }


    // �⹰ ���� �Լ�

    // �� ���� �Լ�
}
