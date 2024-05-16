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
    [SerializeField][Tooltip("���� �� �� ��带 ���մϴ�.")] ETurnMode eTurnMode;

    public bool IsLoading { get; private set; }
    public bool IsHostTurn { get; private set; }

    private bool myTurn;

    [SerializeField] private NotificationPanel notificationPanel;


    public static event Action<bool> OnTurnChanged;

    enum ETurnMode { Random, Host, Other }

    // ĳ��
    WaitForSeconds delay20 = new WaitForSeconds(2.0f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    private void Update() {
        if (myTurn) {
            curTurnTime -= Time.deltaTime;
            if (curTurnTime <= 0) {
                EndTurn();
            }
           
        }
    }

    // �� ����ȭ
    [PunRPC]
    private void SetTurn(bool flag) {
        IsHostTurn = flag;
    }

    // �� �ʱ�ȭ, ����ȭ
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
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        IsLoading = true;

        InGameManager.Instance.UpdateCost(true, 6);
        OnTurnChanged?.Invoke(IsHostTurn);
        if (IsHostTurn) {
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "���� ��!" : "����� ��!");
        }
        else {
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "����� ��!" : "���� ��!");
        }
        yield return delay20;
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
        notificationPanel.TurnShow("�¸�!");
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    public IEnumerator LoseTheGameCo() {
        IsLoading = true;
        notificationPanel.TurnShow("�й�");
        yield return delay20;
        PhotonInGameManager.Instance.LeaveRoom();
    }

    [PunRPC]
    private void TurnChange() {
        IsHostTurn = !IsHostTurn;
        StartCoroutine(StartTurnCo());
    }

    public void EndTurn() {
        myTurn = false;
        curTurnTime = defaultTurnTime;
        photonView.RPC("TurnChange", RpcTarget.All);
    }

}
