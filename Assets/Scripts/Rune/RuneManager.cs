using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RuneManager : NormalSingletonPun<RuneManager>
{
    public delegate void OnRuneDownHandler(Rune rune);
    public delegate void OnRuneUpHandler(Rune rune);
    public delegate void OnRuneEnterHandler(Rune rune);
    public delegate void OnRuneExitHandler(Rune rune);

    public static event OnRuneDownHandler OnRuneDown;
    public static event OnRuneUpHandler OnRuneUp;
    public static event OnRuneEnterHandler OnRuneEnter;
    public static event OnRuneExitHandler OnRuneExit;


    [SerializeField] private GameObject runePrefab;
    [SerializeField] private List<Rune> myRunes;
    [SerializeField] private Transform runeSpawnPoint;
    [SerializeField] private Transform runeGravePoint;
    [SerializeField] private ERuneState eRuneState;
    [SerializeField] private List<PRS> originCardPRSs;

    public IReadOnlyList<Rune> MyRunes => myRunes;

    [field: SerializeField] public GameObject RuneEffect { get; private set; }
    [field: SerializeField] public RuneBorder RuneBorder { get; private set; }

    private List<RuneData> itemBuffer;
    private List<RuneData> graveBuffer = new List<RuneData>(20);

    private Rune selectedRune;
    private BasicMarble aimedMarble;
    private bool isMyRuneDrag;
    private bool onMyMarbleArea;

    public IReadOnlyList<RuneData> ItemBuffer => itemBuffer;
    public IReadOnlyList<RuneData> GraveBuffer => graveBuffer;

    enum ERuneState { Nothing, CanMouseOver, CanMouseDrag }

    protected override void Awake() {
        base.Awake();
        SetUpItemBuffer();
        TurnManager.OnTurnEnded += OnTurnEnded;
    }

    void Update() {
        if (isMyRuneDrag) {
            RuneDrag();
        }
        DetectMarble();
        SetERuneState();
    }

    void OnDestroy() {
        TurnManager.OnTurnEnded -= OnTurnEnded;
    }

    private void OnTurnEnded(bool myTurn) {
        if (myTurn) StartCoroutine(RuneGoToGrave());
    }

    // 임시
    private IEnumerator RuneGoToGrave() {
        PRS t = new PRS(runeGravePoint.position, Quaternion.identity, new Vector3(1.5f, 1.5f, 1f));
        foreach (var rune in myRunes) {
            if (rune.IsFreeze) continue;
            rune.MoveTransform(t, true, 0.2f);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var rune in myRunes) {
            if (rune.IsFreeze) continue;
            graveBuffer.Add(rune.data);
            Destroy(rune.gameObject);
        }

        yield return null;

        myRunes.RemoveAll(rune => rune == null);
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
        graveBuffer.Clear();
        itemBuffer = new List<RuneData>(20);
        for (int i = 0; i < 20; i++) {                      // 변경 필요
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

    public void AddRune() {

        if (myRunes.Count == 6) return;

        // 카드 내 패에다가 넣기
        var runeObject = Instantiate(runePrefab, runeSpawnPoint.position, Utils.QI);
        var rune = runeObject.GetComponent<Rune>();

        rune.SetUp(PopItem());
        myRunes.Add(rune);

        RuneAlignment();
    }

    private void RuneAlignment() {
        for (int i = 0; i < myRunes.Count; i++) {
            var targetCard = myRunes[i];

            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, 0.7f);
        }
    }

    public bool TryUseRune(BasicMarble marble) {

        if (InGameManager.Instance.curCost < selectedRune.data.cost) return false;

        Rune rune = selectedRune;
        var targetRunes = myRunes;

        PhotonNetwork.Instantiate(SettingManager.Instance.prefab_envPath + "RuneRegisterEffect", marble.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        marble.RegisterRune(rune.data);

        targetRunes.Remove(rune);
        rune.transform.DOKill();
        graveBuffer.Add(rune.data);
        DestroyImmediate(rune.gameObject); // Destroy는 호출 후 한프레임이 지난 후에 파괴되기에 아래쪽에 null을 넣는다 해도 미씽이 뜨게 된다.
        selectedRune = null;
        RuneAlignment();

        InGameManager.Instance.UpdateCost(false, rune.data.cost);

        return true;
    }

    public void RuneMouseEnter(Rune rune) {
        if (InGameManager.Instance.gameEnd) return;
        if (eRuneState == ERuneState.Nothing) return;
        if (isMyRuneDrag) return;

        OnRuneEnter?.Invoke(rune);
    }

    public void RuneMouseOver(Rune rune) {
        if (InGameManager.Instance.gameEnd) return;
        if (eRuneState == ERuneState.Nothing) return;

        selectedRune = rune;
        EnLargeRune(true, rune);
    }


    public void RuneMouseExit(Rune rune) {
        if (InGameManager.Instance.gameEnd) return;
        EnLargeRune(false, rune);

        OnRuneExit?.Invoke(rune);
    }


    public void RuneMouseDown() {
        if (InGameManager.Instance.gameEnd) return;
        if (eRuneState != ERuneState.CanMouseDrag) return;
        if (selectedRune.IsFreeze) return;

        isMyRuneDrag = true;
        OnRuneDown?.Invoke(selectedRune);
        RuneBorder.SetCollider(true);
    }


    public void RuneMouseUp() {
        if (InGameManager.Instance.gameEnd) return;
        isMyRuneDrag = false;

        if (eRuneState != ERuneState.CanMouseDrag) return;

        if (onMyMarbleArea) TryUseRune(aimedMarble);
        else selectedRune.MoveTransform(selectedRune.originPRS, false);

        OnRuneUp?.Invoke(selectedRune);
        RuneBorder.SetCollider(false);
    }

    private void RuneDrag() {
        if (InGameManager.Instance.gameEnd) return;
        if (eRuneState != ERuneState.CanMouseDrag) return;

        selectedRune.MoveTransform(new PRS(Utils.MousePos, Utils.QI, selectedRune.originPRS.scale), false);
    }

    private void DetectMarble() {
        int layerMarble = LayerMask.GetMask("Marble");
        RaycastHit2D hit = Physics2D.Raycast(Utils.MousePos, Vector3.forward, 200f,layerMarble);
        onMyMarbleArea = (hit.collider != null && hit.collider.GetComponent<ShotComponent>().ActCondition);
        if (onMyMarbleArea) aimedMarble = hit.collider.GetComponent<BasicMarble>();
        else aimedMarble = null;
    }

    // 카드 크기 조절
    void EnLargeRune(bool isEnLarge, Rune rune) {
        if (isEnLarge) {
            rune.MoveTransform(new PRS(rune.originPRS.pos + new Vector3(0,0,-50f), Utils.QI, rune.originPRS.scale * 1.5f), false);
            
        }
        else
            rune.MoveTransform(rune.originPRS, false);
    }

    // 나의 카드 선택 상태 머신
    void SetERuneState() {
        // 로딩중이면 아무것도 못함
        if (TurnManager.Instance.IsLoading)
            eRuneState = ERuneState.Nothing;
        else if (!TurnManager.Instance.MyTurn || InGameManager.Instance.curCost <= 0)
            eRuneState = ERuneState.CanMouseOver;
        else if (TurnManager.Instance.MyTurn && InGameManager.Instance.curCost >= 1)
            eRuneState = ERuneState.CanMouseDrag;
    }
}
