using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliveredText;
    [SerializeField] private Button playerAgainButton;
    private void Awake() {
        playerAgainButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
            Hide();
        });
        
    }

    private void Start() {
        KitchenGameManager.instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.instance.IsGameOver()) {
            Show();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        } else {
            Hide();
        }
    }

    private void Update() {
        recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        //ToString(F2); "#.##"转字符串同时保留小数
        //Mathf.Ceil()
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
