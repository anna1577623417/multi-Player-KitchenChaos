using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake() {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = KitchenGameMultiplayer.instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameLobby.instance.KickPlayer(playerData.playerId.ToString());
            KitchenGameMultiplayer.instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start() {
        KitchenGameMultiplayer.instance.OnPlayerDataNetworkListChanged += kitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void kitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }
    private void UpdatePlayer() {
        if (KitchenGameMultiplayer.instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            PlayerData playerData = KitchenGameMultiplayer.instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyGameObject.SetActive(CharacterSelectReady.instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();

            playerVisual.SetPlayerColor(KitchenGameMultiplayer.instance.GetPlayerColor(playerData.colorId));
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive (false);
    }
    private void OnDestroy() {
        KitchenGameMultiplayer.instance.OnPlayerDataNetworkListChanged -= kitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}
