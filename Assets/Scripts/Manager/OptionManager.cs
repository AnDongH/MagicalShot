using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{

    public GameObject optionGRP;

    private void Start() {
        // ���� ����Ÿ���� �ɼǰ� �ҷ��ͼ� UI�� ����
        // ����Ÿ�� ���缭 �ɼǰ� ���ӿ� ����
    }

    public void EnterOptionGRP() {
        optionGRP.SetActive(true);
    }

    public void ExitOptionGRP() {
        optionGRP.SetActive(false);
    }
}
