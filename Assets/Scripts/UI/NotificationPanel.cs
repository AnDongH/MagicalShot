using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour {
    [SerializeField] Text notificationText;
    // 누구의 턴인지 보여주는 메서드
    public void TurnShow(string message) {
        notificationText.text = message;
        // DOTween의 시퀀스 애니메이션 -> 코루틴과 비슷하다. 일련의 동작들을 순차적으로 실행하는 것이 가능
        Sequence sequence = DOTween.Sequence().Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
                                              .AppendInterval(0.9f)
                                              .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));
    }

    void Start() => ScaleZero();

    // 인스펙터 창에서 즉시 실행 가능하도록 하는 속성 ContextMenu
    [ContextMenu("ScaleOne")]
    void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;
}
