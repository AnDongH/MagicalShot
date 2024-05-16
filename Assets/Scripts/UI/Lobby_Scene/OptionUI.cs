using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionUI : UI_PopUp
{
    enum Buttons {
        ExitBtn,
        OkBtn
    }

    enum Dropdowns {
        Resolution_Dropdown
    }

    enum Toggles {
        FullScreenToggle
    }

    enum Sliders {
        BackSoundSlider,
        SFXSoundSlider
    }

    protected override void Init() {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Dropdown>(typeof(Dropdowns));
        Bind<Toggle>(typeof(Toggles));
        Bind<Slider>(typeof(Sliders));

        OptionManager.Instance.InitUI(GetDropdown((int)Dropdowns.Resolution_Dropdown), GetToggle((int)Toggles.FullScreenToggle));


        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(OnExitBtnClicked);
        GetButton((int)Buttons.OkBtn).gameObject.BindEvent(OnOkBtnClicked);

        GetDropdown((int)Dropdowns.Resolution_Dropdown).onValueChanged.AddListener(OnResolution_DropdownChanged);
        GetToggle((int)Toggles.FullScreenToggle).onValueChanged.AddListener(OnFullScreenToggleChanged);

        GetSlider((int)Sliders.BackSoundSlider).onValueChanged.AddListener(OnBackSoundSliderChanged);
        GetSlider((int)Sliders.SFXSoundSlider).onValueChanged.AddListener(OnSFXSoundSliderChanged);

        GetSlider((int)Sliders.BackSoundSlider).value = SoundManager.Instance.backSource.volume;
        GetSlider((int)Sliders.SFXSoundSlider).value = SoundManager.Instance.sfxSource.volume;
    }

    private void Start() {
        Init();
    }

    private void OnExitBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        ClosePopUpUI();
    }

    private void OnOkBtnClicked(PointerEventData data) {
        SoundManager.Instance.PlaySFXSound("ButtonClick");
        OptionManager.Instance.OkBtnClick();
    }

    private void OnResolution_DropdownChanged(int x) {
        OptionManager.Instance.DropboxOptionChange(x);
    }

    private void OnFullScreenToggleChanged(bool flag) {
        OptionManager.Instance.FullScreenBtn(flag);
    }

    private void OnBackSoundSliderChanged(float val) {
        SoundManager.Instance.backSource.volume = val;
    }

    private void OnSFXSoundSliderChanged(float val) {
        SoundManager.Instance.sfxSource.volume = val;
    }

}
