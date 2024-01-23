using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackComponent : MonoBehaviourPun {

    [SerializeField] float dmg;

    OwnerSyncComponent sync;
    Animator animator;

    private void Awake() {
        sync = GetComponent<OwnerSyncComponent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Attack(float dmg, IHitable hitable) {
        hitable.OnHit(dmg);
        animator.SetTrigger("Attack");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // 이거 고쳐야 할듯. 일단 각 클라들마다 myTurn 으로 flag를 바꾸고 보여주는걸 다르게 하는걸로 바꿔야겠다.

        IHitable hitable = collision.gameObject.GetComponent<IHitable>();
        OwnerSyncComponent colSync = collision.gameObject.GetComponent<OwnerSyncComponent>();

        if (hitable != null) {

            if (colSync != null) {
                if (colSync.IsHost == sync.IsHost) return;
            }

            if (PhotonNetwork.IsMasterClient) {
                if (TurnManager.Instance.IsHostTurn && GameManager.Instance.IsHost == sync.IsHost) Attack(dmg, hitable);
            }
            else {
                if (!TurnManager.Instance.IsHostTurn && GameManager.Instance.IsHost == sync.IsHost) Attack(dmg, hitable);
            }
        }
    }
}
