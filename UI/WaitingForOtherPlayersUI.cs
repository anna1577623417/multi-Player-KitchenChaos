using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start() {
        KitchenGameManager.instance.OnLocalPlayerReadyChange += KitchenGameManager_OnLocalPlayerReadyChange;
        KitchenGameManager.instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.instance.IsCountdownToStartActive()) {
            Hide();
        }
      
    }
    private void KitchenGameManager_OnLocalPlayerReadyChange(object sender, System.EventArgs e) {
        if (KitchenGameManager.instance.IsLocalPlayerReady()/*&&!KitchenGameManager.instance.IsSinglePlayer()*/) {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
