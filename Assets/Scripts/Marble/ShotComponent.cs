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

    OwnerSyncComponent sync;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sync = GetComponent<OwnerSyncComponent>();
    }

    private void Update() {
       

    }

    private void OnMouseDown() {

        if (TurnManager.Instance.IsLoading) return;
        if (GameManager.Instance.marbleMoving) return;
        if (GameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return;
        if (GameManager.Instance.curCost < 1) return;

        MarbleManager.Instance.MarbleMouseDown(this);
    }

    private void OnMouseUp() {

        if (TurnManager.Instance.IsLoading) return;
        if (GameManager.Instance.marbleMoving) return;
        if (GameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return;
        if (GameManager.Instance.curCost < 1) return;

        MarbleManager.Instance.MarbleMouseUp(this);

        if (MarbleManager.Instance.CurDistance >= MarbleManager.Instance.MinDistance) {
            Shot(dir, speed);
        }
        speed = 0f;
        MarbleManager.Instance.CurDistance = 0f;
    }

    private void OnMouseOver() {

        if (TurnManager.Instance.IsLoading) return;
        if (GameManager.Instance.marbleMoving) return;
        if (GameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return;
        if (GameManager.Instance.curCost < 1) return;

        MarbleManager.Instance.MarbleMouseOver();
    }

    private void OnMouseExit() {

        if (TurnManager.Instance.IsLoading) return;
        if (GameManager.Instance.marbleMoving) return;
        if (GameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return;
        if (GameManager.Instance.curCost < 1) return;

        MarbleManager.Instance.MarbleMouseExit();
    }

    private void OnMouseDrag() {
        if (TurnManager.Instance.IsLoading) return;
        if (GameManager.Instance.marbleMoving) return;
        if (GameManager.Instance.IsHost != sync.IsHost || !photonView.IsMine) return;
        if (GameManager.Instance.curCost < 1) return;
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
    private void Shot(Vector2 dir, float speed) {
        rigid.velocity = Vector2.zero;
        rigid.AddForce(dir * speed, ForceMode2D.Impulse);

        GameManager.Instance.UpdateCost(false);
    }

}
