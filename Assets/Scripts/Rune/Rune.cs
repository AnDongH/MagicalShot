using DG.Tweening;
using UnityEngine;

public class Rune : MonoBehaviour
{
    public PRS originPRS;
    public bool IsFreeze { get; private set; }
    public RuneData data;
    [SerializeField] SpriteRenderer runeImage;

    private void Start() {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    private void OnDestroy() {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    private void OnMouseEnter() {
        RuneManager.Instance.RuneMouseEnter(this);
    }

    private void OnMouseOver() {
        RuneManager.Instance.RuneMouseOver(this);
        Debug.Log("�鿡 ���콺 �ö�");
    }

    private void OnMouseExit() {
        RuneManager.Instance.RuneMouseExit(this);
        Debug.Log("�鿡�� ���콺 ����");
    }

    private void OnMouseDown() {
        RuneManager.Instance.RuneMouseDown();
        Debug.Log("�� Ŭ��");
    }

    private void OnMouseUp() {
        RuneManager.Instance.RuneMouseUp();
        Debug.Log("�鿡�� ���콺 ��");
    }

    public void SetUp(RuneData data) {

        this.data = data;

        runeImage.sprite = DataManager.Instance.Resource.runeImages.Find(x => x.name == this.data.id + "_Image");
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

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Marble"))
            print("�浹��!!");
    }

    private void OnTurnStarted(bool flag) {
        IsFreeze = false;
        runeImage.color = Color.white;
    }

    public void SetFreeze() {
        IsFreeze = true;
        runeImage.color = Color.gray;
    }
}
