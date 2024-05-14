using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTextHeight : MonoBehaviour {
    public Text textComponent;
    public RectTransform rectTransform;


    // �ؽ�Ʈ ������ ����� ������ ȣ��Ǵ� �Լ�
    void UpdateTextHeight(string newText) {
        // Text UI�� ũ�⸦ ����Ͽ� Rect Transform�� ���� ������Ʈ
        float textHeight = textComponent.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textHeight);
    }
}
