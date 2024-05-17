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
        // ���� �������� ��
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

        // ���� �� ��������� �ٽ� 100�� ä���ֱ�
        if (itemBuffer.Count == 0) {
            SetUpItemBuffer();
        }

        // �� ���� �ִ� ī�� �̾��ֱ�
        RuneData item = itemBuffer[0];
        itemBuffer.RemoveAt(0);
        return item;
    }

    void SetUpItemBuffer() {

        // �� �����ϱ�
        itemBuffer = new List<RuneData>(20);
        for (int i = 0; i < DataManager.Instance.Resource.runes.Count; i++) {
            RuneData item = DataManager.Instance.Resource.runes[i];
            itemBuffer.Add(item);
        }

        // ��ġ �������� ����
        for (int i = 0; i < itemBuffer.Count; i++) {
            int rand = Random.Range(i, itemBuffer.Count);
            RuneData temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }
    }

    void AddRune() {

        if (myRunes.Count == 6) return;

        // ī�� �� �п��ٰ� �ֱ�
        var runeObject = Instantiate(runePrefab, runeSpawnPoint.position, Utils.QI);
        var rune = runeObject.GetComponent<Rune>();

        // ���� ī�� ���� �ֱ�
        rune.SetUp(PopItem());

        // �� ������, �� ������
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

        // �� ���ʸ� ���� ������ ī��, ��� ���ʸ� ���� ����
        Rune rune = selectRune;
        var dropPos = Utils.MousePos;
        var targetCards = myRunes;

        targetCards.Remove(rune);
        rune.transform.DOKill();
        DestroyImmediate(rune.gameObject); // Destroy�� ȣ�� �� ���������� ���� �Ŀ� �ı��Ǳ⿡ �Ʒ��ʿ� null�� �ִ´� �ص� �̾��� �߰� �ȴ�.
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

    // ī�� �巡�� �޼���
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

    // ī�� ũ�� ����
    void EnLargeCard(bool isEnLarge, Rune rune) {
        if (isEnLarge) {
            Vector3 enlargePos = new Vector3(rune.originPRS.pos.x, -4.8f, -100f);
            rune.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 3.5f), false);
        }
        else
            rune.MoveTransform(rune.originPRS, false);
    }

    // ���� ī�� ���� ���� �ӽ�
    void SetECardState() {
        // �ε����̸� �ƹ��͵� ����
        if (TurnManager.Instance.IsLoading)
            eRuneState = ERuneState.Nothing;
        else if (!TurnManager.Instance.MyTurn || myPutCount == 1)
            eRuneState = ERuneState.CanMouseOver;
        else if (TurnManager.Instance.MyTurn && myPutCount == 0)
            eRuneState = ERuneState.CanMouseDrag;
    }
}
