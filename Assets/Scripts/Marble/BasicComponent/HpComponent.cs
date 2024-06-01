using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpComponent : MonoBehaviour, IHitable
{
    private GameObject hpObj;
    private RectTransform hpBar;
    private OwnerSyncComponent sync;
    private BasicMarble marble;

    private GameObject hitEffect;

    private void Awake() {
        sync = GetComponent<OwnerSyncComponent>();
        marble = GetComponent<BasicMarble>();
    }

    private void Start() {

        if (InGameManager.Instance.IsHost == sync.IsHost)
            hpObj = Instantiate(InGameManager.Instance.HostHpPrefab, transform.position, Quaternion.identity, GameObject.Find("HpGRP").transform);
        else
            hpObj = Instantiate(InGameManager.Instance.GuestHpPrefab, transform.position, Quaternion.identity, GameObject.Find("HpGRP").transform);

        hitEffect = Resources.Load<GameObject>(SettingManager.Instance.prefab_envPath + "NormalHit");
    }

    private void Update() {
        hpObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));
    }

    private void OnDestroy() {
        if (hpObj != null) Destroy(hpObj);
    }

    public void OnHit(int dmg) {
        marble.FinalCurHp -= dmg;
        hpBar = hpObj.GetComponentsInChildren<RectTransform>()[1];
        hpBar.SetSizeWithCurrentAnchors(0, ((float)marble.FinalCurHp / marble.FinalMaxHp) * 86);
        Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (marble.FinalCurHp <= 0) {
            Dead();
        }
    }

    private void Dead() {
        Destroy(hpObj);
        if (InGameManager.Instance.IsHost == sync.IsHost) {
            InGameManager.Instance.CurMyMarbles.Remove(gameObject);
            InGameManager.Instance.CheckGame();
        }
        Destroy(gameObject);
    }
}
