using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ShopUI : UI_PopUp
{
    enum Buttons {
        ExitBtn,
        MarbleOptionBtn,
        RuneOptionBtn,
        BuyItemBtn,
        WarningOkBtn,
    }

    enum Dropdowns {
        MarbleDropdown,
        RuneDropdown
    }

    enum Texts {
        GoldText,
        RuneNameText,
        RuneExplainText,
        WarningText,
        BuyBtnText,
        MarbleNameText,
        MarbleExplainText,
        MarbleTypeText,
        MarbleHPText,
        MarbleDamageText,
        MarbleTypeRuneText,
        MarbleOriginalRuneText,
        InventoryText
    }

    enum Images {
        ExplainImg
    }

    enum Objects {
        WarningGRP,
        RuneCostExplain,
        MarbleGRP,
        RuneGRP
    }

    enum CurShop {
        Marble,
        Rune
    }

    [SerializeField] private GameObject btnPrefab;
    [SerializeField] private GameObject inventoryItemPrefab;
    [SerializeField] private GameObject costPrefab;
    [SerializeField] private Transform btnParent;
    [SerializeField] private Transform inventoryParent;

    private List<GameObject> itemBtns = new List<GameObject>();
    private List<GameObject> curCosts = new List<GameObject>(6);
    private List<GameObject> curInventory = new List<GameObject>();

    private CurShop curShop = CurShop.Marble;

    private object curSelectedItem;
    private GameObject curSelectedBtn;

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
        GetButton((int)Buttons.BuyItemBtn).gameObject.BindEvent(OnBuyBtnClicked);
        GetButton((int)Buttons.MarbleOptionBtn).gameObject.BindEvent(OnMarbleBtnClicked);
        GetButton((int)Buttons.RuneOptionBtn).gameObject.BindEvent(OnRuneBtnClicked);

        GetDropdown((int)Dropdowns.MarbleDropdown).onValueChanged.AddListener(SetMarbleButton);
        GetDropdown((int)Dropdowns.RuneDropdown).onValueChanged.AddListener(SetRuneButton);

        GetObject((int)Objects.WarningGRP).SetActive(false);
        GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);

        curSelectedItem = null;
        curSelectedBtn = null;

        GetDropdown((int)Dropdowns.MarbleDropdown).value = 0;
        SetMarbleButton(0);

        GetText((int)Texts.GoldText).text = DataManager.Instance.userData.money + "G";

        isBinding = true;
    }

    private void OnEnable() {
        Init();

        if (isBinding) {
            curSelectedItem = null;
            curSelectedBtn = null;
            GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);
            GetDropdown((int)Dropdowns.MarbleDropdown).value = 0;
            SetMarbleButton(0);
            GetText((int)Texts.GoldText).text = DataManager.Instance.userData.money + "G";
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

    private void OnMarbleBtnClicked(PointerEventData data) {
        curShop = CurShop.Marble;
        curSelectedItem = null;
        curSelectedBtn = null;

        SetMarbleButton(0);

    }

    private void OnRuneBtnClicked(PointerEventData data) {
        curShop = CurShop.Rune;
        curSelectedItem = null;
        curSelectedBtn = null;

        SetRuneButton(0);

    }

    private void SetMarbleButton(int value) {
        
        GetObject((int)Objects.RuneGRP).SetActive(false);
        GetObject((int)Objects.MarbleGRP).SetActive(true);

        // 생성했던 버튼들 제거
        for (int i = 0; i < itemBtns.Count; i++) {
            Destroy(itemBtns[i]);
        }
        itemBtns.Clear();

        // 기본 UI 정보들 초기화
        GetImage((int)Images.ExplainImg).sprite = null;
        GetImage((int)Images.ExplainImg).enabled = false;
        GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);

        GetText((int)Texts.MarbleExplainText).text = "";
        GetText((int)Texts.MarbleNameText).text = "";
        GetText((int)Texts.MarbleHPText).text = "";
        GetText((int)Texts.MarbleTypeText).text = "";
        GetText((int)Texts.MarbleDamageText).text = "";
        GetText((int)Texts.MarbleTypeRuneText).text = "";
        GetText((int)Texts.MarbleOriginalRuneText).text = "";

        List<MarbleData> targetMarble = DataManager.Instance.Resource.marbles.FindAll(x => !DataManager.Instance.userData.curMarblesId.Contains(x.id));

        foreach (MarbleData marble in targetMarble) {

            if (value != 0 && (int)marble.Type + 1 != value) continue;

            GameObject btnObj = Instantiate(btnPrefab, btnParent);
            Button btn = btnObj.GetComponentInChildren<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            itemBtns.Add(btnObj);
            image.sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == marble.id + "_Image");

            btn.gameObject.BindEvent((data) => OnMarbleItemBtnClicked(data, marble, btnObj));
        }

        foreach (var item in curInventory) Destroy(item);
        curInventory.Clear();

        foreach (var marbleID in DataManager.Instance.userData.curMarblesId) {
            GameObject g = Instantiate(inventoryItemPrefab, inventoryParent);
            Image image = g.GetComponentsInChildren<Image>()[1];
            curInventory.Add(g);
            image.sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == marbleID + "_Image");
        }

        GetText((int)Texts.InventoryText).text = "현재 보유 기물";
    }

    private void SetRuneButton(int value) {

        GetObject((int)Objects.MarbleGRP).SetActive(false);
        GetObject((int)Objects.RuneGRP).SetActive(true);

        // 생성했던 버튼들 제거
        for (int i = 0; i < itemBtns.Count; i++) {
            Destroy(itemBtns[i]);
        }
        itemBtns.Clear();

        // 기본 UI 정보들 초기화
        GetImage((int)Images.ExplainImg).sprite = null;
        GetImage((int)Images.ExplainImg).enabled = false;
        GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);

        GetText((int)Texts.RuneExplainText).text = "";
        GetText((int)Texts.RuneNameText).text = "";

        foreach (var cost in curCosts) Destroy(cost);
        curCosts.Clear();

        List<RuneData> targetRune = DataManager.Instance.Resource.runes.FindAll(x => !DataManager.Instance.userData.curRunesId.Contains(x.id));

        foreach (RuneData rune in targetRune) {

            if (value != 0 && (int)rune.RuneType + 1 != value) continue;

            GameObject btnObj = Instantiate(btnPrefab, btnParent);
            Button btn = btnObj.GetComponentInChildren<Button>();
            Image image = btnObj.GetComponentsInChildren<Image>()[1];
            itemBtns.Add(btnObj);
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == rune.id + "_Image");

            btn.gameObject.BindEvent((data) => OnRuneItemBtnClicked(data, rune, btnObj));
        }

        foreach (var item in curInventory) Destroy(item);
        curInventory.Clear();

        foreach (var runeID in DataManager.Instance.userData.curRunesId) {
            GameObject g = Instantiate(inventoryItemPrefab, inventoryParent);
            Image image = g.GetComponentsInChildren<Image>()[1];
            curInventory.Add(g);
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == runeID + "_Image");
        }

        GetText((int)Texts.InventoryText).text = "현재 보유 룬";
    }

    private void OnMarbleItemBtnClicked(PointerEventData data, MarbleData marble, GameObject btnObj) {
        GetImage((int)Images.ExplainImg).enabled = true;
        GetImage((int)Images.ExplainImg).sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == marble.id + "_Image");

        CommonMarbleData commonMarbleData = DataManager.Instance.Resource.commonMalbles.Find(x => x.Type == marble.Type);

        GetText((int)Texts.MarbleExplainText).text = marble.explain;
        GetText((int)Texts.MarbleNameText).text = marble.name;
        GetText((int)Texts.MarbleHPText).text = "체력: " + (marble.additionalHp + commonMarbleData.basicHp).ToString();
        GetText((int)Texts.MarbleTypeText).text = "포지션: " + MarbleManager.GetType(marble.Type);
        GetText((int)Texts.MarbleDamageText).text = "공력력: " + (marble.additionalDamage + commonMarbleData.basicDamage).ToString();
        GetText((int)Texts.MarbleTypeRuneText).text = MarbleManager.GetType(marble.Type) + " 특성: \n" + DataManager.Instance.Resource.basicRunes.Find(x => x.id == commonMarbleData.typeRuneID).explain;
        GetText((int)Texts.MarbleOriginalRuneText).text = "개인 특성: \n" + DataManager.Instance.Resource.basicRunes.Find(x => x.id == marble.originalRuneID).explain;

        GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(true);
        GetText((int)Texts.BuyBtnText).text = marble.gold + "G";

        curSelectedItem = marble;
        curSelectedBtn = btnObj;
    }

    private void OnRuneItemBtnClicked(PointerEventData data, RuneData rune, GameObject btnObj) {

        GetImage((int)Images.ExplainImg).enabled = true;
        GetImage((int)Images.ExplainImg).sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == rune.id + "_Image");

        GetText((int)Texts.RuneExplainText).text = rune.explain;
        GetText((int)Texts.RuneNameText).text = rune.name;

        foreach (var cost in curCosts) Destroy(cost);
        curCosts.Clear();

        for (int i = 0; i < rune.cost; i++) {
            curCosts.Add(Instantiate(costPrefab, GetObject((int)Objects.RuneCostExplain).transform));
        }

        GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(true);
        GetText((int)Texts.BuyBtnText).text = rune.gold + "G";

        curSelectedItem = rune;
        curSelectedBtn = btnObj;
    }

    private void OnBuyBtnClicked(PointerEventData data) {
        switch (curShop) {
            case CurShop.Marble:

                if (DataManager.Instance.userData.money < (curSelectedItem as MarbleData).gold) {
                    GetObject((int)Objects.WarningGRP).SetActive(true);
                    GetText((int)Texts.WarningText).text = "돈이 부족합니다!";
                    return;
                }

                DataManager.Instance.userData.curMarblesId.Add((curSelectedItem as MarbleData).id);
                GameObject g = Instantiate(inventoryItemPrefab, inventoryParent);
                Image image = g.GetComponentsInChildren<Image>()[1];
                curInventory.Add(g);
                image.sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == (curSelectedItem as MarbleData).id + "_Image");
                itemBtns.Remove(curSelectedBtn);
                Destroy(curSelectedBtn);

                DataManager.Instance.userData.money -= (curSelectedItem as MarbleData).gold;
                GetText((int)Texts.GoldText).text = DataManager.Instance.userData.money + "G";

                GetImage((int)Images.ExplainImg).sprite = null;
                GetImage((int)Images.ExplainImg).enabled = false;
                GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);

                GetText((int)Texts.MarbleExplainText).text = "";
                GetText((int)Texts.MarbleNameText).text = "";
                GetText((int)Texts.MarbleHPText).text = "";
                GetText((int)Texts.MarbleTypeText).text = "";
                GetText((int)Texts.MarbleDamageText).text = "";
                GetText((int)Texts.MarbleTypeRuneText).text = "";
                GetText((int)Texts.MarbleOriginalRuneText).text = "";
                break;
            case CurShop.Rune:

                if (DataManager.Instance.userData.money < (curSelectedItem as RuneData).gold) {
                    GetObject((int)Objects.WarningGRP).SetActive(true);
                    GetText((int)Texts.WarningText).text = "돈이 부족합니다!";
                    return;
                }

                DataManager.Instance.userData.curRunesId.Add((curSelectedItem as RuneData).id);
                GameObject g2 = Instantiate(inventoryItemPrefab, inventoryParent);
                Image image2 = g2.GetComponentsInChildren<Image>()[1];
                curInventory.Add(g2);
                image2.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == (curSelectedItem as RuneData).id + "_Image");
                itemBtns.Remove(curSelectedBtn);
                Destroy(curSelectedBtn);

                DataManager.Instance.userData.money -= (curSelectedItem as RuneData).gold;
                GetText((int)Texts.GoldText).text = DataManager.Instance.userData.money + "G";


                // 기본 UI 정보들 초기화
                GetImage((int)Images.ExplainImg).sprite = null;
                GetImage((int)Images.ExplainImg).enabled = false;
                GetButton((int)Buttons.BuyItemBtn).gameObject.SetActive(false);

                GetText((int)Texts.RuneExplainText).text = "";
                GetText((int)Texts.RuneNameText).text = "";

                foreach (var cost in curCosts) Destroy(cost);
                curCosts.Clear();
                break;
        }
    }

    private void ShowInventoryItemStatus(PointerEventData data) {

    }
}
