using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance { get; private set; }
    

    [Header("리소스 데이타베이스")]
    [SerializeField] ResourceSO resource;

    [Header("기본 기물 UI 그룹")]
    [SerializeField] GameObject marbleSelectUI;
    [SerializeField] Image marbleImg;
    [SerializeField] Text explain;


    // 데이터 베이스 훑어서 자동으로 채워지도록 만들어야함
    [Header("기물 선택 UI")]
    [SerializeField] List<GameObject> marbleBtns;
    [SerializeField] GameObject btnPrefab;
    [SerializeField] Transform uiParent;
    [SerializeField] GameObject identifyUI;
    [SerializeField] Image[] posMarbleImg;
    [SerializeField] Button[] posBtns;
    [SerializeField] Button setBtn;
    [SerializeField] Button delBtn;
    [SerializeField] Dropdown marbleOption;

    [Header("선택 기물, 데이타")]
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
        
        // pos 버튼 초기화
        for (int i = 0; i < 4; i++) {
            int idx = i;
            posBtns[idx].onClick.RemoveAllListeners();
            posBtns[idx].onClick.AddListener(() => IdentifyUIOn(idx));
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

    // UI 조작 함수
    public void EnterMarbleGRP() {
        marbleSelectUI.SetActive(true);
        marbleOption.value = 0;

        SetMarbleButton();
    }

    // 최적화가 필요할수도...

    public void SetMarbleButton() {

        // 생성했던 버튼들 제거
        for (int i = 0; i < marbleBtns.Count; i++) {
            marbleBtns[i].GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(marbleBtns[i]);
        }
        marbleBtns.Clear();

        // 기본 UI 정보들 초기화
        marbleImg.sprite = null;
        marbleImg.enabled = false;
        explain.text = "";
        marble = null;


        // 데이타베이스와 유저 데이타 비교
        foreach (string id in userData.curMarblesId) {
            UserMarble userMarble = resource.marbles.Find(x => x.id == id);

            if (marbleOption.value != 0 && (int)userMarble.Type + 1 != marbleOption.value) continue;

            if (userMarble == null) {
                print("해당 기물 데이타베이스에 존재하지 않음");
                continue;
            }

            // 유저가 가지고 있는 캐릭터 선택 버튼들 활성화
            GameObject btnObj = Instantiate(btnPrefab, uiParent);
            Button btn = btnObj.GetComponent<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            marbleBtns.Add(btnObj);
            image.sprite = userMarble.sprite;

            // 버튼에 함수 등록
            btn.onClick.AddListener(() => SetMarbleSelect(userMarble));
        }
    }



    public void ExitMarbleGRP() {
        // 선택 UI off
        marbleSelectUI.SetActive(false);
    }

    private void SetMarbleSelect(UserMarble userMarble) {
        marbleImg.enabled = true;
        marbleImg.sprite = userMarble.sprite;
        explain.text = userMarble.explain;

        marble = userMarble;
    }

    public void SetPos(int pos) {
        if (marble.prefab == null) {
            print("선택된 기물이 없습니다.");
            IdentifyUIOff();
            return;
        }

        if (selectedMarbles[pos] != null) {
            print("기물이 이미 존재합니다.");
            IdentifyUIOff();
            return;
        }

        if (selectedMarbles.Find(x => x == marble.prefab)) {
            print("이미 해당 유닛이 존재합니다.");
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
            print("제거할 기물이 없습니다.");
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
        // 이부분 버그 날 수도 있으니 잘 보자.
        foreach (GameObject obj in selectedMarbles) {
            if (obj != null && !SettingData.Instance.marbleDeck.Contains(obj)) SettingData.Instance.marbleDeck.Add(obj);
        }
    }


    // 기물 선택 함수

    // 룬 선택 함수
}
