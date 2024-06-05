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
        //��spawnRecipeTimer = spawnRecipeTimerMax������һ��ʼ�ͱ���4��/�˵�����
        //��spawnRecipeTimer=0�������Ȼ���������һ��Ȼ��ÿ��������һ��
        if (spawnRecipeTimer < 0f ) {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.instance.IsGamePlaying()&&waitingRecipeSOList.Count < waitingRecipesMax) {

                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                
                //Debug.Log(waitingRecipeSO.recipeName);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
                //�����Ӧ��������ֻҪ��ȡ��Ӧ����������Ȼ�����ɶ�Ӧ�Ĳ˵�

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
                            //����ǰ��ΪABCD
                            //����ΪDCBA
                            //����ǰ�ߵ�A����һ�㣩�����ں�����Ҳ�ҵ�A���ڶ��㣩��������Ϊtrue��
                            //���һ��Ҳ��trueʱ��˫ѭ������������ingredientFound���������ӦUI
                        }
                    }
                    if (!ingredientFound ) {
                        //��һ���Ҳ�����Ϊfalse
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

    //  �ɴ˿ɼ�f (!IsServer) {return;}
    // DeliveryManagerҲ��Server��һ����
    // ����Client����ʳ��ʱ�ᱨ��
    //��������һ����ʹ��ClientҲ���Դ���ServerRpc
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
