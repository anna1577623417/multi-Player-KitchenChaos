using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Unity.Netcode;
public static class Loader {
    public enum Scene {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene,
    }
    public static Scene targetScene;
    public static AsyncOperation asyncLoad;
    public static void Load(Scene targetScene) {
        Loader.targetScene = targetScene;//要在LoaderCallback中使用
        SceneManager.LoadScene(Scene.LoadingScene.ToString());

        //using UnityEngine.SceneManagement;
        //实际进行场景切换的类和方法
        //进入加载场景
    }
    public static void LoadNetwork(Scene targetScene) {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(),LoadSceneMode.Single);
    }
    public static void LoadSAsync(Scene targetScene) {
        Loader.targetScene = targetScene;
        SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
}
    public static void LoaderCallback() {

        LoadingProgressBarUI.instance.asyncLoad = SceneManager.LoadSceneAsync(targetScene.ToString());
        //进入加载场景就在第一帧调用该函数，开始加载目标场景，但目标场景在完全加载前不会显示
    }

}
