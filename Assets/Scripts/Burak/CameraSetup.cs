using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera closeCam;


    private void Update() {
        if(Input.GetKeyDown(KeyCode.T)) {
            ChangeCamera();
        }
    }
    private void ChangeCamera() {
        if(closeCam.Priority > 10) {
            closeCam.Priority = 5;
        }
        else{
            closeCam.Priority = 12;
        }
    }


}


