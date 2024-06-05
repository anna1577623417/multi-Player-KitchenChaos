using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Awake() {
        instance = this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME,1f);
        // "Player Preferences"（玩家偏好设置）
        //1f 是当指定键的值不存在时返回的默认值。具体来说，
        //在这个例子中，如果键 PLAYER_PREFS_SOUND_EFFECTS_VOLUME 对应的值不存在
        //（即在之前没有通过 PlayerPrefs.SetFloat 存储过该键的值），那么 GetFloat 方法会返回默认值 1.0。
    }

    private float volume = 1.0f;

    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnanyCut;
        //Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlaceHere += BaseCounter_OnAnyObjectPlaceHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e) {
        TrashCounter baseCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash , baseCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlaceHere(object sender, System.EventArgs e) {
       BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop,baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, System.EventArgs e) {
        //PlaySound(audioClipRefsSO.objectPickup,Player.Instance.transform.position);
    }

    private void CuttingCounter_OnanyCut(object sender, System.EventArgs e) {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        DeliveryManager deliveryCounter = DeliveryManager.Instance;
        PlaySound(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        DeliveryManager deliveryCounter = DeliveryManager.Instance;
        PlaySound(audioClipRefsSO.deliverySuccess, deliveryCounter.transform.position);
    }

    //float volume=1f，若不传入float变量，则volume默认为1，若传入，则覆盖
    private void PlaySound(AudioClip[] audioClipArray,Vector3 position,float volume=1f) {
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)],position,volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    public void PlayFootstepSound(Vector3 position,float volumeMultiplier=1f) {
        PlaySound(audioClipRefsSO.footstep, position, volumeMultiplier*volume);
    }
    public void PlayCountdownSound() {
        PlaySound(audioClipRefsSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position) {
        PlaySound(audioClipRefsSO.warning, position);
    }
    //尝试用scroll bar调音量
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1f) {
            volume = 0f;
        }
        //使用 PlayerPrefs.SetFloat 将浮点数值与特定的键关联起来，意味着你在游戏中创建了一个键值对
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME,volume);
        PlayerPrefs.Save();
        //接着调用 PlayerPrefs.Save() 方法会立即将所有对 PlayerPrefs 的修改写入磁盘，确保数据被及时保存。这样可以避免在程序意外退出或关闭时丢失未保存的数据
        //volume = volume % 1.1f;不超过1
    }
    public float GetVolume() { 
        return volume;
    }
}
