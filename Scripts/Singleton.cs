using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T: new()
{
    //适合仅仅调用某个方法
    //不适用于访问某个实例的某个变量
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

