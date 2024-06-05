using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameInput;

public class OptionsUI : MonoBehaviour {

    public static OptionsUI instance { get; private set; }

    public event EventHandler OnRebinded;

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundEffectText;
    [SerializeField] private TextMeshProUGUI musicText;

    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button MoveLeftButton;
    [SerializeField] private Button MoveRightButton;
    [SerializeField] private Button InteractButton;
    [SerializeField] private Button InteractAlternateButton;
    [SerializeField] private Button PauseButton;

    [SerializeField] private TextMeshProUGUI MoveUpText;
    [SerializeField] private TextMeshProUGUI MoveDownText;
    [SerializeField] private TextMeshProUGUI MoveLeftText;
    [SerializeField] private TextMeshProUGUI MoveRightText;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private TextMeshProUGUI InteractAlternateText;
    [SerializeField] private TextMeshProUGUI PauseText;
    [SerializeField] private TextMeshProUGUI OriginKeyText;
    [SerializeField] private TextMeshProUGUI NewKeyText;
    private string previousText;
   [SerializeField] private Transform pressToRebindKeyTransform;



    private Action onCloseButtonAction;

    private void Start() {
        KitchenGameManager.instance.OnLocalGamePaused += KitchenGameManager_OnGamePaused;
        GameInput.instance.UpdateRebingdingUI += GameInput_UpdateRebingdingUI;

        Hide();
        HidePressToRebindKeyUI();
        UpdateVisual();
    }

    private void GameInput_UpdateRebingdingUI(object sender, EventArgs_GameInput_Binding e) {
        if (e.IsRebinded) {
            NewKeyText.text = "<size=80>" + GameInput.instance.GetBindingText(e.binding).ToUpper() + "</size>";
            
        } else {
            NewKeyText.text = "";
            previousText = GameInput.instance.GetBindingText(e.binding).ToUpper();
            OriginKeyText.text = "<size=80>" +previousText+"</size>";
        }
    }
  
    private void Awake() {
        instance = this;
        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.instance.ChangeVolume();
            UpdateVisual();
        });
        musicButton.onClick.AddListener(() => {
            MusicManager.instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => {
            Hide();
            onCloseButtonAction();//这一层UI的开启伴随着上一层UI的关闭(PauseUI(关)=>OptionsUI(开))
        });

        MoveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Up); });
        MoveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Down); });
        MoveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Left); });
        MoveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Move_Right); });
        InteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact); });
        InteractAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlternate); });
        PauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause); });

    }

    private void RebindBinding(GameInput.Binding binding) {
        Hide();
        ShowPressToRebindKeyUI();//多一个显示界面的操作
        GameInput.instance.RebindBinding(binding, () => {
            //Debug.Log("3");
            UpdateVisual();
            OnRebinded?.Invoke(this, EventArgs.Empty);
            //TimeToDelay = true;
            //HidePressToRebindKeyUI();
            //Debug.Log("4");
        });

        //使用 Action 作为参数类型，并通过Lambda表达式传入，可以以匿名函数的形式简单地传入多个函数作为参数进入函数。这种方式非常灵活，
        //可以在调用函数时直接定义并传入所需的逻辑操作，而无需单独定义函数名称。

        //当参数是 `Action` 类型时，传入 `() => { HidePressToRebindKey();`，`HidePressToRebindKey()` 不会在原函数执行完毕时回调。 

        //`Action` 是一个委托（delegate）类型，它表示一个不带参数也不返回值的方法。
        //当把 `() => {    HidePressToRebindKey();` 这样的Lambda表达式作为 `Action` 类型的参数传入函数时，
        //这个Lambda表达式将被视为一个匿名方法，可以在函数内部调用。

        //如果在函数内部执行了这个 `Action` 参数，那么其中的代码块会在函数执行的过程中被调用，
        //而不是在函数执行完毕后回调。因此，在这种情况下，`
        //HidePressToRebindKey()` 会在函数执行过程中被调用，而不是在函数执行完毕后回调。

        //会执行一个匿名的函数，这个函数调用了HidePressToRebindKey()函数
        //换句话说，当把一个Lambda表达式作为参数传递给另一个函数时，
        //这个Lambda表达式实际上是作为一个功能回调（callback）传递进去的。
        //在接收Lambda表达式的函数内部执行完自己的逻辑后，会执行这个Lambda表达式所代表的操作。
        //这种机制常用于事件处理、异步回调等场景，可以实现灵活的控制流程和逻辑处理。
    }

    private void KitchenGameManager_OnGamePaused(object sender, System.EventArgs e) {
        Hide();
    }

    // Mathf.Round返回的是一个int类型，但是字符串拼接+将其隐式转换为string
    private void UpdateVisual() {
        soundEffectText.text = "Sound Effect:" + Mathf.Round(SoundManager.instance.GetVolume() * 10f);
        musicText.text = " Music:" + Mathf.Round(MusicManager.instance.GetVolume() * 10f);

        MoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
        MoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
        MoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
        MoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
        InteractText.text = GameInput.instance.GetBindingText(GameInput.Binding.Interact);
        InteractAlternateText.text = GameInput.instance.GetBindingText(GameInput.Binding.InteractAlternate);
        PauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
    }
    //Action 是一个委托类型，表示无参数和无返回值的方法
    //通过这种方式，可以在调用 Show 方法时灵活地传入一个闭包函数（closure），
    //从而实现在特定事件发生时执行相应的操作。这种设计模式常用于实现回调功能，
    //例如点击按钮后执行特定的操作或响应用户关闭界面的操作等
    //下一层UI的开启伴随着上一层UI的关闭
    public void ShowOnCloseButton(Action onCloseButtonAction) {
        this.onCloseButtonAction = onCloseButtonAction;

        gameObject.SetActive(true);

        musicButton.Select();
    }
    public void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
    private void ShowPressToRebindKeyUI() {
        pressToRebindKeyTransform.gameObject.SetActive(true);
    }
    public void HidePressToRebindKeyUI() {
        //Debug.Log("5");
        pressToRebindKeyTransform.gameObject.SetActive(false);
    }
    
    //协程所在的 MonoBehaivour 对象被禁用或销毁： 如果调用协程的对象被禁用或销毁，协程也会停止执行。确保调用协程的对象是处于激活状态，并且未被销毁
    
}


