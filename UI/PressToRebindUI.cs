using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressToRebindUI : MonoBehaviour
{
    private float DelayTimer;
    private float DelayTimerMax = 0.2f;

    void Start()
    {
        OptionsUI.instance.OnRebinded += OptionsUI_OnRebinded;
    }
    private void Update() {
        //Debug.Log("DelayTimer:"+ DelayTimer);
    }
    private void OptionsUI_OnRebinded(object sender, System.EventArgs e) {
        StartCoroutine(DelayedFunction());
    }
    IEnumerator DelayedFunction() {
        DelayTimer = 0f;
        while (DelayTimer < DelayTimerMax) {
            DelayTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        DelayTimer = 0f;
        OptionsUI.instance.HidePressToRebindKeyUI();
        //Debug.Log("Hide");
        OptionsUI.instance.Show();
        // 在这里添加需要延迟执行的逻辑
    }
}
