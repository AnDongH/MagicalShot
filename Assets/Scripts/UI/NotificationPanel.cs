using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour {
    [SerializeField] Text notificationText;
    // ������ ������ �����ִ� �޼���
    public void TurnShow(string message) {
        notificationText.text = message;
        // DOTween�� ������ �ִϸ��̼� -> �ڷ�ƾ�� ����ϴ�. �Ϸ��� ���۵��� ���������� �����ϴ� ���� ����
        Sequence sequence = DOTween.Sequence().Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad))
                                              .AppendInterval(0.9f)
                                              .Append(transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutQuad));
    }

    void Start() => ScaleZero();

    // �ν����� â���� ��� ���� �����ϵ��� �ϴ� �Ӽ� ContextMenu
    [ContextMenu("ScaleOne")]
    void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;
}
