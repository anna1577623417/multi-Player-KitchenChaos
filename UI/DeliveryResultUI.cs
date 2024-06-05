using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POP_UP = "PopUp";
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite succeessSprite;
    [SerializeField] private Sprite failureSprite;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += Instance_OnRecipeFailed;
        Hide();//在Awake和Start中会有不同的效果，前者导致不能开启，后者则可以
    }

    private void Instance_OnRecipeFailed(object sender, System.EventArgs e) {
        Show();
        backgroundImage.color = failedColor;
        iconImage.sprite = failureSprite;
        messageText.text = "DELIVERY\nFAILED";
        //Debug.Log("failed");
        animator.SetTrigger(POP_UP);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        Show();
        backgroundImage.color = successColor;
        iconImage.sprite = succeessSprite;
        messageText.text = "DELIVERY\nSUCCESS";
        animator.SetTrigger(POP_UP);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
