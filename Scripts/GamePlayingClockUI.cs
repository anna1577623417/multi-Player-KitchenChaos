using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI gamePlayingTimerText;

    private void Update() {
        timerImage.fillAmount = KitchenGameManager.instance.GetGamePlayingTimerNormalize();
        gamePlayingTimerText.text = Mathf.Ceil(KitchenGameManager.instance.GetGamePlayingTimer()).ToString();
    }
}
