using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{   
    private bool isPlayerTouching = false;
    [SerializeField] DoorScript doorScript;
    private CinemachineFollow vcam;

    private void Start() {
        vcam = FindObjectOfType<CinemachineFollow>();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerTouching) {
            Debug.Log(isPlayerTouching);
            doorScript.ChangeDoorState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            Debug.Log("2");
            isPlayerTouching = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            isPlayerTouching = false;
        }
    }
}
