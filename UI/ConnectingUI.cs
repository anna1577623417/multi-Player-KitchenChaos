using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start() {
        KitchenGameMultiplayer.instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;

        Hide();
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    //切换场景会摧毁，但是由于是单例，订阅不会自动取消，因此需要手动取消订阅，否则订阅会出现重复
    private void OnDestroy() {
        KitchenGameMultiplayer.instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
    }
}
