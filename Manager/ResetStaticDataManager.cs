using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    //Unity 在场景切换时会自动处理对象的销毁和资源释放
    //静态委托的订阅者在场景切换时不会被摧毁或取消。静态委托是与类本身相关联的，而不是与特定对象实例相关联的。
    //因此，无论场景如何切换，静态委托的订阅者都会持续存在，直到程序结束或显式取消订阅。
    private void Awake() {
        BaseCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
        Player.ResetStaticData();
    }
}
