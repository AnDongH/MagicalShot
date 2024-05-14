using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MarbleManager : MonoBehaviour
{
    public static MarbleManager Instance { get; private set; }
    void Awake() => Instance = this;

    [SerializeField] private GameObject arrowPrefab;
    GameObject arrow;

    [SerializeField] private ShotComponent selectMarble;
    public bool isMyMarbleDrag { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField] public float MinDistance { get; private set; }
    public float CurDistance { get; set; }

    public void MarbleMouseOver() {
        
    }

    public void MarbleMouseExit() {
        
    }

    public void MarbleMouseDown(ShotComponent marble) {
        isMyMarbleDrag = true;
        selectMarble = marble;

        arrow = Instantiate(arrowPrefab, marble.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        arrow.transform.localScale = Vector3.zero;
    }

    public void MarbleMouseUp(ShotComponent marble) {
        isMyMarbleDrag = false;
        selectMarble = marble;

        Destroy(arrow);
    }

    public void MarbleDrag(Vector2 dir, float dis) {
        SetArrowInfo(dir, dis);
    }

    private void SetArrowInfo(Vector2 dir, float dis) {
        // 오브젝트의 각도 계산 (라디안을 각도로 변환)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 오브젝트의 각도 설정
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle + 270);
        arrow.transform.localScale = Vector3.one * dis;
    }
}
