using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        playButton.onClick.AddListener(() => {
           Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();//用于终止当前正在运行的应用程序。在许多编程语言和开发框架中都有类似的命令，用于关闭应用程序并结束其执行
        });

        Time.timeScale = 1.0f;//每次进游戏前保证不是暂停状况
    }
    
}
