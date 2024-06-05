using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUP : MonoBehaviour {
    private void Awake() {
        if (NetworkManager.Singleton != null) {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (KitchenGameMultiplayer.instance != null) {
            Destroy(KitchenGameMultiplayer.instance.gameObject);
        }

        if (KitchenGameLobby.instance != null) {
            Destroy(KitchenGameLobby.instance.gameObject);
        }
    }
}
