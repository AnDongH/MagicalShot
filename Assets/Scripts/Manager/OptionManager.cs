using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{

    [Header("�ɼ� UI")]
    public GameObject optionGRP;
    public Dropdown resDrop;
    public Toggle fullScreenBtn;
    
    FullScreenMode ScreenMode;
    List<Resolution> resolutions = new List<Resolution>();
    public int resolutionNum;


    private void Start() {
        // ���� ����Ÿ���� �ɼǰ� �ҷ��ͼ� UI�� ����
        // ����Ÿ�� ���缭 �ɼǰ� ���ӿ� ����

        InitUI();
    }

    private void InitUI() {
        for (int i = 0; i < Screen.resolutions.Length; i++) {
            if ((int)Screen.resolutions[i].refreshRateRatio.value == 60) resolutions.Add(Screen.resolutions[i]);
        }
        resolutions.Reverse();
        resDrop.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions) {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " + item.refreshRateRatio + "hz";
            resDrop.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resDrop.value = optionNum;
            optionNum++;
        }
        resDrop.RefreshShownValue();

        fullScreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropboxOptionChange(int x) {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull) {
        ScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnBtnClick() {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, ScreenMode);
    }

    public void EnterOptionGRP() {
        optionGRP.SetActive(true);
    }

    public void ExitOptionGRP() {
        optionGRP.SetActive(false);
    }


}
