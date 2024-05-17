using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public PRS originPRS;
    public RuneData data;
    [SerializeField] SpriteRenderer runeImage;

    void OnMouseOver() {
        RuneManager.Instance.RuneMouseOver(this);
    }

    void OnMouseExit() {
        RuneManager.Instance.RuneMouseExit(this);
    }

    void OnMouseDown() {
        RuneManager.Instance.RuneMouseDown();
    }

    void OnMouseUp() {
        RuneManager.Instance.RuneMouseUp();
    }

    public void SetUp(RuneData data) {

        this.data = data;

        runeImage.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == this.data.id + "_image");
    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0) {
        if (useDotween) {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else {
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }
}
