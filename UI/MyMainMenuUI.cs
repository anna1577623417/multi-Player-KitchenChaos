using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyMainMenuUI : MonoBehaviour {
    public static MyMainMenuUI instance { get; private set; }
    [SerializeField] private Button MultiplayButton;
    [SerializeField] private Button SingleplayButton;
    [SerializeField] private Button quitButton;


    private void Awake() {
        MultiplayButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.LoadSAsync(Loader.Scene.LobbyScene);
        });

        SingleplayButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.LoadSAsync(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        Time.timeScale = 1.0f;
    }

}
