using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarUI : MonoBehaviour
{

    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;
    private void Start() {
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();

        if (hasProgress == null ) {
            //Debug.LogError("has not a IHasProgress!");
        }

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount =0f;

        Hide();
    }
    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized==0f||e.progressNormalized==1f) {
            Hide();
            //事件被触发并且progressNormalized=0，表示刚放到切菜桌上
            //事件被触发并且progressNormalized=1时，表示切菜完成
        } else {
            Show();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }

}
