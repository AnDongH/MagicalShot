using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SoundUnit {
    public string name;
    public AudioClip clip;
}

public class SoundManager : DontDestroySingleton<SoundManager> {

    [SerializeField] SoundUnit[] BackGroundSounds;
    [SerializeField] SoundUnit[] SFXSounds;

    public AudioSource backSource;
    public AudioSource sfxSource;

    private void Start() {
        PlayBackGroundSound("MainTheme");
    }

    public void PlayBackGroundSound(string name) {

        SoundUnit sound = Array.Find(BackGroundSounds, x => x.name == name);

        if (sound == null) return;

        if (backSource.clip == sound.clip) return;

        backSource.Stop();
        backSource.clip = sound.clip;
        backSource.loop = true;
        backSource.Play();
    }

    public void BackGroundSoundStop() {
        backSource.Stop();
    }

    public void PlaySFXSound(string name) {

        SoundUnit sound = Array.Find(SFXSounds, x => x.name == name);

        if (sound == null) return;

        sfxSource.PlayOneShot(sound.clip);
    }

}
