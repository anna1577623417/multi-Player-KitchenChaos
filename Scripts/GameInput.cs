using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    public static GameInput instance { get; private set; } 
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteracAlternatetAction;
    public event EventHandler OnPauseAction;
    public event EventHandler OnBindingReBind;
    public event EventHandler<EventArgs_GameInput_Binding> UpdateRebingdingUI;

    private PlayerInputAction playerInputActions;

    public class EventArgs_GameInput_Binding {
        public Binding binding;
        public bool IsRebinded;
    }
    public enum Binding {
        Move_Up, 
        Move_Down,
        Move_Left, 
        Move_Right,
        Interact,
        InteractAlternate,
        Pause,
    }

    private void Awake() {
        EventAwake();
        instance = this;
    }
    private void EventAwake()
    {
        playerInputActions = new PlayerInputAction();//每一次都是重新创建
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        //只要按下Interact里的按钮（一个操作可以绑定多个键）就会触发关联的回调函数
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;

        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS)) {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        //尝试做一个重置默认键的按钮

        //Debug.Log(GetBindingText(Binding.Pause));
    }
    private void OnDestroy() {
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();//清理释放 Player Input Actions 对象的方法
    }


    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this,EventArgs.Empty);    
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteracAlternatetAction?.Invoke(this, EventArgs.Empty);
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)//监视输入并触发OnInteractAction事件
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
        //？运算符用于检测调用者是否为Null，为null则返回null，否则正常调用

        //if (OnInteractAction != null) {
        //    OnInteractAction(this, EventArgs.Empty);
        
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        //通过在插件设置前后作用对应的WSAD键，即可监视输入,并读取对应的值（vector类型的向量up,down,left,right）
        inputVector = inputVector.normalized;

        return inputVector;
    } 

    public string GetBindingText(Binding binding) {
        switch(binding) {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();

            case Binding.Pause:
                 return playerInputActions.Player.Pause.bindings[0].ToDisplayString();//来自插件GameInput
             case Binding.InteractAlternate:
                 return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
             case Binding.Interact:
                 return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
        }
    }
    public void RebindBinding(Binding binding,Action onActionRebound) {
        UpdateRebingdingUI?.Invoke(this, new EventArgs_GameInput_Binding {
            binding = binding,
            IsRebinded =false
        });
        playerInputActions.Player.Disable();
        InputAction inputAction;
        int bindingIndex;
        switch (binding) {
            default :
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1; 
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;

            case Binding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.InteractAlternate:
                inputAction = playerInputActions.Player.InteractAlternate;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex)
            .OnComplete(callback => {
                callback.Dispose();
                playerInputActions.Player.Enable();

                UpdateRebingdingUI?.Invoke(this, new EventArgs_GameInput_Binding {
                    binding = binding,
                    IsRebinded = true
                });

                //Debug.Log("2");
                onActionRebound();

                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
                //保存到本地
                OnBindingReBind?.Invoke(this,EventArgs.Empty);
            })
            .Start();
        //Debug.Log("1");
        //Debug.Log(callback.action.bindings[1].path);
        //Debug.Log(callback.action.bindings[1].overridePath);
    }
}
