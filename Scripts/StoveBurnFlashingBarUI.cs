using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private const string IS_FLASHING = "IsFlashing";
    [SerializeField] private StoveCounter stoveCounter;
    private Animator Animator;
    private void Start() {
        Animator  = GetComponent<Animator>();
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        float burnShowProgressAmount = 0.5f;
        bool show = stoveCounter.IsFried() && e.progressNormalized >= burnShowProgressAmount;

        if (show) {
            Animator.SetBool(IS_FLASHING, true);
        } else {
            Animator.SetBool(IS_FLASHING, false);
        }
    }
    
}
