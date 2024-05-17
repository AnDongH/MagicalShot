using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 게임 메니져에서 전체 클라이언트 관리, 내 클라이언트 관리 분리 생각해보자

public class InGameManager : NormalSingletonPun<InGameManager>, IPunObservable
{

    #region InGameUI delegate
    public delegate void OnCostUpdatedHandler(bool flag, int cost);
    #endregion

    #region InGameUI event
    public static event OnCostUpdatedHandler OnCostUpdated;
    #endregion

    public List<GameObject> CurMyMarbles { get; private set; }
    public List<GameObject> CurOtherMarbles { get; private set; }
    public GameObject[] curMarbles { get; private set; }

    [SerializeField] private Transform[] hostMarblesPos;
    [SerializeField] private Transform[] otherMarblesPos;
    private Transform[] targetPos;

    [field: SerializeField] public GameObject HostHpPrefab { get; private set; }
    [field: SerializeField] public GameObject GuestHpPrefab { get; private set; }

    public int HostID { get; private set; }
    public int GuestID { get; private set; }

    public bool MarbleMoving { get; private set; }
    [field: SerializeField] public float ConstV { get; private set; }

    private int readyCnt = 0;

    public bool IsHost { get; private set; }

    public int curCost = 6;

    public bool gameEnd = false;


    [SerializeField] private GameObject[] mapArray;


    protected override void Awake() {
        base.Awake();
        CurMyMarbles = new List<GameObject>();
        CurOtherMarbles = new List<GameObject>();
    }

    private void Start() {
        // 호스트와 클라이언트의 아이디 초기화
        if (PhotonNetwork.IsMasterClient) HostID = PhotonNetwork.LocalPlayer.ActorNumber;
        else GuestID = PhotonNetwork.LocalPlayer.ActorNumber;

        // 맵 설정
        foreach (GameObject obj in mapArray) {
            obj.SetActive(false);
        }
        mapArray[DataManager.Instance.mapIndex].SetActive(true);
        SoundManager.Instance.PlayBackGroundSound(mapArray[DataManager.Instance.mapIndex].name);

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
            GameObject t = PhotonNetwork.Instantiate(SettingManager.Instance.envPath + DataManager.Instance.userData.marbleDeck[i], targetPos[i].position, Quaternion.identity);
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
    }

    [PunRPC]
    private void CountReady() {
        readyCnt++;
        if (readyCnt >= 2) photonView.RPC("InitMarbleList", RpcTarget.All);
    }

    [PunRPC]
    private void InitMarbleList() {
        curMarbles = GameObject.FindGameObjectsWithTag("Marble");
        foreach (GameObject m in curMarbles) {
            OwnerSyncComponent sync = m.gameObject.GetComponent<OwnerSyncComponent>();
            if (sync.IsHost == IsHost) CurMyMarbles.Add(m);
            else CurOtherMarbles.Add(m);
        }
    }


    private void MarbleMoveCheck() {
        if (curMarbles == null) return;
        MarbleMoving = false;

        foreach (GameObject obj in curMarbles) {

            if (obj == null) continue;

            Rigidbody2D rigidbody = obj.GetComponent<Rigidbody2D>();
            if (rigidbody.velocity != Vector2.zero) MarbleMoving = true;
        }
    }

    public void CheckGame() {
        if (CurMyMarbles.Count <= 0) {
            LoseTheGame();
            photonView.RPC("WinTheGame", RpcTarget.Others);
        }
    }

    public void GiveUp() {
        LoseTheGame();
        photonView.RPC("WinTheGame", RpcTarget.Others);
    }

    [PunRPC]
    private void WinTheGame() {

        if (gameEnd) return;
        else gameEnd = true;

        SoundManager.Instance.BackGroundSoundStop();
        SoundManager.Instance.PlaySFXSound("WinTheGame");
        StartCoroutine(TurnManager.Instance.WinTheGameCo());
        DataManager.Instance.userData.winCnt++;
        DataManager.Instance.userData.winScore += 10;
        DataManager.Instance.userData.money += 100;
        PlayFabManager.Instance.SendStatisticToServer(DataManager.Instance.userData.winScore, "WinScore");

        DataManager.Instance.SaveData();
    }

    public void OtherPlayerEscape() => WinTheGame();

    private void LoseTheGame() {

        if (gameEnd) return;
        else gameEnd = true;

        SoundManager.Instance.BackGroundSoundStop();
        SoundManager.Instance.PlaySFXSound("LoseTheGame");
        StartCoroutine(TurnManager.Instance.LoseTheGameCo());
        DataManager.Instance.userData.loseCnt++;
        DataManager.Instance.userData.winScore = (DataManager.Instance.userData.winScore - 10) >= 0 ? DataManager.Instance.userData.winScore - 10 : 0;
        DataManager.Instance.userData.money += 20;
        PlayFabManager.Instance.SendStatisticToServer(DataManager.Instance.userData.winScore, "WinScore");

        DataManager.Instance.SaveData();
    }

    public void UpdateCost(bool flag, int cost) {
        OnCostUpdated?.Invoke(flag, cost);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        
    }
}
