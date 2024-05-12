using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isClosed = true;

    private void Update() {
        if(isClosed) {
            this.GetComponent<SpriteRenderer>().enabled = true;
        }
        else{
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public bool ChangeDoorState() {
        this.isClosed = !isClosed;
        return isClosed;
    }
}
