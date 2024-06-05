using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO KitchenObjectSo;
    }


    [SerializeField] private List<KitchenObjectSO> vaildKitchenObjectSOList;
    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake() {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if(!vaildKitchenObjectSOList.Contains(kitchenObjectSO)) {
            //Not a vaild ingredient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            //Already has this type
            return false;
        }else {
            AddIngredientServerRpc(
                KitchenGameMultiplayer.instance.GetKitchenObjectSOIndex(kitchenObjectSO)
                );
            
            return true;
        }
        
    }
    [ServerRpc(RequireOwnership =false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex) {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex) {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
            KitchenObjectSo = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}
