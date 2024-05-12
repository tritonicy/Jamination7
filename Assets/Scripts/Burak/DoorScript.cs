using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isClosed = true;
    private CinemachineFollow vcam;
    private PlayerController playerController;
    private bool isTouchingDoor = false;

    private void Start() {
        vcam = FindObjectOfType<CinemachineFollow>();
        playerController = FindObjectOfType<PlayerController>();
    }
    private void Update() {      
        if(isClosed) {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
        else{
            this.GetComponent<SpriteRenderer>().enabled = true;
        }

        if(!isClosed && Input.GetKeyDown(KeyCode.E) && isTouchingDoor) {
            vcam.currentLevel++;
            vcam.TeleportUzayli();
        }
    }

    public bool ChangeDoorState() {
        this.isClosed = !isClosed;
        return isClosed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            isTouchingDoor = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player") {
            isTouchingDoor = false;
        }
    }


}
