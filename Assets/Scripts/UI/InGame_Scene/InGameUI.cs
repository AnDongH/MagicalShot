using DG.Tweening;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class InGameUI : UI_Scene
{
    enum Texts {
        MarbleNameText,
        MarbleTypeText,
        MarbleHpText,
        MarbleDmgText,
        MarbleSkillNameText,
        MarbleSkillExplainText,
        RuneNameText,
        RuneExplainText,
        TurnNumberText,
        RuneContainerText
    }

    enum Buttons {
        TurnBtn,
        MenuBtn,
        RuneContainerExitBtn,
        RuneBagBtn,
        RuneGraveBtn,
        RuneFreezePanelExitBtn,
        RuneFreezerBtn
    }

    enum Images {
        TimerIMG,
        MarbleImg,
        RuneImg
    }

    enum Objects {
        TimerBar,
        CostList,
        NotificationPanel,
        MarbleBasicStatus,
        RuneSkillStatus,
        MarbleSkillStatus,
        RegisteredRunes,
        CostExplainGRP,
        RuneStatus,
        RuneContainer,
        RuneContainerRuneList,
        RuneFreezePanel,
        FreezedRunes
    }

    [SerializeField] private GameObject registeredRune;
    [SerializeField] private GameObject freezedRune;
    [SerializeField] private GameObject costImgPrefab;

    private List<GameObject> registeredRuneList = new List<GameObject>(6);
    private List<GameObject> curContainerRuneList = new List<GameObject>(20);
    private List<GameObject> freezedRuneList = new List<GameObject>(6);

    private Transform runeParent;

    private Transform[] costList;
    private List<GameObject> costExplains = new List<GameObject>(6);
    private RectTransform timer;


    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(Objects));

        GetButton((int)Buttons.TurnBtn).gameObject.BindEvent(OnTurnBtnClicked);
        GetButton((int)Buttons.MenuBtn).gameObject.BindEvent(OnMenuBtnClicked);
        GetButton((int)Buttons.RuneBagBtn).gameObject.BindEvent(OnRuneBagBtnClicked);
        GetButton((int)Buttons.RuneGraveBtn).gameObject.BindEvent(OnRuneGraveBtnClicked);
        GetButton((int)Buttons.RuneContainerExitBtn).gameObject.BindEvent(OnRuneContainerExitBtnClicked);

        GetButton((int)Buttons.RuneFreezerBtn).gameObject.BindEvent(OnRuneFreezerBtnClicked);
        GetButton((int)Buttons.RuneFreezePanelExitBtn).gameObject.BindEvent(OnRuneFreezePanelExitBtnClicked);

        timer = GetObject((int)Objects.TimerBar).GetComponentsInChildren<RectTransform>()[1];

        costList = new Transform[GetObject((int)Objects.CostList).transform.childCount];
        for (int i = 0; i < GetObject((int)Objects.CostList).transform.childCount; i++) {
            costList[i] = GetObject((int)Objects.CostList).transform.GetChild(i);
        }

        runeParent = GetObject((int)Objects.RegisteredRunes).transform;

        InGameManager.OnCostUpdated += UpdateCost;
        MarbleManager.OnMarbleEnter += ShowMarbleStatus;
        MarbleManager.OnMarbleExit += ExitMarbleStatus;
        MarbleManager.OnMarbleDown += ExitMarbleStatus;
        RuneManager.OnRuneEnter += ShowRuneStatus;
        RuneManager.OnRuneExit += ExitRuneStatus;
        RuneManager.OnRuneDown += ExitRuneStatus;
        TurnManager.OnTurnStarted += UpdateTurnNumber;
    }

    private void OnEnable() {
        Init();
    }

    private void LateUpdate() {
        timer.SetSizeWithCurrentAnchors(0, (TurnManager.Instance.CurTurnTime / TurnManager.Instance.DefaultTurnTime) * 240);
    }

    private void OnDestroy() {
        InGameManager.OnCostUpdated -= UpdateCost;
        MarbleManager.OnMarbleEnter -= ShowMarbleStatus;
        MarbleManager.OnMarbleExit -= ExitMarbleStatus;
        MarbleManager.OnMarbleDown -= ExitMarbleStatus;
        RuneManager.OnRuneEnter -= ShowRuneStatus;
        RuneManager.OnRuneExit -= ExitRuneStatus;
        RuneManager.OnRuneDown -= ExitRuneStatus;
        TurnManager.OnTurnStarted -= UpdateTurnNumber;
    }

    private void OnTurnBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("TurnChange");
        TurnManager.Instance.EndTurn();
    }

    private void OnMenuBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("MenuPanelUI");
    }

    private void UpdateCost(bool flag, int cost) {
        StartCoroutine(UpdateCost_CO(flag, cost));
    }

    private IEnumerator UpdateCost_CO(bool flag, int cost) {
        if (flag) {
            for (int i = InGameManager.Instance.curCost; i < cost; i++) {
                CostShow(costList[i].gameObject, flag);
                yield return new WaitForSeconds(0.2f);
            }
            InGameManager.Instance.curCost = cost;
        }
        else {
            for (int i = 0; i < cost; i++) {
                CostShow(costList[--InGameManager.Instance.curCost].gameObject, flag);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void CostShow(GameObject t, bool flag) {
        Vector3 tarVec = flag ? Vector3.one : Vector3.zero;
        t.transform.DOScale(tarVec, 0.3f).SetEase(Ease.InOutQuad);
    }

    private void ShowMarbleStatus(BasicMarble marble) {
        GetImage((int)Images.MarbleImg).sprite = DataManager.Instance.Resource.marbleImages.Find(x => x.name == marble.marbleData.id + "_Image");
        GetText((int)Texts.MarbleNameText).text = marble.marbleData.name;
        GetText((int)Texts.MarbleTypeText).text = "Æ÷Áö¼Ç: " + MarbleManager.GetType(marble.marbleData.Type);
        GetText((int)Texts.MarbleHpText).text = "Ã¼·Â: " + marble.FinalCurHp.ToString() + " / " + marble.FinalMaxHp.ToString();
        GetText((int)Texts.MarbleDmgText).text = "°ø°Ý·Â: " + marble.FinalDmg.ToString();

        GetText((int)Texts.MarbleSkillNameText).text = marble.MarbleBasicSkillData.name;
        GetText((int)Texts.MarbleSkillExplainText).text = marble.MarbleBasicSkillData.explain;

        foreach (var rune in registeredRuneList) {
            Destroy(rune);
        }
        registeredRuneList.Clear();

        foreach (var rune in marble.RegisteredRunes) {

            if (rune.runeState == BaseRuneSkill.RuneState.Used) continue;

            GameObject g = Instantiate(registeredRune, runeParent);
            Image image = g.GetComponentsInChildren<Image>()[1];
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == rune.runeData.id + "_Image");
            registeredRuneList.Add(g);
        }

        GetObject((int)Objects.MarbleBasicStatus).GetComponent<UISmoothMove>().AllTransformMove(true);
        GetObject((int)Objects.MarbleSkillStatus).GetComponent<UISmoothMove>().AllTransformMove(true);
        GetObject((int)Objects.RuneSkillStatus).GetComponent<UISmoothMove>().AllTransformMove(true);
    }

    private void ExitMarbleStatus(BasicMarble marble) {
        GetObject((int)Objects.MarbleBasicStatus).GetComponent<UISmoothMove>().AllTransformMove(false);
        GetObject((int)Objects.MarbleSkillStatus).GetComponent<UISmoothMove>().AllTransformMove(false);
        GetObject((int)Objects.RuneSkillStatus).GetComponent<UISmoothMove>().AllTransformMove(false);
    }

    private void ShowRuneStatus(Rune rune) {
        GetText((int)Texts.RuneNameText).text = rune.data.name;
        GetText((int)Texts.RuneExplainText).text = rune.data.explain;
        GetImage((int)Images.RuneImg).sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == rune.data.id + "_Image");
        
        foreach(var cost in costExplains) Destroy(cost);
        costExplains.Clear();

        for (int i = 0; i < rune.data.cost; i++) {
            costExplains.Add(Instantiate(costImgPrefab, GetObject((int)Objects.CostExplainGRP).transform));
        }

        GetObject((int)Objects.RuneStatus).GetComponent<UISmoothMove>().AllTransformMove(true);
    }

    private void ExitRuneStatus(Rune rune) {
        GetObject((int)Objects.RuneStatus).GetComponent<UISmoothMove>().AllTransformMove(false);
    }

    private void UpdateTurnNumber(bool flag) {
        GetText((int)Texts.TurnNumberText).text = TurnManager.Instance.CurTurnNumber.ToString();
        if (flag)
            GetButton((int)Buttons.RuneFreezerBtn).GetComponent<UISmoothMove>().TransformScaleMove(false);
    }

    private void OnRuneBagBtnClicked(PointerEventData data) {

        RuneManager.Instance.RuneBorder.SetCollider(true);

        GetText((int)Texts.RuneContainerText).text = "ÇöÀç ·é";
        
        foreach (var rune in curContainerRuneList) Destroy(rune);
        curContainerRuneList.Clear();

        // µ¦ ±¸¼ºÇÏ±â
        List<RuneData> itemBuffer = new List<RuneData>(20);
        for (int i = 0; i < RuneManager.Instance.ItemBuffer.Count; i++) {
            RuneData item = RuneManager.Instance.ItemBuffer[i];
            itemBuffer.Add(item);
        }

        // À§Ä¡ ·£´ýÀ¸·Î ¼¯±â
        for (int i = 0; i < itemBuffer.Count; i++) {
            int rand = Random.Range(i, itemBuffer.Count);
            RuneData temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }

        foreach (var runeData in itemBuffer) {
            GameObject g = Instantiate(registeredRune, GetObject((int)Objects.RuneContainerRuneList).transform);
            Image image = g.GetComponentsInChildren<Image>()[1];
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == runeData.id + "_Image");
            curContainerRuneList.Add(g);
        }

        GetObject((int)Objects.RuneContainer).GetComponent<UISmoothMove>().AllTransformMove(true);
    }

    private void OnRuneGraveBtnClicked(PointerEventData data) {

        RuneManager.Instance.RuneBorder.SetCollider(true);

        GetText((int)Texts.RuneContainerText).text = "Æó±âµÈ ·é";

        foreach (var rune in curContainerRuneList) Destroy(rune);
        curContainerRuneList.Clear();

        // µ¦ ±¸¼ºÇÏ±â
        List<RuneData> itemBuffer = new List<RuneData>(20);
        for (int i = 0; i < RuneManager.Instance.GraveBuffer.Count; i++) {
            RuneData item = RuneManager.Instance.GraveBuffer[i];
            itemBuffer.Add(item);
        }

        // À§Ä¡ ·£´ýÀ¸·Î ¼¯±â
        for (int i = 0; i < itemBuffer.Count; i++) {
            int rand = Random.Range(i, itemBuffer.Count);
            RuneData temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }

        foreach (var runeData in itemBuffer) {
            GameObject g = Instantiate(registeredRune, GetObject((int)Objects.RuneContainerRuneList).transform);
            Image image = g.GetComponentsInChildren<Image>()[1];
            image.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == runeData.id + "_Image");
            curContainerRuneList.Add(g);
        }

        GetObject((int)Objects.RuneContainer).GetComponent<UISmoothMove>().AllTransformMove(true);
    }

    private void OnRuneContainerExitBtnClicked(PointerEventData data) {
        RuneManager.Instance.RuneBorder.SetCollider(false);
        GetObject((int)Objects.RuneContainer).GetComponent<UISmoothMove>().AllTransformMove(false);
    }

    private void OnRuneFreezerBtnClicked(PointerEventData data) {

        GetButton((int)Buttons.RuneFreezerBtn).GetComponent<UISmoothMove>().TransformScaleMove(true);

        foreach (var freezedRune in freezedRuneList) Destroy(freezedRune);
        freezedRuneList.Clear();

        foreach (var rune in RuneManager.Instance.MyRunes) {
            GameObject g = Instantiate(freezedRune, GetObject((int)Objects.FreezedRunes).transform);
            g.GetComponentsInChildren<Image>()[1].sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == rune.data.id + "_Image");
            g.BindEvent((data) => OnOnFreezedRuneClicked(data, rune));
            freezedRuneList.Add(g);
        }

        GetObject((int)Objects.RuneFreezePanel).GetComponent<UISmoothMove>().TransformPosMove(true);
    }

    private void OnOnFreezedRuneClicked(PointerEventData data, Rune rune) {

        rune.SetFreeze();

        GetObject((int)Objects.RuneFreezePanel).GetComponent<UISmoothMove>().TransformPosMove(false);
    }

    private void OnRuneFreezePanelExitBtnClicked(PointerEventData data) {

        GetButton((int)Buttons.RuneFreezerBtn).GetComponent<UISmoothMove>().TransformScaleMove(false);
        GetObject((int)Objects.RuneFreezePanel).GetComponent<UISmoothMove>().TransformPosMove(false);
    }
}
