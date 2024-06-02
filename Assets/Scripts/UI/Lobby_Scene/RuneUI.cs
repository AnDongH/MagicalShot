using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuneUI : UI_PopUp
{
    enum Buttons {
        ExitBtn,
        WarningOkBtn
    }

    enum Dropdowns {
        RuneDropdown
    }

    enum Texts {
        NameText,
        ExplainText,
        WarningText
    }

    enum Images {
        RuneExplainImg
    }

    enum Objects {
        RuneBox,
        WarningGRP,
        CostExplain
    }

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private GameObject costPrefab;
    [SerializeField] private Transform btnParent;
    private List<RuneBtnContainer> runeBtns = new List<RuneBtnContainer>();
    private List<GameObject> curRunes = new List<GameObject>(20);
    private List<GameObject> curCosts = new List<GameObject>(6);

    public class RuneBtnContainer {
        public GameObject btnObj;
        public RuneData data;

        public RuneBtnContainer(GameObject btnObj, RuneData data) {
            this.btnObj = btnObj;
            this.data = data;
        }
    }

    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));
        Bind<Dropdown>(typeof(Dropdowns));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Objects));


        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
        GetButton((int)Buttons.WarningOkBtn).gameObject.BindEvent(OnWarningOkBtnClicked);

        GetDropdown((int)Dropdowns.RuneDropdown).onValueChanged.AddListener(SetRuneButton);

        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);

        GetDropdown((int)Dropdowns.RuneDropdown).value = 0;
        SetRuneButton(0);

        for (int i = 0; i < GetObject((int)Objects.RuneBox).transform.childCount; i++) {
            GameObject g = GetObject((int)Objects.RuneBox).transform.GetChild(i).gameObject;
            curRunes.Add(g);
        }

        for (int i = 0; i < curRunes.Count; i++) {
            int idx = i;
            curRunes[idx].GetComponentInChildren<Button>().gameObject.BindEvent((data) => DelRuneOnDeck(data, idx));
            curRunes[idx].GetComponentInChildren<Button>().gameObject.BindEvent((data) => ShowRuneInfo(data, idx), Define.UIEvent.Enter);
        }

        RuneSort();

        isBinding = true;
    }

    private void OnEnable() {
        Init();

        if (isBinding) {
            GetDropdown((int)Dropdowns.RuneDropdown).value = 0;
            SetRuneButton(0);
        }
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
        DataManager.Instance.SaveData();
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
    }

    private void SetRuneButton(int value) {

        // 생성했던 버튼들 제거
        for (int i = 0; i < runeBtns.Count; i++) {
            Destroy(runeBtns[i].btnObj);
        }
        runeBtns.Clear();

        // 기본 UI 정보들 초기화
        GetImage((int)Images.RuneExplainImg).sprite = null;
        GetImage((int)Images.RuneExplainImg).enabled = false;
        GetText((int)Texts.ExplainText).text = "";
        GetText((int)Texts.NameText).text = "";
        foreach (var cost in curCosts) Destroy(cost);
        curCosts.Clear();
        SettingManager.Instance.Rune = null;


        // 데이타베이스와 유저 데이타 비교
        foreach (string id in DataManager.Instance.userData.curRunesId) {
            RuneData userRune = DataManager.Instance.Resource.runes.Find(x => x.id == id);

            if (value != 0 && (int)userRune.RuneType + 1 != value) continue;

            if (userRune == null) {
                print("해당 룬 데이타베이스에 존재하지 않음");
                continue;
            }

            // 유저가 가지고 있는 룬 선택 버튼들 활성화
            GameObject btnObj = Instantiate(btnPrefab, btnParent);
            Button btn = btnObj.GetComponentInChildren<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            Text text = btnObj.GetComponentInChildren<Text>();
            RuneBtnContainer container = new RuneBtnContainer(btnObj, userRune);

            text.text = (userRune.maxHasCount - DataManager.Instance.userData.runesDeck.FindAll(x => x == userRune.id).Count).ToString();
            runeBtns.Add(container);
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == id + "_Image");

            btn.gameObject.BindEvent((data) => ShowRuneInfo(data, userRune), Define.UIEvent.Enter);
            btn.gameObject.BindEvent((data) => SetRuneOnDeck(data, container), Define.UIEvent.Click);
        }
    }

    private void ShowRuneInfo(PointerEventData data, RuneData userRune) {
        GetImage((int)Images.RuneExplainImg).enabled = true;
        GetImage((int)Images.RuneExplainImg).sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == userRune.id + "_Image");
        
        GetText((int)Texts.NameText).text = userRune.name;
        GetText((int)Texts.ExplainText).text = userRune.explain;

        foreach (var cost in curCosts) Destroy(cost);
        curCosts.Clear();

        for (int i = 0; i < userRune.cost; i++) {
            curCosts.Add(Instantiate(costPrefab, GetObject((int)Objects.CostExplain).transform));
        }

    }

    private void ShowRuneInfo(PointerEventData data, int idx) {

        if (idx >= DataManager.Instance.userData.runesDeck.Count) return;

        RuneData userRune = DataManager.Instance.Resource.runes.Find(x => x.id == DataManager.Instance.userData.runesDeck[idx]);

        GetImage((int)Images.RuneExplainImg).enabled = true;
        GetImage((int)Images.RuneExplainImg).sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == userRune.id + "_Image");

        GetText((int)Texts.NameText).text = userRune.name;
        GetText((int)Texts.ExplainText).text = userRune.explain;

        foreach (var cost in curCosts) Destroy(cost);
        curCosts.Clear();

        for (int i = 0; i < userRune.cost; i++) {
            curCosts.Add(Instantiate(costPrefab, GetObject((int)Objects.CostExplain).transform));
        }

    }

    private void SetRuneOnDeck(PointerEventData data, RuneBtnContainer container) {
        SettingManager.Instance.Rune = container.data;
        string target = SettingManager.Instance.SetRuneOnDeck();

        if (target != null) {
            GetObject((int)Objects.WarningGRP).SetActive(true);
            GetText((int)Texts.WarningText).text = target;
            return;
        }

        int idx = DataManager.Instance.userData.runesDeck.Count - 1;

        if (idx < 0) return;

        // 넣을 수 있는 룬 개수 계산
        container.btnObj.GetComponentInChildren<Text>().text = (container.data.maxHasCount - DataManager.Instance.userData.runesDeck.FindAll(x => x == container.data.id).Count).ToString();
        curRunes[idx].GetComponentsInChildren<Image>()[1].enabled = true;
        curRunes[idx].GetComponentsInChildren<Image>()[1].sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == DataManager.Instance.userData.runesDeck[idx] + "_Image");
        print("룬 추가");
    }

    private void RuneSort() {
        for (int i = 0; i < DataManager.Instance.userData.runesDeck.Count; i++) {
            curRunes[i].GetComponentsInChildren<Image>()[1].enabled = true;
            curRunes[i].GetComponentsInChildren<Image>()[1].sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == DataManager.Instance.userData.runesDeck[i] + "_Image");
        }
    }

    private void DelRuneOnDeck(PointerEventData data, int idx) {

        if (idx >= DataManager.Instance.userData.runesDeck.Count) {
            GetObject((int)Objects.WarningGRP).SetActive(true);
            GetText((int)Texts.WarningText).text = "룬이 이미 존재하지 않습니다.";
            return;
        }

        GameObject g = runeBtns.Find(x => x.data.id == DataManager.Instance.userData.runesDeck[idx]).btnObj;
        g.GetComponentInChildren<Text>().text = (int.Parse(g.GetComponentInChildren<Text>().text) + 1).ToString();
        DataManager.Instance.userData.runesDeck.RemoveAt(idx);
        curRunes[DataManager.Instance.userData.runesDeck.Count].GetComponentsInChildren<Image>()[1].sprite = null;
        curRunes[DataManager.Instance.userData.runesDeck.Count].GetComponentsInChildren<Image>()[1].enabled = false;
        RuneSort();
    }
}
