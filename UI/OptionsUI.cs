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
            onCloseButtonAction();//��һ��UI�Ŀ�����������һ��UI�Ĺر�(PauseUI(��)=>OptionsUI(��))
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
        ShowPressToRebindKeyUI();//��һ����ʾ����Ĳ���
        GameInput.instance.RebindBinding(binding, () => {
            //Debug.Log("3");
            UpdateVisual();
            OnRebinded?.Invoke(this, EventArgs.Empty);
            //TimeToDelay = true;
            //HidePressToRebindKeyUI();
            //Debug.Log("4");
        });

        //ʹ�� Action ��Ϊ�������ͣ���ͨ��Lambda���ʽ���룬������������������ʽ�򵥵ش�����������Ϊ�������뺯�������ַ�ʽ�ǳ���
        //�����ڵ��ú���ʱֱ�Ӷ��岢����������߼������������赥�����庯�����ơ�

        //�������� `Action` ����ʱ������ `() => { HidePressToRebindKey();`��`HidePressToRebindKey()` ������ԭ����ִ�����ʱ�ص��� 

        //`Action` ��һ��ί�У�delegate�����ͣ�����ʾһ����������Ҳ������ֵ�ķ�����
        //���� `() => {    HidePressToRebindKey();` ������Lambda���ʽ��Ϊ `Action` ���͵Ĳ������뺯��ʱ��
        //���Lambda���ʽ������Ϊһ�����������������ں����ڲ����á�

        //����ں����ڲ�ִ������� `Action` ��������ô���еĴ������ں���ִ�еĹ����б����ã�
        //�������ں���ִ����Ϻ�ص�����ˣ�����������£�`
        //HidePressToRebindKey()` ���ں���ִ�й����б����ã��������ں���ִ����Ϻ�ص���

        //��ִ��һ�������ĺ������������������HidePressToRebindKey()����
        //���仰˵������һ��Lambda���ʽ��Ϊ�������ݸ���һ������ʱ��
        //���Lambda���ʽʵ��������Ϊһ�����ܻص���callback�����ݽ�ȥ�ġ�
        //�ڽ���Lambda���ʽ�ĺ����ڲ�ִ�����Լ����߼��󣬻�ִ�����Lambda���ʽ������Ĳ�����
        //���ֻ��Ƴ������¼������첽�ص��ȳ���������ʵ�����Ŀ������̺��߼�����
    }

    private void KitchenGameManager_OnGamePaused(object sender, System.EventArgs e) {
        Hide();
    }

    // Mathf.Round���ص���һ��int���ͣ������ַ���ƴ��+������ʽת��Ϊstring
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
    //Action ��һ��ί�����ͣ���ʾ�޲������޷���ֵ�ķ���
    //ͨ�����ַ�ʽ�������ڵ��� Show ����ʱ���ش���һ���հ�������closure����
    //�Ӷ�ʵ�����ض��¼�����ʱִ����Ӧ�Ĳ������������ģʽ������ʵ�ֻص����ܣ�
    //��������ť��ִ���ض��Ĳ�������Ӧ�û��رս���Ĳ�����
    //��һ��UI�Ŀ�����������һ��UI�Ĺر�
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
    
    //Э�����ڵ� MonoBehaivour ���󱻽��û����٣� �������Э�̵Ķ��󱻽��û����٣�Э��Ҳ��ִֹͣ�С�ȷ������Э�̵Ķ����Ǵ��ڼ���״̬������δ������
    
}


