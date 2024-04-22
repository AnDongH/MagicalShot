using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarbleUI : UI_PopUp
{
    enum Buttons {
        TA_SelectPosBtn = 0,
        DD_SelectPosBtn = 1,
        AD_SelectPosBtn = 2,
        AP_SelectPosBtn = 3,
        ExitBtn,
        Set_Btn,
        Del_Btn
    }

    enum Dropdowns {
        MarbleDropdown
    }

    enum Texts {
        ExplainText
    }

    enum Images {
        TA_Img = 0,
        DD_Img = 1,
        AD_Img = 2,
        AP_Img = 3,
        MarbleImg,
    }

    enum Objects {
        Identify_UI
    }

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private Transform btnParent;
    private List<GameObject> marbleBtns = new List<GameObject>();

    protected override void Init() {
        base.Init();

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

        GetDropdown((int)Dropdowns.MarbleDropdown).onValueChanged.AddListener(SetMarbleButton);

        GetObject((int)Objects.Identify_UI).gameObject.SetActive(false);
    }

    private void Start() {
        Init();

        GetDropdown((int)Dropdowns.MarbleDropdown).value = 0;
        SetMarbleButton(0);
    }

    private void OnEnable() {

    }

    private void OnExitBtnClicked(PointerEventData data) {
        ClosePopUpUI();
    }

    private void OnSelectPosBtnClicked(PointerEventData data, int pos) {
        GetObject((int)Objects.Identify_UI).SetActive(true);
        SettingManager.Instance.selectedMarblePos = pos;
    }

    private void OnSetBtnCliked(PointerEventData data) {
        SettingManager.Instance.SetPos();
        SettingManager.Instance.PosImgSet(true, GetImage(SettingManager.Instance.selectedMarblePos));
        GetObject((int)Objects.Identify_UI).SetActive(false);
    }

    private void OnDelBtnCliked(PointerEventData data) {
        SettingManager.Instance.DelPos();
        SettingManager.Instance.PosImgSet(false, GetImage(SettingManager.Instance.selectedMarblePos));
        GetObject((int)Objects.Identify_UI).SetActive(false);
    }

    private void SetMarbleButton(int value) {

        // 생성했던 버튼들 제거
        for (int i = 0; i < marbleBtns.Count; i++) {
            marbleBtns[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            Destroy(marbleBtns[i]);
        }
        marbleBtns.Clear();

        // 기본 UI 정보들 초기화
        GetImage((int)Images.MarbleImg).sprite = null;
        GetImage((int)Images.MarbleImg).enabled = false;
        GetText((int)Texts.ExplainText).text = "";
        SettingManager.Instance.marble = null;


        // 데이타베이스와 유저 데이타 비교
        foreach (string id in SettingManager.Instance.userData.curMarblesId) {
            UserMarble userMarble = SettingManager.Instance.resource.marbles.Find(x => x.id == id);

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
            image.sprite = userMarble.sprite;

            // 버튼에 함수 등록
            btn.onClick.AddListener(() => SetMarbleSelect(userMarble, GetImage((int)Images.MarbleImg), GetText((int)Texts.ExplainText)));
        }
    }

    private void SetMarbleSelect(UserMarble userMarble, Image marbleImg, Text explain) {
        marbleImg.enabled = true;
        marbleImg.sprite = userMarble.sprite;
        explain.text = userMarble.explain;

        SettingManager.Instance.marble = userMarble;
    }

}
