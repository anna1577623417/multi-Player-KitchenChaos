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
        Loader.targetScene = targetScene;//Ҫ��LoaderCallback��ʹ��
        SceneManager.LoadScene(Scene.LoadingScene.ToString());

        //using UnityEngine.SceneManagement;
        //ʵ�ʽ��г����л�����ͷ���
        //������س���
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
        //������س������ڵ�һ֡���øú�������ʼ����Ŀ�곡������Ŀ�곡������ȫ����ǰ������ʾ
    }

}
