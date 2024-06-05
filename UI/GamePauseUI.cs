using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Start() {
        KitchenGameManager.instance.OnLocalGamePaused += KitchenGameManger_OnLocalGamePaused;
        KitchenGameManager.instance.OnLocalGameUnPaused += KitchenGameManger_OnLocalGameUnPaused;

        Hide();

    }
    private void Awake() {
        resumeButton.onClick.AddListener(() => { 
            KitchenGameManager.instance.TogglePauseGame();//暂停时按下按钮继续游戏，取消暂停
        });
        mainMenuButton.onClick.AddListener(() => {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() => {
            Hide();
            OptionsUI.instance.ShowOnCloseButton(Show);
        });
    }

    private void KitchenGameManger_OnLocalGamePaused(object sender, System.EventArgs e) {
        Show();

    }

    private void KitchenGameManger_OnLocalGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
