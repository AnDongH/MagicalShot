using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviourPun {


    public static TurnManager Instance { get; private set; }

    public GameObject timerBar;
    public float defaultTurnTime = 120f;
    [SerializeField] private float curTurnTime = 120f;
    RectTransform timer;


    [Header("Develop")]
    [SerializeField][Tooltip("���� �� �� ��带 ���մϴ�.")] ETurnMode eTurnMode;

    [field: SerializeField] public bool IsLoading { get; private set; }
    [field: SerializeField] public bool IsHostTurn { get; private set; }

    [SerializeField] private bool myTurn;

    [SerializeField] Text turnText;


    public static event Action<bool> OnTurnChanged;

    enum ETurnMode { Random, Host, Other }

    // ĳ��
    WaitForSeconds delay20 = new WaitForSeconds(2.0f);
    WaitForSeconds delay07 = new WaitForSeconds(0.7f);

    void Awake() {
        Instance = this;
        timer = timerBar.GetComponentsInChildren<RectTransform>()[1];
    }

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

    private void LateUpdate() {
        timer.SetSizeWithCurrentAnchors(0, (curTurnTime / defaultTurnTime) * 120);
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

        turnText.gameObject.SetActive(true);
        turnText.text = "���� ����. �⹰ ������..";
        yield return delay07;
        GameManager.Instance.InitMarbles(IsHostTurn);
        StartCoroutine(StartTurnCo());
    }

    IEnumerator StartTurnCo() {
        IsLoading = true;

        GameManager.Instance.UpdateCost(true, 6);
        OnTurnChanged?.Invoke(IsHostTurn);
        turnText.gameObject.SetActive(true);
        if (IsHostTurn) {
            turnText.text = PhotonNetwork.IsMasterClient ? "���� ��!" : "����� ��!";
        }
        else {
            turnText.text = PhotonNetwork.IsMasterClient ? "����� ��!" : "���� ��!";
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
        turnText.text = "�̰���ϴ�!";
        yield return delay20;
        PlayerPrefs.SetInt("mainScene", 1);
        PhotonNetwork.LoadLevel(0);
    }

    public IEnumerator LoseTheGameCo() {
        IsLoading = true;
        turnText.gameObject.SetActive(true);
        turnText.text = "�����ϴ�..";
        yield return delay20;
        PlayerPrefs.SetInt("mainScene", 1);
        PhotonNetwork.LoadLevel(0);
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
