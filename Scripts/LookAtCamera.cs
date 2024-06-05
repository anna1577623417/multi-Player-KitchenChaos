using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraBackwardInverted,
    }
    [SerializeField]private Mode mode; 

    private void LateUpdate() {
        switch (mode) {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform); 
                break;
            case Mode.LookAtInverted:
                //使得进度条始终都是从左到右，并且看向摄像头
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position+dirFromCamera);
                break;
            case Mode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraBackwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}
