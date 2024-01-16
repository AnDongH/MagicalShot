using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }
    

    [Header("���ҽ� ����Ÿ���̽�")]
    [SerializeField] ResourceSO resource;

    [Header("�⺻ �⹰ UI �׷�")]
    [SerializeField] GameObject marbleSelectUI;
    [SerializeField] GameObject posSelectUI;
    [SerializeField] Image marbleImg;
    [SerializeField] Button selectBtn;
    [SerializeField] Text explain;


    // ������ ���̽� �Ⱦ �ڵ����� ä�������� ��������
    [Header("�⹰ ���� UI")]
    [SerializeField] List<GameObject> marbleBtns;
    [SerializeField] GameObject btnPrefab;
    [SerializeField] Transform uiParent;
    [SerializeField] GameObject identifyUI;
    [SerializeField] Image[] posMarbleImg;
    [SerializeField] Button[] posBtns;
    [SerializeField] Button setBtn;
    [SerializeField] Button delBtn;

    [Header("���� �⹰, ����Ÿ")]
    public UserDataSO userData;
    public List<GameObject> selectedMarbles;
    public int SelectCount { 
        get {
            int tem = 0;
            foreach (var item in selectedMarbles) {
                if (item != null) tem++;
            }
            return tem;
        } 
    }

    [SerializeField] UserMarble marble;



    private void Awake() {
        Instance = this;

        
        selectedMarbles = new List<GameObject>();
        for (int i = 0; i < 4; i++) { 
            selectedMarbles.Add(null);
        }

    }

    private void Start() {
        
        // pos ��ư �ʱ�ȭ
        for (int i = 0; i < 4; i++) {
            int idx = i;
            posBtns[idx].onClick.RemoveAllListeners();
            posBtns[idx].onClick.AddListener(() => IdentifyUIOn(idx));
        }

    }

    // UI ���� �Լ�
    public void EnterMarbleGRP() {
        marbleSelectUI.SetActive(true);

        // ����Ÿ���̽��� ���� ����Ÿ ��
        foreach (string id in userData.curMarblesId) {
            UserMarble userMarble = resource.marbles.Find(x => x.id == id);
            if (userMarble == null) {
                print("�ش� �⹰ ����Ÿ���̽��� �������� ����");
                continue;
            }

            // ������ ������ �ִ� ĳ���� ���� ��ư�� Ȱ��ȭ
            GameObject btnObj = Instantiate(btnPrefab, uiParent);
            Button btn = btnObj.GetComponent<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            marbleBtns.Add(btnObj);
            image.sprite = userMarble.sprite;

            // ��ư�� �Լ� ���
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SetMarbleSelect(userMarble));
        }
    }

    public void ExitMarbleGRP() {

        // ���� UI off
        marbleSelectUI.SetActive(false);

        // �����ߴ� ��ư�� ����
        for (int i = 0; i < marbleBtns.Count; i++) {
            Destroy(marbleBtns[i]);
        }
        marbleBtns.Clear();

        // �⺻ UI ������ �ʱ�ȭ
        marbleImg.sprite = null;
        marbleImg.enabled = false;
        explain.text = "";
        selectBtn.gameObject.SetActive(false);
    }

    public void EnterPosSelect() {
        posSelectUI.SetActive(true);
    }

    public void ExitPosSelect() {
        identifyUI.SetActive(false);
        posSelectUI.SetActive(false);
    }

    private void SetMarbleSelect(UserMarble testMarble) {
        selectBtn.gameObject.SetActive(true);
        marbleImg.enabled = true;
        marbleImg.sprite = testMarble.sprite;
        explain.text = testMarble.explain;

        marble = testMarble;
    }

    public void SetPos(int pos) {
        if (selectedMarbles[pos] != null) {
            print("�⹰�� �̹� �����մϴ�.");
            IdentifyUIOff();
            return;
        }

        if (selectedMarbles.Find(x => x == marble.prefab)) {
            print("�̹� �ش� ������ �����մϴ�.");
            IdentifyUIOff();
            return;
        }


        selectedMarbles[pos] = marble.prefab;
        posMarbleImg[pos].enabled = true;
        posMarbleImg[pos].sprite = marble.sprite;
        IdentifyUIOff();
    }

    public void DelPos(int pos) {
        if (selectedMarbles[pos] == null) {
            print("������ �⹰�� �����ϴ�.");
            IdentifyUIOff();
            return;
        }
        selectedMarbles[pos] = null;
        posMarbleImg[pos].enabled = false;
        posMarbleImg[pos].sprite = null;
        IdentifyUIOff();
    }

    private void IdentifyUIOn(int pos) {
        identifyUI.SetActive(true);

        setBtn.onClick.RemoveAllListeners();
        setBtn.onClick.AddListener(() => SetPos(pos));
        delBtn.onClick.RemoveAllListeners();
        delBtn.onClick.AddListener(() => DelPos(pos));
    }

    private void IdentifyUIOff() {
        identifyUI.SetActive(false);
        setBtn.onClick.RemoveAllListeners();
        delBtn.onClick.RemoveAllListeners();
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
