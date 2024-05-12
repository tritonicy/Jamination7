using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{   
    private bool isPlayerTouching = false;
    [SerializeField] DoorScript doorScript;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.E) && isPlayerTouching) {
            doorScript.ChangeDoorState();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            isPlayerTouching = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            isPlayerTouching = false;
        }
    }
}
