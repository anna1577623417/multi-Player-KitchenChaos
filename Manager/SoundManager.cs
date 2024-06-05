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
        // "Player Preferences"�����ƫ�����ã�
        //1f �ǵ�ָ������ֵ������ʱ���ص�Ĭ��ֵ��������˵��
        //����������У������ PLAYER_PREFS_SOUND_EFFECTS_VOLUME ��Ӧ��ֵ������
        //������֮ǰû��ͨ�� PlayerPrefs.SetFloat �洢���ü���ֵ������ô GetFloat �����᷵��Ĭ��ֵ 1.0��
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

    //float volume=1f����������float��������volumeĬ��Ϊ1�������룬�򸲸�
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
    //������scroll bar������
    public void ChangeVolume() {
        volume += 0.1f;
        if (volume > 1f) {
            volume = 0f;
        }
        //ʹ�� PlayerPrefs.SetFloat ��������ֵ���ض��ļ�������������ζ��������Ϸ�д�����һ����ֵ��
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME,volume);
        PlayerPrefs.Save();
        //���ŵ��� PlayerPrefs.Save() ���������������ж� PlayerPrefs ���޸�д����̣�ȷ�����ݱ���ʱ���档�������Ա����ڳ��������˳���ر�ʱ��ʧδ���������
        //volume = volume % 1.1f;������1
    }
    public float GetVolume() { 
        return volume;
    }
}
