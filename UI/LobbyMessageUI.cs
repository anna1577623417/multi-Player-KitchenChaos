using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake() {
        closeButton.onClick.AddListener(Hide);
    }


    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }
    private void Start() {
        KitchenGameMultiplayer.instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.instance.OnCreatedLobbyStarted += KitchenGameLobby_OnCreatedLobbyStarted;
        KitchenGameLobby.instance.OnCreatedLobbyFailed += KitchenGameLobby_OnCreatedLobbyFailed;
        KitchenGameLobby.instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;

        Hide();
    }
    private void KitchenGameLobby_OnCreatedLobbyFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to create Lobby !");
    }

    private void KitchenGameLobby_OnCreatedLobbyStarted(object sender, System.EventArgs e) {
        ShowMessage("Creating Lobby. . .");
    }
    private void KitchenGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Could not find a Lobby to Quick Join !");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, System.EventArgs e) {
        ShowMessage("Joining Lobby . . .");
    }

    private void KitchenGameLobby_OnJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to join Lobby !");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to connect !");
        } else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.instance.OnCreatedLobbyStarted -= KitchenGameLobby_OnCreatedLobbyStarted;
        KitchenGameLobby.instance.OnCreatedLobbyFailed -= KitchenGameLobby_OnCreatedLobbyFailed;
        KitchenGameLobby.instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
    }
}
