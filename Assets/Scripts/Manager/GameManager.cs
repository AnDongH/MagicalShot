using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 게임 메니져에서 전체 클라이언트 관리, 내 클라이언트 관리 분리 생각해보자

public class GameManager : MonoBehaviourPun, IPunObservable
{

    public static GameManager Instance { get; private set; }


    public GameObject menuUI;


    public List<GameObject> curMyMarbles;
    public List<GameObject> curOtherMarbles;
    public GameObject[] curMarbles;

    public Transform[] hostMarblesPos;
    public Transform[] otherMarblesPos;
    private Transform[] targetPos;

    public GameObject hostHpPrefab;
    public GameObject guestHpPrefab;

    public int hostID;
    public int guestID;

    public bool marbleMoving;
    public float constV;

    public int readyCnt;

    [field: SerializeField] public bool IsHost { get; private set; }

    public int curCost = 6;
    public GameObject[] costList;

    void Awake() => Instance = this;

    private void Start() {
        // 호스트와 클라이언트의 아이디 초기화
        if (PhotonNetwork.IsMasterClient) hostID = PhotonNetwork.LocalPlayer.ActorNumber;
        else guestID = PhotonNetwork.LocalPlayer.ActorNumber;

        // 시작 턴 설정, 턴에 따라서 기물 생성

        StartCoroutine(StartGame());

    }

    private void Update() {
        MarbleMoveCheck();
    }

    // 기물 생성
    public void InitMarbles(bool flag) {

        if (PhotonNetwork.IsMasterClient) targetPos = hostMarblesPos;
        else targetPos = otherMarblesPos;

        for (int i = 0; i < 4; i++) {

            // 만약 재소환 기능 넣을 시 어떻게 할건지 생각하자.
            GameObject t = PhotonNetwork.Instantiate(SettingData.Instance.envPath + SettingData.Instance.marbleDeck[i].name, targetPos[i].position, Quaternion.identity);
            OwnerSyncComponent sync = t.GetComponent<OwnerSyncComponent>();

            if (PhotonNetwork.IsMasterClient) {
                sync.SetControlOwner(true);
                IsHost = true;
            }
            else {
                sync.SetControlOwner(false);
                IsHost = false;
            }

            sync.ChangeOwner(flag);
        }

    }


    private IEnumerator StartGame() {
        yield return StartCoroutine(TurnManager.Instance.StartGameCo());

        photonView.RPC("CountReady", RpcTarget.All);

        if (readyCnt >= 2) photonView.RPC("InitMarbleList", RpcTarget.All);
    }

    [PunRPC]
    private void CountReady() {
        readyCnt++;
    }

    [PunRPC]
    private void InitMarbleList() {
        curMarbles = GameObject.FindGameObjectsWithTag("Marble");
        foreach (GameObject m in curMarbles) {
            OwnerSyncComponent sync = m.gameObject.GetComponent<OwnerSyncComponent>();
            if (sync.IsHost == IsHost) curMyMarbles.Add(m);
            else curOtherMarbles.Add(m);
        }
    }


    private void MarbleMoveCheck() {
        if (curMarbles == null) return;
        marbleMoving = false;

        foreach (GameObject obj in curMarbles) {

            if (obj == null) continue;

            Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
            if (rigidbody.velocity != Vector2.zero) marbleMoving = true;
        }
    }

    public void CheckGame() {
        if (curMyMarbles.Count <= 0) {
            LoseTheGame();
            photonView.RPC("WinTheGame", RpcTarget.Others);
        }
    }

    public void MenuUIOn() {
        menuUI.SetActive(true);
    }

    public void MenuUIOff() {
        menuUI.SetActive(false);
    }

    public void GameOver() {
        menuUI.SetActive(false);
        LoseTheGame();
        photonView.RPC("WinTheGame", RpcTarget.Others);

    }

    [PunRPC]
    private void WinTheGame() {
        StartCoroutine(TurnManager.Instance.WinTheGameCo());
    }

    private void LoseTheGame() {
        StartCoroutine(TurnManager.Instance.LoseTheGameCo());
    }

    public void UpdateCost(bool flag, int cost) {
        if (flag) {
            for (int i = curCost; i < cost; i++) {
                costList[i].SetActive(true);
                curCost = cost;
            }
        }
        else {
            for (int i = 0; i < cost; i++) {
                costList[--curCost].SetActive(false);
            }
        }
      
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        
    }
}
