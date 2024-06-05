using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingProgressBarUI : MonoBehaviour {
    public static LoadingProgressBarUI instance { get; private set; }
    [SerializeField] private Image progressBar; // ������
    [SerializeField] private TextMeshProUGUI progressText; // ��ʾ���ؽ��ȵ��ı�
    public AsyncOperation asyncLoad;

    private void Start() {
        instance = this;
    }
    private void Update() {
        //����Ҫ��߼�һ��While�����ױ����ѭ��
        if (asyncLoad!=null&& !asyncLoad.isDone) {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                //����ֵ asyncLoad.progress �ķ�Χ�Ǵ� 0 �� 0.9
                //������ؽ��ȣ���Χ��0��1
                //Debug.Log(progress*100f+"%"+ asyncLoad.progress);
                progressBar.fillAmount = progress; // ���ý�������ֵ
                progressText.text = "Loading: " + (progress * 100f).ToString("F0") + "%..."; // ������ʾ���ؽ��ȵ��ı�
        }
    }
        
}
 
