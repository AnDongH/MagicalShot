using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{

    public GameObject optionGRP;

    private void Start() {
        // 세팅 데이타에서 옵션값 불러와서 UI에 적용
        // 데이타에 맞춰서 옵션값 게임에 적용
    }

    public void EnterOptionGRP() {
        optionGRP.SetActive(true);
    }

    public void ExitOptionGRP() {
        optionGRP.SetActive(false);
    }
}
