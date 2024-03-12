using System;
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
    List<Tuple<int, int>> resolutions = new List<Tuple<int, int>>();
    public int resolutionNum;


    private void Start() {
        // ���� ����Ÿ���� �ɼǰ� �ҷ��ͼ� UI�� ����
        // ����Ÿ�� ���缭 �ɼǰ� ���ӿ� ����

        InitUI();
    }

    private void InitUI() {

        resolutions.Add(Tuple.Create(1920, 1080));
        resolutions.Add(Tuple.Create(1600, 900));
        resolutions.Add(Tuple.Create(1200, 675));
        resDrop.options.Clear();

        int optionNum = 0;
        foreach (Tuple<int, int> item in resolutions) {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.Item1 + "x" + item.Item2 + " " + "hz";
            resDrop.options.Add(option);

            if (item.Item1 == Screen.width && item.Item2 == Screen.height)
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
        Screen.SetResolution(resolutions[resolutionNum].Item1, resolutions[resolutionNum].Item2, ScreenMode);
    }

    public void EnterOptionGRP() {
        optionGRP.SetActive(true);
    }

    public void ExitOptionGRP() {
        optionGRP.SetActive(false);
    }


}
