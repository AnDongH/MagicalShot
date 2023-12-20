using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPun, IPunObservable
{

    public static GameManager Instance { get; private set; }

    public List<GameObject> curMyMarbles;
    public Transform[] myMarblesPos;
    public Transform[] otherMarblesPos;
    private Transform[] targetPos;

    public int hostID;
    public int guestID;

    public bool marbleMoving;

    [field: SerializeField] public bool IsHost { get; private set; }

    public int curCost = 3;
    public Text costText;

    void Awake() => Instance = this;

    private void Start() {
        // 호스트와 클라이언트의 아이디 초기화
        if (PhotonNetwork.IsMasterClient) hostID = PhotonNetwork.LocalPlayer.ActorNumber;
        else guestID = PhotonNetwork.LocalPlayer.ActorNumber;

        // 시작 턴 설정, 턴에 따라서 기물 생성
        StartGame();
    }

    private void Update() {
        MarbleMoveCheck();
    }

    // 기물 생성
    public void InitMarbles(bool flag) {

        if (PhotonNetwork.IsMasterClient) targetPos = myMarblesPos;
        else targetPos = otherMarblesPos;

        for (int i = 0; i < 4; i++) {

            // 만약 재소환 기능 넣을 시 어떻게 할건지 생각하자.
            curMyMarbles.Add(PhotonNetwork.Instantiate(SettingData.Instance.envPath + SettingData.Instance.marbleDeck[i].name, targetPos[i].position, Quaternion.identity));
            OwnerSyncComponent sync = curMyMarbles[i].GetComponent<OwnerSyncComponent>();

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

    public void StartGame() {
        StartCoroutine(TurnManager.Instance.StartGameCo());
    }

    private void MarbleMoveCheck() {
        marbleMoving = false;
        foreach (GameObject obj in curMyMarbles) {

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

    [PunRPC]
    private void WinTheGame() {
        StartCoroutine(TurnManager.Instance.WinTheGameCo());
    }

    private void LoseTheGame() {
        StartCoroutine(TurnManager.Instance.LoseTheGameCo());
    }

    public void UpdateCost(bool flag) {
        if (flag) curCost = 3;
        else curCost--;
        costText.text = "남은 횟수 : " + curCost.ToString();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        
    }
}
