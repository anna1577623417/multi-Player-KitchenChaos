using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    //Unity �ڳ����л�ʱ���Զ������������ٺ���Դ�ͷ�
    //��̬ί�еĶ������ڳ����л�ʱ���ᱻ�ݻٻ�ȡ������̬ί�������౾��������ģ����������ض�����ʵ��������ġ�
    //��ˣ����۳�������л�����̬ί�еĶ����߶���������ڣ�ֱ�������������ʽȡ�����ġ�
    private void Awake() {
        BaseCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
