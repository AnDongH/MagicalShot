using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISmoothMove : MonoBehaviour
{
    [SerializeField] private PRS originPRS;
    [SerializeField] private PRS activatePRS;
    [SerializeField] private float finishTime;

    private RectTransform rectTransform;
    
    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AllTransformMove(bool flag) {
        rectTransform.DOKill();
        PRS targetPRS = flag ? activatePRS : originPRS;
        rectTransform.DOAnchorPosX(targetPRS.pos.x, finishTime);
        rectTransform.DOAnchorPosY(targetPRS.pos.y, finishTime);
        rectTransform.DOScale(targetPRS.scale, finishTime);
        rectTransform.DORotateQuaternion(targetPRS.rot, finishTime);
    }

    public void TransformPosMove(bool flag) {
        rectTransform.DOKill();
        PRS targetPRS = flag ? activatePRS : originPRS;
        rectTransform.DOAnchorPosX(targetPRS.pos.x, finishTime);
        rectTransform.DOAnchorPosY(targetPRS.pos.y, finishTime);
    }

    public void TransformScaleMove(bool flag) {
        rectTransform.DOKill();
        PRS targetPRS = flag ? activatePRS : originPRS;
        rectTransform.DOScale(targetPRS.scale, finishTime);
    }

    public void TransformRotMove(bool flag) {
        rectTransform.DOKill();
        PRS targetPRS = flag ? activatePRS : originPRS;
        rectTransform.DORotateQuaternion(targetPRS.rot, finishTime);
    }
}
