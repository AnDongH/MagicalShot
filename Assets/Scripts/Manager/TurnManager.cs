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
    [SerializeField][Tooltip("���� �� �� ��带 ���մϴ�.")] ETurnMode eTurnMode;

    public bool IsLoading { get; private set; }
    public bool IsHostTurn { get; private set; }

    public bool MyTurn { get; private set; }

    [SerializeField] private NotificationPanel notificationPanel;


    public static event Action<bool> OnTurnChanged;
    public static event Action <bool>OnTurnEnded;
    public static event Action <bool>OnTurnStarted;

    enum ETurnMode { Random, Host, Other }

    // ĳ��
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

        for (int i = 0; i < startCardCount; i++) {
            // ī�尡 �߰��� ���� �̺�Ʈ �߻�
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
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "���� ��!" : "����� ��!");
        }
        else {
            notificationPanel.TurnShow(PhotonNetwork.IsMasterClient ? "����� ��!" : "���� ��!");
        }
        yield return delay20;

        if (MyTurn) {
            for (int i = 0; i < startCardCount; i++) {
                // ī�尡 �߰��� ���� �̺�Ʈ �߻�
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
        OnTurnEnded?.Invoke(MyTurn);
        IsHostTurn = !IsHostTurn;
        StartCoroutine(StartTurnCo());
    }

    public void EndTurn() {
        CurTurnTime = DefaultTurnTime;
        photonView.RPC("TurnChange", RpcTarget.All);
    }

}
