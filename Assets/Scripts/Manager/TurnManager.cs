using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : NormalSingletonPun<TurnManager> {

    public float defaultTurnTime { get; private set; } = 120f;
    public float curTurnTime { get; private set; } = 120f;


    [Header("Develop")]
    [SerializeField][Tooltip("시작 전 턴 모드를 정합니다.")] ETurnMode eTurnMode;

    public bool IsLoading { get; private set; }
    public bool IsHostTurn { get; private set; }

    private bool myTurn;

    [SerializeField] private Text turnText;


    public static event Action<bool> OnTurnChanged;

    enum ETurnMode { Random, Host, Other }

    // 캐싱
    WaitForSeconds delay20 = new WaitForSeconds(2.0f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    private void Start() {

    }

    private void Update() {
        if (myTurn) {
            curTurnTime -= Time.deltaTime;
            if (curTurnTime <= 0) {
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

        turnText.gameObject.SetActive(true);
        turnText.text = "게임 시작. 기물 생성중..";
        yield return delay07;
        InGameManager.Instance.InitMarbles(IsHostTurn);
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        IsLoading = true;

        InGameManager.Instance.UpdateCost(true, 6);
        OnTurnChanged?.Invoke(IsHostTurn);
        turnText.gameObject.SetActive(true);
        if (IsHostTurn) {
            turnText.text = PhotonNetwork.IsMasterClient ? "나의 턴!" : "상대의 턴!";
        }
        else {
            turnText.text = PhotonNetwork.IsMasterClient ? "상대의 턴!" : "나의 턴!";
        }
        yield return delay20;
        turnText.gameObject.SetActive(false);
        IsLoading = false;

        if (IsHostTurn) {
            myTurn = PhotonNetwork.IsMasterClient ? true : false;
        }
        else {
            myTurn = PhotonNetwork.IsMasterClient ? false : true;
        }
    }

    public IEnumerator WinTheGameCo() {
        IsLoading = true;
        turnText.gameObject.SetActive(true);
        turnText.text = "이겼습니다!";
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    public IEnumerator LoseTheGameCo() {
        IsLoading = true;
        turnText.gameObject.SetActive(true);
        turnText.text = "졌습니다..";
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    [PunRPC]
    private void TurnChange() {
        IsHostTurn = !IsHostTurn;
        Debug.Log(1);
        StartCoroutine(StartTurnCo());
    }

    public void EndTurn() {
        myTurn = false;
        curTurnTime = defaultTurnTime;
        photonView.RPC("TurnChange", RpcTarget.All);
    }

}
