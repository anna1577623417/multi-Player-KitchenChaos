using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpwaned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax=4f;
    private int waitingRecipesMax = 4;
    private int successfulRecipesAmount;

    private void Awake() {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();

    }

    private void Update() {
        if (!IsServer) {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        //若spawnRecipeTimer = spawnRecipeTimerMax，则在一开始就保持4秒/此的生成
        //若spawnRecipeTimer=0，则首先会立即生成一个然后每四秒生成一次
        if (spawnRecipeTimer < 0f ) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.instance.IsGamePlaying()&&waitingRecipeSOList.Count < waitingRecipesMax) {

                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                
                //Debug.Log(waitingRecipeSO.recipeName);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
                //传入对应的索引，只要获取对应的索引，自然会生成对应的菜单

            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex) {

        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];
        waitingRecipeSOList.Add(waitingRecipeSO);

        OnRecipeSpwaned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0;i< waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectSOList.Count==plateKitchenObject.GetKitchenObjectSOList().Count) {
                // Has the same number of in gredients
                bool plateContentsMatchesRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList) {
                    // Cycling through all ingredients in the recipe

                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        // Cycling through all ingredients in the Plate
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            // Ingredient matches!
                            ingredientFound = true;
                            break;
                            //假设前者为ABCD
                            //后者为DCBA
                            //对于前者的A（第一层），能在后者中也找到A（第二层），就先设为true，
                            //最后一个也会true时，双循环结束，根据ingredientFound情况反馈对应UI
                        }
                    }
                    if (!ingredientFound ) {
                        //有一个找不到即为false
                        // this Recipe ingredient was not found on the Plate
                        plateContentsMatchesRecipe = false;
                    }
                }
                if(plateContentsMatchesRecipe) {
                    // Player delivered the correct recipe!
                   //Debug.Log("Player delivered the correct recipe!");

                    DeliverCorrectRecipeServerRpc(i);
                    return;
                } 
            }
        }

        DeliverIncorrectRecipeServerRpc();
        //Debug.Log("Player delivered the wrong recipe!");
        //no matchees found
        //Player did not deliever the correct recipe!
    }

    //  由此可见f (!IsServer) {return;}
    // DeliveryManager也是Server的一部分
    // 所以Client传送食物时会报错
    //加上了这一条，使得Client也可以触发ServerRpc
    [ServerRpc(RequireOwnership =false)]
    private void DeliverIncorrectRecipeServerRpc() {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc() {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }


    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOIndex) {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOIndex);
    }

    [ClientRpc] 
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOIndex) {
        successfulRecipesAmount++;

        waitingRecipeSOList.RemoveAt(waitingRecipeSOIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }
}
