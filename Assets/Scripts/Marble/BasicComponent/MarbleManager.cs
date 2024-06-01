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
    [SerializeField] private GameObject aimPrefab;
    private GameObject arrow;
    private GameObject aim;

    [SerializeField] private BasicMarble selectMarble;
    public bool isMyMarbleDrag { get; private set; }
    [field: SerializeField] public float MaxDistance { get; private set; }
    [field: SerializeField] public float MinDistance { get; private set; }
    public float CurDistance { get; set; }

    public static event Action<BasicMarble> OnMarbleEnter;
    public static event Action<BasicMarble> OnMarbleExit;
    public static event Action<BasicMarble> OnMarbleDown;

    public void MarbleMouseEnter(ShotComponent shot, BasicMarble marble) {
        // TODO 마블 정보 UI에 보여주기
        if (isMyMarbleDrag) return;

        OnMarbleEnter?.Invoke(marble);

        if (!shot.ActCondition) return;

        if (aim == null)
            aim = Instantiate(aimPrefab, marble.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
    }

    public void MarbleMouseExit(ShotComponent shot, BasicMarble marble) {
        // TODO 마블 정보 UI에서 없애기

        OnMarbleExit?.Invoke(marble);

        if (!shot.ActCondition) return;

        if (aim != null)
            Destroy(aim);
    }

    public void MarbleMouseDown(ShotComponent shot, BasicMarble marble) {

        if (!shot.ActCondition) return;

        OnMarbleDown?.Invoke(marble);

        isMyMarbleDrag = true;
        selectMarble = marble;

        arrow = Instantiate(arrowPrefab, marble.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        arrow.transform.localScale = Vector3.zero;
    }

    public void MarbleMouseUp(ShotComponent shot, BasicMarble marble) {

        if (!shot.ActCondition) return;

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
