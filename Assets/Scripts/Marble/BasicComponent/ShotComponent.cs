using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ShotComponent : MonoBehaviourPun
{
   
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float speed = 0f;
    [SerializeField] Vector2 dir = Vector2.zero;
    [SerializeField] BasicMarble marble;

    private Animator animator;
    private Vector3 originFlip = new Vector3 (1, 1, 1);
    private Vector3 revFlip = new Vector3 (-1, 1, 1);

    OwnerSyncComponent sync;

    public bool ActCondition {
        get {
            if (TurnManager.Instance.IsLoading) return false;
            if (InGameManager.Instance.MarbleMoving) return false;
            if (InGameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return false;
            if (InGameManager.Instance.curCost < 1) return false;

            return true;
        }
    }

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sync = GetComponent<OwnerSyncComponent>();
        animator = GetComponentInChildren<Animator>();
        marble= GetComponentInChildren<BasicMarble>();
    }

    private void Update() {

        if (rigid.velocity.magnitude <= InGameManager.Instance.ConstV) {
            rigid.velocity = Vector2.zero;
        }

        if (rigid.velocity != Vector2.zero)
            animator.SetFloat("RunState", 0.5f);
        else
            animator.SetFloat("RunState", 0);

        if (rigid.velocity.x < 0) transform.localScale = originFlip;
        if (rigid.velocity.x > 0) transform.localScale = revFlip;

    }

    private void OnMouseEnter() {
        MarbleManager.Instance.MarbleMouseEnter(this, marble);
    }

    private void OnMouseDown() {
        MarbleManager.Instance.MarbleMouseDown(this, marble);
    }

    private void OnMouseUp() {
        MarbleManager.Instance.MarbleMouseUp(this, marble);

        if (MarbleManager.Instance.CurDistance >= MarbleManager.Instance.MinDistance) {
            photonView.RPC("Shot", RpcTarget.All, dir, speed, marble.AdditionalSpeed);
        }
        speed = 0f;
        MarbleManager.Instance.CurDistance = 0f;
    }

    private void OnMouseExit() {
        MarbleManager.Instance.MarbleMouseExit(this, marble);
    }

    private void OnMouseDrag() {

        if (InGameManager.Instance.gameEnd) return;
        if (!ActCondition) return;
        if (!MarbleManager.Instance.isMyMarbleDrag) return;

        // 기물 드래그
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 mPos = new Vector2(Utils.MousePos.x, Utils.MousePos.y);
        dir = (pos - mPos).normalized;
        MarbleManager.Instance.CurDistance = Vector2.Distance(pos, mPos);

        if (MarbleManager.Instance.CurDistance > MarbleManager.Instance.MaxDistance)
            MarbleManager.Instance.CurDistance = MarbleManager.Instance.MaxDistance;

        speed = MarbleManager.Instance.CurDistance * 300;

        MarbleManager.Instance.MarbleDrag(dir, MarbleManager.Instance.CurDistance);
    }

    // 기물 쏘기
    [PunRPC]
    private void Shot(Vector2 dir, float speed, float additionalSpeed) {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(dir * (speed + additionalSpeed), ForceMode2D.Impulse);

        if (InGameManager.Instance.IsHost == sync.IsHost)
            InGameManager.Instance.UpdateCost(false, 1);
    }

}
