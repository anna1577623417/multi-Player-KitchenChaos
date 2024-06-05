using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T: new()
{
    //�ʺϽ�������ĳ������
    //�������ڷ���ĳ��ʵ����ĳ������
    static T instance; 

    public static T Instance
    {
        get 
        { 
            if (instance == null)
            {
                instance = new T();
            }
            return instance; 
        }
    }
}

