using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RankingUI : UI_PopUp
{
    enum Buttons {
        ExitBtn
    }

    enum Objects {
        RankContent
    }

    [SerializeField] private GameObject rankUnit;
    private List<GameObject> rankList = new List<GameObject>();

    protected override void Init() {
        base.Init();

        Bind<GameObject>(typeof(Objects));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
    }

    private void Start() {
        Init();
        PlayFabManager.OnRankGet += OnLeaderboardGet;
    }

    private void OnEnable() {
        Init();
    }

    private void OnDestroy() {
        PlayFabManager.OnRankGet -= OnLeaderboardGet;
    }

    private void OnLeaderboardGet(List<PlayerLeaderboardEntry> playerList) {

        foreach (var item in rankList) Destroy(item);
        rankList.Clear();

        foreach (var entry in playerList) {
            GameObject obj = Instantiate(rankUnit, GetObject((int)Objects.RankContent).transform);
            obj.transform.Find("RankText").GetComponent<Text>().text = (entry.Position + 1).ToString();
            obj.transform.Find("NameText").GetComponent<Text>().text = entry.DisplayName;
            obj.transform.Find("ScoreText").GetComponent<Text>().text = entry.StatValue.ToString();
            rankList.Add(obj);
        }

    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }
}
