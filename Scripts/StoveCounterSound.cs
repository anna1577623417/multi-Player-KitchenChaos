using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter StoveCounter;

    
    private float warningSoundTimer;
    private bool playWarningSound;
    private AudioSource cookAudioSource;//≈Î‚ø“Ù¿÷
    private void Awake() {
        cookAudioSource = GetComponent<AudioSource>();
    }
    private void Start() {
        StoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        StoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        float burnShowProgressAmount = 0.5f;
        playWarningSound = StoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;
        if (playWarningSound) {

        } else {

        }
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {

        //Debug.Log(e.state);
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;
        if (playSound) {
            cookAudioSource.Play();
        } else {
            cookAudioSource.Pause();
        }
    }
    private void Update() {
        if (playWarningSound) {
            warningSoundTimer -= Time.deltaTime;
            if (warningSoundTimer < 0f) {
                float warningSoundGapTime = 0.2f;
                warningSoundTimer = warningSoundGapTime;
                SoundManager.instance.PlayWarningSound(StoveCounter.transform.position);
            }
        }

    }
}
