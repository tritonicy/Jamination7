using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header ("Movement")]
    private Vector2 moveValue;
    [SerializeField] float jumpValue = 1f;
    [SerializeField] float moveSpeed = 0.5f;
    [SerializeField] Vector2 wallJumpingPower = new Vector2(8f,16f);
    [SerializeField] float glidingSpeed;
    [SerializeField] float wallSlideSpeed;
    [SerializeField] float swimmingSpeed = 1.5f;
    [SerializeField] Vector3 extendCircle;

    [Header ("Components")]
    private Rigidbody2D rb;
    [Header ("Other")]
    [SerializeField] GameObject current;
    private Rigidbody2D currentRB;
    [Header ("Bools, timers and layers")]
    private float coyotaTimeCounter;
    private float coyotaTime = 0.2f;
    private float jumpBufferCounter;
    private float jumpBufferTime = 0.5f;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private float initialGravityScale;
    private bool isGliding;
    private bool isSwimming;
    private Vector3 originalScale;
    public Vector3 smallScale;
    private Transform wallCheck;
    private EnemyData enemyData;
    private bool isSmall = false;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask playerLayer;

    private void Awake() {
        currentRB = current.GetComponent<Rigidbody2D>();
        enemyData = current.GetComponent<EnemyData>();
        initialGravityScale = currentRB.gravityScale;
        wallCheck = current.transform.Find("WallCheck");
    }

    private void Start()
    {
        originalScale = current.transform.gameObject.transform.localScale;
    }
    void Update()
    {
        ChangeBody();
        ChangeSize();

        if (enemyData.isJumpMaster)
        {
            WallSlide();
        }


        if(isWallSliding && !isSwimming && enemyData.isJumpMaster){
            WallJump();
        }else{

            if (enemyData.isJumpMaster)
            {
                DoubleJump();
            }
            else
            {
                Jump();
            }
        }

        if(!isWallSliding && enemyData.canSwim) {
            Swim();
        }
    }
    
    private void FixedUpdate() {

        if (enemyData.canGlide)
        {
            Glide();
        }

        if(!isWallJumping) {
            Move();
            FlipSprite();
        }

    }

    private void ChangeBody() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position + new Vector3(extendCircle.x * current.transform.localScale.x, extendCircle.y, extendCircle.z), 0.3f, playerLayer); //localscale duzenlenecek
            if(colliders.Length > 0) {
                current = colliders[0].gameObject;
                enemyData = current.GetComponent<EnemyData>();
                Debug.Log(colliders.Length);
                wallCheck = current.transform.Find("WallCheck");
                currentRB = current.GetComponentInChildren<Rigidbody2D>();
            }
        }
    }

    private void ChangeSize()
    {
        if (Input.GetKeyDown(KeyCode.C) && isSmall)
        {
            isSmall = false;
            current.gameObject.transform.localScale = Vector3.Lerp(current.gameObject.transform.localScale, originalScale,1f);
            current.gameObject.transform.position += new Vector3(0f, originalScale.y - smallScale.y, 0f);
        }
        else if (Input.GetKeyDown(KeyCode.C) && !isSmall)
        {
            isSmall = true;
            current.gameObject.transform.localScale = Vector3.Lerp(current.gameObject.transform.localScale, smallScale, 1f);
        }
    }
    private void OnDrawGizmos() {
        if(wallCheck != null) {
            UnityEditor.Handles.DrawWireDisc(wallCheck.position + new Vector3(extendCircle.x * current.transform.localScale.x, extendCircle.y, extendCircle.z), new Vector3(0, 0, 1), 0.3f);
        }
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

    private bool isWalled() {
        return Physics2D.OverlapCircle(wallCheck.position,0.2f, wallLayer);
    }

    private void WallSlide() {
        if(isWalled() && !FloorTouching.isTouchingFloor && moveValue.x != 0f) {
            isWallSliding = true;
            currentRB.velocity = new Vector2(currentRB.velocity.x, Mathf.Clamp(currentRB.velocity.y, -wallSlideSpeed, float.MaxValue));
        }else{
            isWallSliding = false;
        }
    }
    private void WallJump() {
        if(isWallSliding) {
            isWallJumping = false;
            wallJumpingDirection = -current.gameObject.transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else{
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")) {
            isWallJumping = true;
            currentRB.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if(current.gameObject.transform.localScale.x != wallJumpingDirection) {
                FlipSprite();
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

    }

    private void Glide() {
        if(Input.GetKey(KeyCode.E) && currentRB.velocity.y < 0f) {
            currentRB.gravityScale = 0f;
            currentRB.velocity = new Vector2(currentRB.velocity.x, -glidingSpeed);
            isGliding = true;
        }
        else{
            isGliding = false;
            currentRB.gravityScale = initialGravityScale;
        }
    }
    private void Swim() {
        if(Physics2D.OverlapCircle(currentRB.gameObject.transform.position, 0.2f, waterLayer)) {
            currentRB.gravityScale = 0f;
            isSwimming = true;
            if(moveValue.y > 0) {
                currentRB.velocity = new Vector2(currentRB.velocity.x, glidingSpeed * swimmingSpeed);
            }
            else{
                currentRB.velocity = new Vector2(currentRB.velocity.x, -glidingSpeed * swimmingSpeed);

            }
        }
        else{
            isSwimming = false;
            currentRB.gravityScale = initialGravityScale;
        }
    }

    private void StopWallJumping() {
        isWallJumping = false;
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
        if((coyotaTimeCounter > 0f || FloorTouching.canDoubleJump) && jumpBufferCounter > 0 && !isWalled()) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,jumpValue);
            jumpBufferCounter = 0f;
            FloorTouching.canDoubleJump = !FloorTouching.canDoubleJump;
        
        }

        if(currentRB.velocity.y > 0 && Input.GetButtonUp("Jump")) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,currentRB.velocity.y * 0.5f);
            coyotaTimeCounter = 0f;
        }
    }
    
    private void FlipSprite() {
        Vector3 localScale = current.gameObject.transform.localScale;
        if(current.TryGetComponent<Rigidbody2D>(out Rigidbody2D component)) {
            if(component.velocity.x > 0 && localScale.x < 0) {
                localScale.x = -localScale.x;
            }
            if(component.velocity.x < 0 && localScale.x > 0) {

                localScale.x = -localScale.x;
            }
        current.gameObject.transform.localScale = localScale;
        }
    }

}
