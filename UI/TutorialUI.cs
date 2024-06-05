using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI keyMoveUpText;
    [SerializeField] private TextMeshProUGUI keyMoveDownText;
    [SerializeField] private TextMeshProUGUI keyMoveLeftText;
    [SerializeField] private TextMeshProUGUI keyMoveRightText;
    [SerializeField] private TextMeshProUGUI keyInteractText;
    [SerializeField] private TextMeshProUGUI keyInteractAlternateText;
    [SerializeField] private TextMeshProUGUI keyPauseText;
    [SerializeField]private TextMeshProUGUI keyToPlayer;

    private void Start() {
        GameInput.instance.OnBindingReBind += GameInput_OnBindingReBind;
        //KitchenGameManager.instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        KitchenGameManager.instance.OnLocalPlayerReadyChange += KitchenGameManager_OnLocalPlayerReadyChange;

        UpdateVisual();

        Show();
    }

    private void KitchenGameManager_OnLocalPlayerReadyChange(object sender, System.EventArgs e) {
        if (KitchenGameManager.instance.IsLocalPlayerReady()) {
            Hide();
        }
    }

    //private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
    //    if (KitchenGameManager.instance.IsCountdownToStartActive()) {
    //        Hide();
    //    }
    //}

    private void GameInput_OnBindingReBind(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        keyMoveUpText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Up);
        keyMoveDownText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Down);
        keyMoveLeftText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Left);
        keyMoveRightText.text = GameInput.instance.GetBindingText(GameInput.Binding.Move_Right);
        keyInteractText.text = GameInput.instance.GetBindingText(GameInput.Binding.Interact);
        keyInteractAlternateText.text = GameInput.instance.GetBindingText(GameInput.Binding.InteractAlternate);
        keyPauseText.text = GameInput.instance.GetBindingText(GameInput.Binding.Pause);
        keyToPlayer.text = keyInteractText.text;
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
