using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WebSocketSharp;

public class MarbleUI : UI_PopUp
{
    enum Buttons {
        TA_SelectPosBtn = 0,
        DD_SelectPosBtn = 1,
        AD_SelectPosBtn = 2,
        AP_SelectPosBtn = 3,
        ExitBtn,
        Set_Btn,
        Del_Btn,
        WarningOkBtn
    }

    enum Dropdowns {
        MarbleDropdown
    }

    enum Texts {
        MarbleNameText,
        MarbleExplainText,
        MarbleTypeText,
        MarbleHPText,
        MarbleDamageText,
        MarbleTypeRuneText,
        MarbleOriginalRuneText,
        WarningText
    }

    enum Images {
        TA_Img = 0,
        DD_Img = 1,
        AD_Img = 2,
        AP_Img = 3,
        MarbleImg,
    }

    enum Objects {
        Identify_UI,
        WarningGRP
    }

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform btnParent;
    private List<GameObject> marbleBtns = new List<GameObject>();

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
        GetButton((int)Buttons.TA_SelectPosBtn).gameObject.BindEvent((data) => OnSelectPosBtnClicked(data, (int)Buttons.TA_SelectPosBtn));
        GetButton((int)Buttons.DD_SelectPosBtn).gameObject.BindEvent((data) => OnSelectPosBtnClicked(data, (int)Buttons.DD_SelectPosBtn));
        GetButton((int)Buttons.AD_SelectPosBtn).gameObject.BindEvent((data) => OnSelectPosBtnClicked(data, (int)Buttons.AD_SelectPosBtn));
        GetButton((int)Buttons.AP_SelectPosBtn).gameObject.BindEvent((data) => OnSelectPosBtnClicked(data, (int)Buttons.AP_SelectPosBtn));
        GetButton((int)Buttons.Set_Btn).gameObject.BindEvent(OnSetBtnCliked);
        GetButton((int)Buttons.Del_Btn).gameObject.BindEvent(OnDelBtnCliked);
        GetButton((int)Buttons.WarningOkBtn).gameObject.BindEvent(OnWarningOkBtnClicked);

        GetDropdown((int)Dropdowns.MarbleDropdown).onValueChanged.AddListener(SetMarbleButton);

        GetObject((int)Objects.Identify_UI).gameObject.SetActive(false);
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);

        GetDropdown((int)Dropdowns.MarbleDropdown).value = 0;
        SetMarbleButton(0);

        for (int i = 0; i < DataManager.Instance.userData.marbleDeck.Length; i++) {
            if (!DataManager.Instance.userData.marbleDeck[i].IsNullOrEmpty())
                SettingManager.Instance.MarblePosImgSet(true, GetImage(i), DataManager.Instance.userData.marbleDeck[i]);
        }

        isBinding = true;
    }

    private void OnEnable() {
        Init();

        if (isBinding) {
            GetDropdown((int)Dropdowns.MarbleDropdown).value = 0;
            SetMarbleButton(0);
        }
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
        DataManager.Instance.SaveData();
    }

    private void OnSelectPosBtnClicked(PointerEventData data, int pos) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.Identify_UI).SetActive(true);
        SettingManager.Instance.SelectedMarblePos = pos;    
    }

    private void OnSetBtnCliked(PointerEventData data) {
        string text = SettingManager.Instance.SetMarblePos(GetImage(SettingManager.Instance.SelectedMarblePos));

        if (text != null) {
            GetObject((int)Objects.WarningGRP).SetActive(true);
            GetText((int)Texts.WarningText).text = text;
        }

        GetObject((int)Objects.Identify_UI).SetActive(false);
    }

    private void OnDelBtnCliked(PointerEventData data) {
        string text = SettingManager.Instance.DelMarblePos(GetImage(SettingManager.Instance.SelectedMarblePos));

        if (text != null) {
            GetObject((int)Objects.WarningGRP).SetActive(true);
            GetText((int)Texts.WarningText).text = text;
        }

        GetObject((int)Objects.Identify_UI).SetActive(false);
    }

    private void OnWarningOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        GetObject((int)Objects.WarningGRP).gameObject.SetActive(false);
    }

    private void SetMarbleButton(int value) {

        // 생성했던 버튼들 제거
        for (int i = 0; i < marbleBtns.Count; i++) {
            Destroy(marbleBtns[i]);
        }
        marbleBtns.Clear();

        // 기본 UI 정보들 초기화
        GetImage((int)Images.MarbleImg).sprite = null;
        GetImage((int)Images.MarbleImg).enabled = false;
        GetText((int)Texts.MarbleExplainText).text = "";
        GetText((int)Texts.MarbleNameText).text = "";
        GetText((int)Texts.MarbleHPText).text = "";
        GetText((int)Texts.MarbleTypeText).text = "";
        GetText((int)Texts.MarbleDamageText).text = "";
        GetText((int)Texts.MarbleTypeRuneText).text = "";
        GetText((int)Texts.MarbleOriginalRuneText).text = "";
        SettingManager.Instance.Marble = null;


        // 데이타베이스와 유저 데이타 비교
        foreach (string id in DataManager.Instance.userData.curMarblesId) {
            MarbleData userMarble = DataManager.Instance.Resource.marbles.Find(x => x.id == id);

            if (value != 0 && (int)userMarble.Type + 1 != value) continue;

            if (userMarble == null) {
                print("해당 기물 데이타베이스에 존재하지 않음");
                continue;
            }

            // 유저가 가지고 있는 캐릭터 선택 버튼들 활성화
            GameObject btnObj = Instantiate(btnPrefab, btnParent);
            Button btn = btnObj.GetComponentInChildren<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            marbleBtns.Add(btnObj);
            image.sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == id + "_Image");

            // 버튼에 함수 등록
            btn.gameObject.BindEvent((data) => SetMarbleSelect(data, userMarble));
            btn.gameObject.BindEvent((data) => ShowMarbleData(data, userMarble), Define.UIEvent.Enter);
        }
    }

    private void SetMarbleSelect(PointerEventData data, MarbleData userMarble) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");

        GetObject((int)Objects.Identify_UI).SetActive(false);

        SettingManager.Instance.Marble = userMarble;
    }

    private void ShowMarbleData(PointerEventData data, MarbleData userMarble) {
        GetImage((int)Images.MarbleImg).enabled = true;
        GetImage((int)Images.MarbleImg).sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == userMarble.id + "_Image");

        CommonMarbleData commonMarbleData = DataManager.Instance.Resource.commonMalbles.Find(x => x.Type == userMarble.Type);

        GetText((int)Texts.MarbleExplainText).text = userMarble.explain;
        GetText((int)Texts.MarbleNameText).text = userMarble.name;
        GetText((int)Texts.MarbleHPText).text = "체력: " + (userMarble.additionalHp + commonMarbleData.basicHp).ToString();
        GetText((int)Texts.MarbleTypeText).text = "포지션: " + MarbleManager.GetType(userMarble.Type);
        GetText((int)Texts.MarbleDamageText).text = "공력력: " + (userMarble.additionalDamage + commonMarbleData.basicDamage).ToString();
        GetText((int)Texts.MarbleTypeRuneText).text = MarbleManager.GetType(userMarble.Type) + " 특성: \n" + DataManager.Instance.Resource.basicRunes.Find(x => x.id == commonMarbleData.typeRuneID).explain;
        GetText((int)Texts.MarbleOriginalRuneText).text = "개인 특성: \n" + DataManager.Instance.Resource.basicRunes.Find(x => x.id == userMarble.originalRuneID).explain;
    }

}
