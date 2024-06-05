using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float footstepTimeMax=0.1f;

    private void Awake() {
        player = gameObject.GetComponent<Player>();
    }

    private void Update() {

        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f) {
            footstepTimer = footstepTimeMax;

            //使得每次播放间隔为0.1秒
            //否则会出现重叠音效的情况

            if (player.IsWakling()) {
                float volume = 1f;
                SoundManager.instance.PlayFootstepSound(player.transform.position, volume);
            }
        }
    }
}
