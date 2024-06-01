using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : NormalSingletonPun<TurnManager> {

    public float DefaultTurnTime { get; private set; } = 120f;
    public float CurTurnTime { get; private set; } = 120f;

    public int CurTurnNumber { get; private set; } = 0;

    public int startCardCount = 6;

    [Header("Develop")]
    [SerializeField][Tooltip("시작 전 턴 모드를 정합니다.")] ETurnMode eTurnMode;

    public bool IsLoading { get; private set; }
    public bool IsHostTurn { get; private set; }

    public bool MyTurn { get; private set; }

    [SerializeField] private NotificationPanel notificationPanel;


    public static event Action<bool> OnTurnChanged;
    public static event Action <bool>OnTurnEnded;
    public static event Action <bool>OnTurnStarted;

    enum ETurnMode { Random, Host, Other }

    // 캐싱
    WaitForSeconds delay20 = new WaitForSeconds(2.0f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    private void Update() {
        if (MyTurn) {
            CurTurnTime -= Time.deltaTime;
            if (CurTurnTime <= 0) {
                EndTurn();
            }
           
        }
    }

    // 턴 동기화
    [PunRPC]
    private void SetTurn(bool flag) {
        IsHostTurn = flag;
    }

    // 턴 초기화, 동기화
    public void GameSetUp() {

        switch (eTurnMode) {
            case ETurnMode.Random:
                IsHostTurn = Random.Range(0, 2) == 0;
                break;
            case ETurnMode.Host:
                IsHostTurn = true;
                break;
            case ETurnMode.Other:
                IsHostTurn = false;
                break;
        }
    }


    public IEnumerator StartGameCo() {
        GameSetUp();
        IsLoading = true;

        yield return delay07;
        InGameManager.Instance.InitMarbles(IsHostTurn);

        for (int i = 0; i < startCardCount; i++) {
            // 카드가 추가될 때의 이벤트 발생
            yield return new WaitForSeconds(0.2f);
            RuneManager.Instance.AddRune();
        }

        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {

        CurTurnNumber++;

        IsLoading = true;

        InGameManager.Instance.UpdateCost(true, 6);
        OnTurnChanged?.Invoke(IsHostTurn);
        if (IsHostTurn) {
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "나의 턴!" : "상대의 턴!");
        }
        else {
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "상대의 턴!" : "나의 턴!");
        }
        yield return delay20;

        if (MyTurn) {
            for (int i = 0; i < startCardCount; i++) {
                // 카드가 추가될 때의 이벤트 발생
                yield return new WaitForSeconds(0.2f);
                RuneManager.Instance.AddRune();
            }
        }

        IsLoading = false;

        if (IsHostTurn) {
            MyTurn = PhotonNetwork.IsMasterClient ? true : false;
        }
        else {
            MyTurn = PhotonNetwork.IsMasterClient ? false : true;
        }

        OnTurnStarted?.Invoke(MyTurn);
    }

    public IEnumerator WinTheGameCo() {
        IsLoading = true;
        notificationPanel.TurnShow("승리!");
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    public IEnumerator LoseTheGameCo() {
        IsLoading = true;
        notificationPanel.TurnShow("패배");
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    [PunRPC]
    private void TurnChange() {
        OnTurnEnded?.Invoke(MyTurn);
        IsHostTurn = !IsHostTurn;
        StartCoroutine(StartTurnCo());
    }

    public void EndTurn() {
        CurTurnTime = DefaultTurnTime;
        photonView.RPC("TurnChange", RpcTarget.All);
    }

}
