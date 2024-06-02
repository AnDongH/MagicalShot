using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResultCanvasUI : UI_PopUp
{
    enum Buttons {
        GameExitBtn
    }

    enum Texts {
        NameText,
        WinScoreText,
        GoldText
    }

    enum Images {
        ResultImg
    }

    [SerializeField] private Sprite winImage;
    [SerializeField] private Sprite loseImage;

    protected override void Init() {
        base.Init();
    }

    private void Start() {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetButton((int)Buttons.GameExitBtn).gameObject.BindEvent(OnGameExitBtnClicked);

        Result();
    }

    private void OnEnable() {
        Init();
    }

    private void OnGameExitBtnClicked(PointerEventData data) {
        PhotonInGameManager.Instance.LeaveRoom();
    }

    private void Result() {

        GetText((int)Texts.NameText).text = PhotonNetwork.LocalPlayer.NickName;

        if (InGameManager.Instance.IsWin) {
            GetImage((int)Images.ResultImg).sprite = winImage;
            GetText((int)Texts.WinScoreText).text = "½ÂÁ¡: " + DataManager.Instance.userData.winScore + " (+ " + InGameManager.Instance.BasicWinScore + ")";
            GetText((int)Texts.GoldText).text = "°ñµå: " + DataManager.Instance.userData.money + " (+ " + InGameManager.Instance.BasicWinGold + ")";
        }
        else {
            GetImage((int)Images.ResultImg).sprite = loseImage;
            GetText((int)Texts.WinScoreText).text = "½ÂÁ¡: " + DataManager.Instance.userData.winScore + " (- " + InGameManager.Instance.BasicWinScore + ")";
            GetText((int)Texts.GoldText).text = "°ñµå: " + DataManager.Instance.userData.money + " (+ " + InGameManager.Instance.BasicLoseGold + ")";
        }
    }
}
