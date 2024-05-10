using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header ("Scripts")]
    FloorTouching floorTouching;
    [Header ("Movement")]
    private Vector2 moveValue;
    [SerializeField] float jumpValue = 1f;
    [SerializeField] float moveSpeed = 0.5f;
    [Header ("Components")]
    private Rigidbody2D rb;
    [Header ("Other")]
    [SerializeField] GameObject current;
    private Rigidbody2D currentRB;
    private float coyotaTimeCounter;
    private float coyotaTime = 0.2f;
    private float jumpBufferCounter;
    private float jumpBufferTime = 0.5f;


    void Start()
    {   
        floorTouching = FindObjectOfType<FloorTouching>();
        current = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
        currentRB = current.GetComponent<Rigidbody2D>(); 
    }

    void Update()
    {   
        Move();
        //Jump();
        DoubleJump();
    }

    private void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
    }

    private void Move() {
        currentRB.velocity  = new Vector2(moveValue.x * moveSpeed, currentRB.velocity.y);
    }
    private void Jump() {
        if(FloorTouching.isTouchingFloor) {
            coyotaTimeCounter = coyotaTime; 
        }
        else{
            coyotaTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        }
        else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if(coyotaTimeCounter > 0f && jumpBufferCounter > 0f) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,jumpValue);
            jumpBufferCounter = 0f;
        }

        if(currentRB.velocity.y > 0 && Input.GetButtonUp("Jump")) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,currentRB.velocity.y * 0.5f);
            coyotaTimeCounter = 0f;
        }
    }


    private void DoubleJump() {
        if(FloorTouching.isTouchingFloor) {
            coyotaTimeCounter = coyotaTime; 
        }
        else{
            coyotaTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        }
        else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if((coyotaTimeCounter > 0f || FloorTouching.canDoubleJump) && jumpBufferCounter > 0) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,jumpValue);
            jumpBufferCounter = 0f;
            FloorTouching.canDoubleJump = !FloorTouching.canDoubleJump;
        
        }

        if(currentRB.velocity.y > 0 && Input.GetButtonUp("Jump")) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,currentRB.velocity.y * 0.5f);
            coyotaTimeCounter = 0f;
        }
    }


}
