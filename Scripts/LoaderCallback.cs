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
            //第一帧即开始加载场景
            //由于 SceneManager.LoadScene 是一个异步操作，
            //它会在后台开始加载目标场景，但不会立即切换到目标场景。
            //又由于加载场景一般都是资源占用很小的场景，
            //所以看起来加载场景不需要加载，
            //这样看起来就是瞬间进入加载场景然后开始准备加载,具备流畅性和连贯性
        }
    }
}
