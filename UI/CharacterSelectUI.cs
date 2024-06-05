using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour  {
    [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button ReadyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake() {
        MainMenuButton.onClick.AddListener(() => {
            KitchenGameLobby.instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        ReadyButton.onClick.AddListener(() => {
            CharacterSelectReady.instance.SetPlayerReady();
        });
    }

    private void Start() {
        Lobby lobby = KitchenGameLobby.instance.GetLobby();

        lobbyNameText.text = "Lobby Name: "+lobby.Name;
        lobbyCodeText.text = "Lobby Code: "+lobby.LobbyCode;
    }
}
