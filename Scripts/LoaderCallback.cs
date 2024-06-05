using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    public static LoaderCallback instance { get; private set; } 
   private bool isFirstUpdate = true;
   
    private void Start() {
        instance = this;
    }
    private void Update() {
        if (isFirstUpdate) {
            isFirstUpdate = false;
            Loader.LoaderCallback();
            //��һ֡����ʼ���س���
            //���� SceneManager.LoadScene ��һ���첽������
            //�����ں�̨��ʼ����Ŀ�곡���������������л���Ŀ�곡����
            //�����ڼ��س���һ�㶼����Դռ�ú�С�ĳ�����
            //���Կ��������س�������Ҫ���أ�
            //��������������˲�������س���Ȼ��ʼ׼������,�߱������Ժ�������
        }
    }
}
