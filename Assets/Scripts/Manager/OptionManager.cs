using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : NormalSingleton<OptionManager>
{
    
    FullScreenMode ScreenMode;
    List<Tuple<int, int>> resolutions = new List<Tuple<int, int>>();
    private int resolutionNum;

    public void InitUI(Dropdown dropdown, Toggle toggle) {

        resolutions.Add(Tuple.Create(1920, 1080));
        resolutions.Add(Tuple.Create(1600, 900));
        resolutions.Add(Tuple.Create(1200, 675));
        dropdown.options.Clear();

        int optionNum = 0;
        foreach (Tuple<int, int> item in resolutions) {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.Item1 + "x" + item.Item2 + " " + "hz";
            dropdown.options.Add(option);

            if (item.Item1 == Screen.width && item.Item2 == Screen.height)
                dropdown.value = optionNum;
            optionNum++;
        }
        dropdown.RefreshShownValue();

        toggle.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
        ScreenMode = Screen.fullScreenMode;
    }

    public void DropboxOptionChange(int x) {
        resolutionNum = x;
    }

    public void FullScreenBtn(bool isFull) {
        ScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OkBtnClick() {
        Screen.SetResolution(resolutions[resolutionNum].Item1, resolutions[resolutionNum].Item2, ScreenMode);
    }

}
