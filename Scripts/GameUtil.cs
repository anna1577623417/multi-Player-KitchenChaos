using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameUtil : Singleton<GameUtil>
{
    //工具类，外部脚本只需要通过类名即可调用这里面的方法
    //不需要知道这个类是谁
    public bool InScreen(Vector3 position)
    {
        return Screen.safeArea.Contains(Camera.main.WorldToScreenPoint(position));
    }
}
