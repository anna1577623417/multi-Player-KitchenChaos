using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingProgressBarUI : MonoBehaviour {
    public static LoadingProgressBarUI instance { get; private set; }
    [SerializeField] private Image progressBar; // 进度条
    [SerializeField] private TextMeshProUGUI progressText; // 显示加载进度的文本
    public AsyncOperation asyncLoad;

    private void Start() {
        instance = this;
    }
    private void Update() {
        //不需要里边加一个While很容易变成死循环
        if (asyncLoad!=null&& !asyncLoad.isDone) {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                //进度值 asyncLoad.progress 的范围是从 0 到 0.9
                //计算加载进度，范围从0到1
                //Debug.Log(progress*100f+"%"+ asyncLoad.progress);
                progressBar.fillAmount = progress; // 设置进度条的值
                progressText.text = "Loading: " + (progress * 100f).ToString("F0") + "%..."; // 更新显示加载进度的文本
        }
    }
        
}
 
