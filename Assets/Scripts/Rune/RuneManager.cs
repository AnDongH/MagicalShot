using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class RuneManager : NormalSingleton<RuneManager>
{
    [SerializeField] GameObject runePrefab;
    [SerializeField] List<Rune> myRunes;
    [SerializeField] Transform runeSpawnPoint;
    [SerializeField] ERuneState eRuneState;
    [SerializeField] List<PRS> originCardPRSs;

    List<RuneData> itemBuffer;
    Rune selectRune;
    bool isMyRuneDrag;
    bool onMyRuneArea;
    int myPutCount;
    enum ERuneState { Nothing, CanMouseOver, CanMouseDrag }

    protected override void Awake() {
        base.Awake();
        SetUpItemBuffer();
        TurnManager.OnAddRune += AddRune;
        TurnManager.OnTurnChanged += OnTurnStarted;
        TurnManager.OnTurnEnded += OnTurnEnded;
    }

    void Update() {
        if (isMyRuneDrag) {
            CardDrag();
        }
        DetectCardArea();
        SetECardState();
    }

    void OnDestroy() {
        TurnManager.OnAddRune -= AddRune;
        TurnManager.OnTurnChanged -= OnTurnStarted;
        TurnManager.OnTurnEnded -= OnTurnEnded;
    }

    void OnTurnStarted(bool myTurn) {
        // 턴이 시작했을 때
        if (myTurn)
            myPutCount = 0;
    }

    void OnTurnEnded() {
        foreach (var rune in myRunes) {
            Destroy(rune.gameObject);
        }
        myRunes.Clear();
    }

    public RuneData PopItem() {

        // 덱이 다 비워졌으면 다시 100장 채워주기
        if (itemBuffer.Count == 0) {
            SetUpItemBuffer();
        }

        // 맨 위에 있는 카드 뽑아주기
        RuneData item = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return item;
    }

    void SetUpItemBuffer() {

        // 덱 구성하기
        itemBuffer = new List<RuneData>(20);
        for (int i = 0; i < DataManager.Instance.Resource.runes.Count; i++) {
            RuneData item = DataManager.Instance.Resource.runes[i];
            itemBuffer.Add(item);
        }

        // 위치 랜덤으로 섞기
        for (int i = 0; i < itemBuffer.Count; i++) {
            int rand = Random.Range(i, itemBuffer.Count);
            RuneData temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }

    void AddRune() {

        if (myRunes.Count == 6) return;

        // 카드 내 패에다가 넣기
        var runeObject = Instantiate(runePrefab, runeSpawnPoint.position, Utils.QI);
        var rune = runeObject.GetComponent<Rune>();

        // 뽑은 카드 정보 넣기
        rune.SetUp(PopItem());

        // 내 패인지, 적 패인지
        myRunes.Add(rune);

        CardAlignment();
    }

    void CardAlignment() {
        for (int i = 0; i < myRunes.Count; i++) {
            var targetCard = myRunes[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    public bool TryUseRune() {

        // 내 차례면 내가 선택한 카드, 상대 차례면 랜덤 선택
        Rune rune = selectRune;
        var dropPos = Utils.MousePos;
        var targetCards = myRunes;

        targetCards.Remove(rune);
        rune.transform.DOKill();
        DestroyImmediate(rune.gameObject); // Destroy는 호출 후 한프레임이 지난 후에 파괴되기에 아래쪽에 null을 넣는다 해도 미씽이 뜨게 된다.
        selectRune = null;
        myPutCount++;
        CardAlignment();

        return true;
    }

    public void RuneMouseOver(Rune rune) {

        if (eRuneState == ERuneState.Nothing) return;

        selectRune = rune;
        EnLargeCard(true, rune);
    }


    public void RuneMouseExit(Rune rune) {
        EnLargeCard(false, rune);
    }


    public void RuneMouseDown() {


        if (eRuneState != ERuneState.CanMouseDrag) return;

        isMyRuneDrag = true;
    }


    public void RuneMouseUp() {

        isMyRuneDrag = false;

        if (eRuneState != ERuneState.CanMouseDrag) return;

        TryUseRune();
    }

    // 카드 드래그 메서드
    void CardDrag() {

        if (eRuneState != ERuneState.CanMouseDrag) return;

        if (!onMyRuneArea) {
            selectRune.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectRune.originPRS.scale), false);
        }
    }

    void DetectCardArea() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Utils.MousePos, Vector3.forward);
        int layer = LayerMask.NameToLayer("MyCardArea");
        onMyRuneArea = Array.Exists(hits, x => x.collider.gameObject.layer == layer);
    }

    // 카드 크기 조절
    void EnLargeCard(bool isEnLarge, Rune rune) {
        if (isEnLarge) {
            Vector3 enlargePos = new Vector3(rune.originPRS.pos.x, -4.8f, -100f);
            rune.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 3.5f), false);
        }
        else
            rune.MoveTransform(rune.originPRS, false);
    }

    // 나의 카드 선택 상태 머신
    void SetECardState() {
        // 로딩중이면 아무것도 못함
        if (TurnManager.Instance.IsLoading)
            eRuneState = ERuneState.Nothing;
        else if (!TurnManager.Instance.MyTurn || myPutCount == 1)
            eRuneState = ERuneState.CanMouseOver;
        else if (TurnManager.Instance.MyTurn && myPutCount == 0)
            eRuneState = ERuneState.CanMouseDrag;
    }
}
