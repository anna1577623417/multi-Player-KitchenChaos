using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }
    private void Start() {
        DeliveryManager.Instance.OnRecipeSpwaned += DeliverManager_OnRecipeSpwaned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliverManager_OnRecipeCompleted;
    }

    private void DeliverManager_OnRecipeSpwaned(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void DeliverManager_OnRecipeCompleted(object sender, EventArgs e) {
        UpdateVisual();
    }
    private void UpdateVisual() {
        foreach(Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);//把第一个样版删掉
        }

        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList()){
            Transform recipeTransform = Instantiate(recipeTemplate,container);
            recipeTransform.gameObject.SetActive(true);
            //recipeTransform.Find("RecipeNameText");

            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
