using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        image.sprite = kitchenObjectSO.sprite;
        //��kitchenObjectSO�е���Ϣ�����ɶ�Ӧ��sprite
        //������Ҫ��n���Ԥ��������¼��ͬ��sprite
    }
}
