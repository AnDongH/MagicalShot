using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> OnExitHandler = null;

    public bool interactable = true;

    public void OnPointerClick(PointerEventData eventData) // Ŭ�� �̺�Ʈ �������̵�
    {
        if (!interactable) return;

        if (OnClickHandler != null)
            OnClickHandler.Invoke(eventData); // Ŭ���� ���õ� �׼� ����
    }

    public void OnDrag(PointerEventData eventData) // �巡�� �̺�Ʈ �������̵�
    {
        if (!interactable) return;

        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData); // �巡�׿� ���õ� �׼� ����
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!interactable) return;

        if (OnEnterHandler != null)
            OnEnterHandler.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!interactable) return;

        if (OnExitHandler != null)
            OnExitHandler.Invoke(eventData);
    }
}
