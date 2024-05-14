using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NoticePanelUI : MonoBehaviour
{
    [SerializeField] Text noticeText;

    private void Start() {
        if (DataManager.Instance.NoticeString != null)
            DisplayText(DataManager.Instance.NoticeString);
    }
    
    private void DisplayText(string notice) {
        noticeText.text = notice;
        RectTransform rectTransform = noticeText.GetComponent<RectTransform>();
        float textHeight = noticeText.preferredHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, textHeight);
    }
}

