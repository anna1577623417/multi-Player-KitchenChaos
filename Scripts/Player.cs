using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent {
    public static Player LocalInstance {  get; private set; }
    public static event EventHandler OnAnyPlayerSpawned;

    public static void ResetStaticData() {
        OnAnyPlayerSpawned = null;
    }
    public event EventHandler OnPickedSomething;

    [SerializeField] private float rotationOffset;
    [SerializeField] private float MoveSpeed;//SerializeField使得该变量可以在unity监视器中编辑，即使这个变量是私有的
    [SerializeField] private float RotationSpeed;
    [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform KitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private PlayerVisual playerVisual;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    //委托所携带的事件参数类型为OnSelectedCounterChangedEventArgs

    //在 C# 中，通常在定义事件时，会使用 EventArgs 类或其派生类作为事件参数的类型。
    //EventArgs 类本身并不包含任何特定的事件数据，它只是一个空的基类，用于表示事件参数。
    //开发人员可以通过继承 EventArgs 类来定义自己的事件参数类型，并添加需要的字段或属性来携带事件相关的数据
    public class OnSelectedCounterChangedEventArgs : EventArgs//将这个类作为委托的类型
    {
        public BaseCounter selectedCounter;//让其他脚本知道SelectedCounter
    }


    //private static Player instance;
    //public static Player Instance
    //{
    //    get { return instance; }

    //    private set { instance = value; }
    //}

    private void Awake() {

        //if (instance != null && instance != this) {
        //    Destroy(gameObject);
        //} else {
        //    instance = this;
        //}
    }
    void Start() {

        GameInput.instance.OnInteractAction += GameInput_OnInterAction;
        GameInput.instance.OnInteracAlternatetAction += GameInput_OnInterAlternateAction;

        PlayerData playerData = KitchenGameMultiplayer.instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(KitchenGameMultiplayer.instance.GetPlayerColor(playerData.colorId));
    }

    //networkcode中不用awake或者start来创建单例，用 OnNetworkSpawn
    //"NetworkDespawn"通常是指在网络游戏开发中，
    //当一个游戏对象从网络中被销毁或移除时所触发的事件。
    //这个事件可能会涉及到清理网络同步数据、发送相关消息给其他玩家等操作。

    //Ondestroy可以用OnNetworkDespawn替换

    public override void OnNetworkSpawn() {
        if(IsOwner) {
            LocalInstance = this;
        }
        transform.position = spawnPositionList[KitchenGameMultiplayer.instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke(this,EventArgs.Empty);
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientConnectedCallback;
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        if(clientId== OwnerClientId&&HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    public bool IsWakling() {
        return isWalking;
    }
    private void Update() {
        if (!IsOwner) {
            return;
        }
        HandleMovement();
        HandleInteractions();
    }
    private void GameInput_OnInterAlternateAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }
    private void GameInput_OnInterAction(object sender, System.EventArgs e) {
        if (!KitchenGameManager.instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }
    private void HandleInteractions() {
        Vector2 inputVector = GameInput.instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter basecounter)) {
                //Has ClearCounter
                if (basecounter != selectedCounter) {
                    SetSelectedCounter(basecounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    //private void HandleMovementServerAuth() {
    //    Vector2 inputVector = GameInput.instance.GetMovementVectorNormalized();
    //    HandleMovementServerRpc(inputVector);
    //    //这行代码调用了名为 HandleMovementServerRpc 的方法，并将之前获取到的移动向量作为参数传递给这个方法。
    //    //根据命名中的 "Rpc" 缩写，这可能是一个用于在网络上进行远程过程调用（Remote Procedure Call，RPC）的方法。
    //    //通过调用这个方法并传递移动向量，可能会触发服务器端的相关操作，比如更新角色的位置或执行移动逻辑。

    //}

    //PVP选服务器端来处理角色
    ////这意味着即使客户端没有拥有权（ownership）
    //[ServerRpc(RequireOwnership = false)]
    //private void HandleMovementServerRpc(Vector2 inputVector) {
    //    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    //    float moveDistance = MoveSpeed * Time.deltaTime;
    //    float playerRadius = 0.7f;
    //    float playerHeight = 2f;

    //    bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

    //    if (!canMove) {

    //        Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

    //        canMove = (moveDir.x < -0.5f || moveDir.x > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);
    //        if (canMove) {

    //            moveDir = moveDirX;
    //        } else {
    //            Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
    //            canMove = (moveDir.z < -0.5f || moveDir.z > +0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
    //            if (canMove) {
    //                moveDir = moveDirZ;
    //            } else {
    //            }
    //        }
    //    }
    //    if (canMove) {
    //        this.transform.position += moveDir * moveDistance;
    //    }

    //    this.transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * RotationSpeed);

    //    isWalking = (moveDir != Vector3.zero);
    //}
    //通过调用 HandleMovementServerRpc(inputVector) 方法，
    //将这个移动向量作为参数传递给服务器端的 RPC 方法，
    //从而触发服务器端处理玩家移动操作的逻辑。
    private void HandleMovement() {
        Vector2 inputVector = GameInput.instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = MoveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        //float playerHeight = 2f;

        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);
        //光线长度等于玩家与碰撞体的距离
        //如果发出的射线被碰撞体阻挡，则返回true
        //线光线由于很薄，导致还是可能导致穿模
        //所以采用其他形状的射线，例如capsule或box

        if (!canMove) {
            //Cannot move towards moveDir

            //Attempt only X movement

            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            //moveDir.x!=0
            canMove = (moveDir.x < -rotationOffset || moveDir.x > +rotationOffset) && 
                !Physics.BoxCast(transform.position,  Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);
            if (canMove) {
                // Can move only on the X
                moveDir = moveDirX;
            } else {
                // Cannot move only on the x

                // Attempt only Z movement 
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                //moveDir.z!=0
                canMove = (moveDir.z < -rotationOffset || moveDir.z > +rotationOffset) && 
                    !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);
                //Physics.CapsuleCast(transform.position,transform.position+Vector3.up*playerHeight,playerRadius,moveDir,moveDistance,collisionsLayerMask);
                if (canMove) {
                    // Can move only on the z
                    moveDir = moveDirZ;
                } else {
                    //Can not move in any direction
                }
            }
        }

        if (canMove) {
            this.transform.position += moveDir * moveDistance;
        }


        this.transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * RotationSpeed);

        isWalking = (moveDir != Vector3.zero);
    }


    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;//【1】把射线返回的selectedCounter给本Player
        //这里问号的作用就是检查委托是否有订阅者，没有则不执行后续的代码，使得代码更简洁
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        //【2】将Player里的selectedCounter传给订阅者
        //Invoke函数将订阅者和委托联动，第一个参数是参数的发送者，即Player实例，并指定其他参数，这里是一个类实例
        {
            selectedCounter = selectedCounter//前者是OnSelectedCounterChangedEventArgs类中的，ClearCounter参数
        });
    }
    public Transform GetKitchenObjectFollowTransform() {
        return KitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject() {
        return this.kitchenObject;
    }
    public void ClearKitchenObject() {
        this.kitchenObject = null;
    }
    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
    public NetworkObject GetNetworkObject() {
        return NetworkObject;
        //NetworkObject 来自Unity.Netcode;
    }
}
