using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class CinemachineFollow : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    [SerializeField] GameObject gameManager;

    private void Start() {
        vcam = this.GetComponent<CinemachineVirtualCamera>();

    }

    private void Update() {
        vcam.Follow = gameManager.GetComponent<PlayerController>().current.transform;
    }
}
