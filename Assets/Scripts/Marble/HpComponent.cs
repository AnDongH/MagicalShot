using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HpComponent : MonoBehaviourPun, IHitable
{
    [SerializeField] float hp;

    GameObject hpObj;
    RectTransform hpBar;
    OwnerSyncComponent sync;

    private void Awake() {
        sync = GetComponent<OwnerSyncComponent>();
    }

    private void Start() {

        if (GameManager.Instance.IsHost == sync.IsHost)
            hpObj = Instantiate(GameManager.Instance.hostHpPrefab, transform.position, Quaternion.identity, GameObject.Find("HpGRP").transform);
        else
            hpObj = Instantiate(GameManager.Instance.guestHpPrefab, transform.position, Quaternion.identity, GameObject.Find("HpGRP").transform);

    }

    private void Update() {
        hpObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.8f, 0));
    }

    public void OnHit(float dmg) {
        hp -= dmg;
        hpBar = hpObj.GetComponentsInChildren<RectTransform>()[1];
        hpBar.SetSizeWithCurrentAnchors(0, (hp / 100) * 20);

        if (hp <= 0) {
            Dead();
        }

        photonView.RPC("setHp", RpcTarget.Others, hp);
    }

    [PunRPC]
    private void setHp(float _hp) {
        hp = _hp;
        hpBar = hpObj.GetComponentsInChildren<RectTransform>()[1];
        hpBar.SetSizeWithCurrentAnchors(0, (hp/100) * 20);


        if (hp <= 0) {
            Dead();
            GameManager.Instance.CheckGame();
        }

    }

    private void Dead() {
        Destroy(hpObj);
        if (GameManager.Instance.IsHost == sync.IsHost) GameManager.Instance.curMyMarbles.Remove(gameObject);
        Destroy(gameObject);

        
    }
}
