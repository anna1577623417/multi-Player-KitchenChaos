using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        image.sprite = kitchenObjectSO.sprite;
        //用kitchenObjectSO中的信息来生成对应的sprite
        //而不需要用n多个预制体来记录不同的sprite
    }
}
