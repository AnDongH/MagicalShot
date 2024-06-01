using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class BasicMarble : MonoBehaviourPun
{
    enum SkillStartTime {
        OnTurnStarted,
        OnTurnEnded
    }

    public MarbleData marbleData;
    public CommonMarbleData commonData;

    private List<BaseRuneSkill> registeredRunes = new List<BaseRuneSkill>(6);
    public IReadOnlyList<BaseRuneSkill> RegisteredRunes => registeredRunes;

    [SerializeField] private string typeBasicSkillID;
    [SerializeField] private string marbleBasicSkillID;
    [SerializeField] private SkillStartTime typeSkillStartTime;
    [SerializeField] private SkillStartTime marbleSkillStartTime;

    public event Action<BasicMarble> OnMarbleCollision;
    public event Action<BasicMarble> OnMarbleEnemyCollision;

    public RuneData TypeBasicSkillData { get; private set; }
    public RuneData MarbleBasicSkillData { get; private set; }

    [field: SerializeField] public int FinalCurHp { get; set; }
    [field: SerializeField] public int FinalMaxHp { get; private set; }
    [field: SerializeField] public int FinalDmg { get; set; }
    [field: SerializeField] public float AdditionalSpeed { get; set; }

    private OwnerSyncComponent sync;
    private Animator animator;

    private void Awake() {
        sync = GetComponent<OwnerSyncComponent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start() {
        marbleData = DataManager.Instance.Resource.marbles.Find(x => name.Contains(x.id));
        commonData = DataManager.Instance.Resource.commonMalbles.Find(x => x.Type == marbleData.Type);
        TypeBasicSkillData = DataManager.Instance.Resource.basicRunes.Find(x => x.id == typeBasicSkillID);
        MarbleBasicSkillData = DataManager.Instance.Resource.basicRunes.Find(x => x.id == marbleBasicSkillID);

        TurnManager.OnTurnStarted += OnTurnStarted;
        TurnManager.OnTurnEnded += OnTurnEnded;

        GetComponent<Rigidbody2D>().mass = commonData.basicMass;
        FinalMaxHp = commonData.basicHp + marbleData.additionalHp;
        FinalCurHp = FinalMaxHp;
        FinalDmg = commonData.basicDamage + marbleData.additionalDamage;
    }

    private void OnDestroy() {
        TurnManager.OnTurnStarted -= OnTurnStarted;
        TurnManager.OnTurnEnded -= OnTurnEnded;
    }

    private void Use(BaseRuneSkill runeSkill) {
        StartCoroutine(HandleSkillUse_CO(runeSkill));
    }

    private IEnumerator HandleSkillUse_CO(BaseRuneSkill runeSkill) {
        runeSkill.Enter(this);
        runeSkill.runeState = BaseRuneSkill.RuneState.Activate;
        yield return StartCoroutine(runeSkill.Activate(this));
        runeSkill.Exit(this);
        runeSkill.runeState = BaseRuneSkill.RuneState.Used;
        Destroy(runeSkill);
    }

    public void RegisterRune(RuneData runeData) {
        string runeDataSer = JsonUtility.ToJson(runeData);
        photonView.RPC("RegisterRune_RPC", RpcTarget.All, runeDataSer);
    }
    public void DoTypeSkill() => RegisterRune(TypeBasicSkillData);
    public void DoMarbleSkill() => RegisterRune(MarbleBasicSkillData);

    [PunRPC]
    protected virtual void RegisterRune_RPC(string runeDataSer) {
        RuneData data = JsonUtility.FromJson<RuneData>(runeDataSer);

        BaseRuneSkill rune = DataManager.Instance.Resource.runeSkills.Find(x => x.name == (data.id + "_Skill"));

        if (rune == null) return;

        BaseRuneSkill newRune = Instantiate(rune);
        newRune.Init(data);

        if (rune.curBoundCount == 0) Use(newRune);
        registeredRunes.Add(newRune);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // 이거 고쳐야 할듯. 일단 각 클라들마다 myTurn 으로 flag를 바꾸고 보여주는걸 다르게 하는걸로 바꿔야겠다.

        OnMarbleCollision?.Invoke(this);

        IHitable hitable = collision.gameObject.GetComponent<IHitable>();
        OwnerSyncComponent colSync = collision.gameObject.GetComponent<OwnerSyncComponent>();

        if (TurnManager.Instance.IsHostTurn == sync.IsHost) {
            if (hitable != null) {

                if (colSync != null) {
                    if (colSync.IsHost != sync.IsHost) {
                        Attack(FinalDmg, hitable);
                        OnMarbleEnemyCollision?.Invoke(this);
                    }
                }

            }
        }

        foreach (BaseRuneSkill rune in registeredRunes) {
            if (rune.curBoundCount > 0) rune.curBoundCount--;
            if (rune.runeState == BaseRuneSkill.RuneState.Ready && rune.curBoundCount == 0) Use(rune);
        }
    }

    private void OnTurnStarted(bool flag) {

        if (flag) {
            if (typeSkillStartTime == SkillStartTime.OnTurnStarted) {
                PhotonNetwork.Instantiate(SettingManager.Instance.prefab_envPath + "TypeSkillEffect", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                DoTypeSkill();
            }
            if (marbleSkillStartTime == SkillStartTime.OnTurnStarted) {
                PhotonNetwork.Instantiate(SettingManager.Instance.prefab_envPath + "MarbleSkillEffect", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                DoMarbleSkill();
            }
        }
    }

    private void OnTurnEnded(bool flag) {
        if (flag) {
            if (typeSkillStartTime == SkillStartTime.OnTurnEnded) {
                PhotonNetwork.Instantiate(SettingManager.Instance.prefab_envPath + "TypeSkillEffect", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                DoTypeSkill();
            }
            if (marbleSkillStartTime == SkillStartTime.OnTurnEnded) {
                PhotonNetwork.Instantiate(SettingManager.Instance.prefab_envPath + "MarbleSkillEffect", transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                DoMarbleSkill();
            }
        }

        registeredRunes.RemoveAll(x => x == null || x.runeState == BaseRuneSkill.RuneState.Used);
        Debug.Log(registeredRunes.Count);

    }

    private void Attack(int dmg, IHitable hitable) {
        hitable.OnHit(dmg);
        animator.SetTrigger("Attack");
    }

}
