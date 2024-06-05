using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playerAgainButton;

    private void Awake() {
        playerAgainButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenuScene);
            //Hide();
        });
    }

    private void Start() {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        
        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if(clientId== NetworkManager.ServerClientId) {
            //Server is shutting down
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if(NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }
}
