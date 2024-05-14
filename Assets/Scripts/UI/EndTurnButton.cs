using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    Button button;
    

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void Start() {
        button.interactable = false;
        TurnManager.OnTurnChanged += SetUp;
    }

    private void OnDestroy() {
        TurnManager.OnTurnChanged -= SetUp;
    }

    private void Update() {

        // 빨리 턴 flag부분 개선해서 이 부분 바꾸자..
        if (PhotonNetwork.IsMasterClient) {
            if (TurnManager.Instance.IsHostTurn) {
                button.interactable = !InGameManager.Instance.MarbleMoving;
                button.gameObject.SetInteractable(button.interactable);
            }
        }
        else {
            if (!TurnManager.Instance.IsHostTurn) {
                button.interactable = !InGameManager.Instance.MarbleMoving;
                button.gameObject.SetInteractable(button.interactable);
            }
        }
    }

    public void SetUp(bool flag) {

        if (flag) {
            button.interactable = PhotonNetwork.IsMasterClient ? true : false;
        }
        else {
            button.interactable = PhotonNetwork.IsMasterClient ? false : true;
        }
        button.gameObject.SetInteractable(button.interactable);
    }
}
