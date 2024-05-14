using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameUI : UI_Scene
{
    enum Buttons {
        TurnBtn,
        MenuBtn
    }

    enum Images {
        TimerIMG
    }

    enum Texts {
        TurnText
    }

    enum Objects {
        TimerBar,
        CostList
    }

    private Transform[] costList;
    private RectTransform timer; 

    protected override void Init() {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(Objects));

        GetButton((int)Buttons.TurnBtn).gameObject.BindEvent(OnTurnBtnClicked);
        GetButton((int)Buttons.MenuBtn).gameObject.BindEvent(OnMenuBtnClicked);
    }

    private void Start() {
        Init();

        GetText((int)Texts.TurnText).gameObject.SetActive(false);

        timer = GetObject((int)Objects.TimerBar).GetComponentsInChildren<RectTransform>()[1];

        costList = new Transform[GetObject((int)Objects.CostList).transform.childCount];
        for (int i = 0; i < GetObject((int)Objects.CostList).transform.childCount; i++) {
            costList[i] = GetObject((int)Objects.CostList).transform.GetChild(i);
        }

        InGameManager.OnCostUpdated += UpdateCost;
    }

    private void LateUpdate() {
        timer.SetSizeWithCurrentAnchors(0, (TurnManager.Instance.curTurnTime / TurnManager.Instance.defaultTurnTime) * 240);
    }

    private void OnDestroy() {
        InGameManager.OnCostUpdated -= UpdateCost;
    }

    private void OnTurnBtnClicked(PointerEventData data) {
        TurnManager.Instance.EndTurn();
    }

    private void OnMenuBtnClicked(PointerEventData data) {
        UI_Manager.Instance.ShowPopupUI<UI_PopUp>("MenuPanelUI");
    }

    private void UpdateCost(bool flag, int cost) {
        if (flag) {
            for (int i = InGameManager.Instance.curCost; i < cost; i++) {
                costList[i].gameObject.SetActive(true);
                InGameManager.Instance.curCost = cost;
            }
        }
        else {
            for (int i = 0; i < cost; i++) {
                costList[--InGameManager.Instance.curCost].gameObject.SetActive(false);
            }
        }

    }
}
