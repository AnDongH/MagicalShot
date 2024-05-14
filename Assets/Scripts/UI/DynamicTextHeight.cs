using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicTextHeight : MonoBehaviour {
    public Text textComponent;
    public RectTransform rectTransform;


    // 텍스트 내용이 변경될 때마다 호출되는 함수
    void UpdateTextHeight(string newText) {
        // Text UI의 크기를 계산하여 Rect Transform의 높이 업데이트
        float textHeight = textComponent.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textHeight);
    }
}
