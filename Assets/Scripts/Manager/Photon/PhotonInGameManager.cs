using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonInGameManager : NormalSingletonPunCallbacks<PhotonInGameManager>
{
    /// <summary>
    /// 방 나가기
    /// </summary>
    public void LeaveRoom() {
        GameManager.ShowLoadingUI();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        print("방에서 나감");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        InGameManager.Instance.OtherPlayerEscape();
    }

    public override void OnConnectedToMaster() {
        print("서버접속완료");
        PhotonNetwork.LocalPlayer.NickName = PlayFabManager.Instance.NickName;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        GameManager.CloseLoadingUI();
        SceneManager.LoadScene("02Lobby");
        print("로비접속완료");
    }

    protected override void OnApplicationQuit() {
        if (!InGameManager.Instance.IsWin) {
            DataManager.Instance.userData.loseCnt++;
            DataManager.Instance.userData.winScore = (DataManager.Instance.userData.winScore - InGameManager.Instance.BasicWinScore) >= 0 ? DataManager.Instance.userData.winScore - InGameManager.Instance.BasicWinScore : 0;
            DataManager.Instance.userData.money += InGameManager.Instance.BasicLoseGold;
            DataManager.Instance.LocalSaveData();
        }
        else {
            DataManager.Instance.userData.winCnt++;
            DataManager.Instance.userData.winScore += InGameManager.Instance.BasicWinScore;
            DataManager.Instance.userData.money += InGameManager.Instance.BasicWinGold;
            DataManager.Instance.LocalSaveData();
        }
        base.OnApplicationQuit();
    }
}
