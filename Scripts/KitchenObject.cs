using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;

    private ClearCounter clearCounter;

    protected virtual void Awake() {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO; 
    }
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent){
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
            //放置物品前物品已经在柜台上，要先清除这个柜台上的道具信息（把物体拿起来）
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (this.kitchenObjectParent.HasKitchenObject()) {
            Debug.LogError("IKitchenObjectParent already has a kitchenObject!");
        }
        kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());

        //transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform(); 
        //transform.localPosition = Vector3.zero;
    }


    public IKitchenObjectParent GetIKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void ClearKitchenObjectOnParent() {
        kitchenObjectParent.ClearKitchenObject();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }else {
            plateKitchenObject = null;
            return false;
        }
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        KitchenGameMultiplayer.instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject) {
        KitchenGameMultiplayer.instance.DestroyKitchenObject(kitchenObject);
    }
}
