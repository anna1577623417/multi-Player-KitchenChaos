using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MusicManager : MonoBehaviour
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    public static MusicManager instance { get; private set; }
    private AudioSource audioSource;
    private float volume=0.3f;

    private void Awake() {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME,0.3f);
        audioSource.volume = volume;
    }
    //尝试制作一个禁音按钮
    //尝试用scroll bar调音量
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1f) {
            volume = 0f;
        }
        audioSource.volume = volume;
        //volume = volume % 1.1f;不超过1

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume() {
        return volume;
    }
}
