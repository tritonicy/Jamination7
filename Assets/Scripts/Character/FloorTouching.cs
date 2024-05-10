using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTouching : MonoBehaviour
{
    public static bool isTouchingFloor = false;
    public static bool canDoubleJump = false;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.IsTouchingLayers(LayerMask.GetMask("Feet"))) {
            isTouchingFloor = true;
            canDoubleJump = false;
        }
    }
    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Ground") {
            isTouchingFloor = false;

        }
    }
}
