using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameUtil : Singleton<GameUtil>
{
    //�����࣬�ⲿ�ű�ֻ��Ҫͨ���������ɵ���������ķ���
    //����Ҫ֪���������˭
    public bool InScreen(Vector3 position)
    {
        return Screen.safeArea.Contains(Camera.main.WorldToScreenPoint(position));
    }
}
