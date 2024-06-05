using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button StartHostButton;
    [SerializeField] private Button StartClientButton;

    private void Start() {
        StartHostButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.instance.StartHost();
            Hide();
        });
        StartClientButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.instance.StartClient();
            Hide();
        });
    }
 
    private void Hide() {
        gameObject.SetActive(false);
    }
}


