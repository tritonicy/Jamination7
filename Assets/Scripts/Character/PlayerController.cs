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
    [SerializeField] float runningSpeed;
    [SerializeField] Vector3 extendCircle;
    [SerializeField] public GameObject mainObject;

    [Header ("Components")]
    public Rigidbody2D rb;
    [Header ("Other")]
    [SerializeField] public GameObject current;
    public Rigidbody2D currentRB;
    [Header ("Bools, timers and layers")]
    private float coyotaTimeCounter;
    private float coyotaTime = 0.2f;
    private float jumpBufferCounter;
    private float jumpBufferTime = 0.5f;
    private bool isWallSliding = false;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private float initialGravityScale;
    private bool isGliding;
    private bool isSwimming;
    private bool canLeftBody = false;
    private bool isTouchingFloor;
    private bool canDoubleJump;
    public Transform feetCheck;
    private Vector3 originalScale;
    public Vector3 smallScale;
    public Transform wallCheck;
    public EnemyData enemyData;
    private bool isSmall = false;
    private bool isRunning = false;
    private bool isGrounded;
    private int jumpCounter;
    public Animator currentAnimator;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask ladderLayer;

    private void Awake() {
        currentRB = current.GetComponent<Rigidbody2D>();
        enemyData = current.GetComponent<EnemyData>();
        initialGravityScale = currentRB.gravityScale;
        wallCheck = current.transform.Find("WallCheck");
        feetCheck = current.transform.Find("Feet");
        currentAnimator = current.GetComponent<Animator>();
    }

    private void Start()
    {
        originalScale = current.transform.gameObject.transform.localScale;
    }
    void Update()
    {
        ChangeBody();
        FindFeetCollider();

        if(enemyData.canBeSmall) {
            ChangeSize();
        }

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

        if (Input.GetKeyDown(KeyCode.X))
        {
            canLeftBody = true;
        }

        climbLadder();

        if(moveValue.x != 0)
        {
            currentAnimator.SetBool("run", true);
        }
        else
        {
            currentAnimator.SetBool("run", false);
        }
    }
    
    private void FixedUpdate() {

        if (enemyData.canGlide)
        {
            Glide();
        }

        if(enemyData.canRunFast && !isWallJumping) {
            Run();
            if(!isRunning && !isWallJumping) {
                Move();
            }
        }
        else if(!isWallJumping){
            Move();
        }

        if (!isWallJumping)
        {
            FlipSprite();
        }

        if (canLeftBody)
        {
            LeaveBody();
            Invoke(nameof(StopLeavingBody), 0.1f);
        }
    }
    private void ChangeBody() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position + new Vector3(extendCircle.x * current.transform.localScale.x, extendCircle.y, extendCircle.z), 0.3f, playerLayer); //localscale duzenlenecek
            if(colliders.Length > 0) {
                if(current == mainObject)
                {
                    current = colliders[0].gameObject;
                    enemyData = current.GetComponent<EnemyData>();
                    currentAnimator = current.GetComponent<Animator>();
                    wallCheck = current.transform.Find("WallCheck");
                    feetCheck = current.transform.Find("Feet");
                    currentRB = current.GetComponentInChildren<Rigidbody2D>();
                    currentRB.velocity = new Vector2(currentRB.velocity.x, currentRB.velocity.y + 2f);
                    mainObject.gameObject.SetActive(false);
                }
                else{
                    current = colliders[0].gameObject;
                    currentAnimator = current.GetComponent<Animator>();
                    enemyData = current.GetComponent<EnemyData>();
                    wallCheck = current.transform.Find("WallCheck");
                    feetCheck = current.transform.Find("Feet");
                    currentRB = current.GetComponentInChildren<Rigidbody2D>();
                    currentRB.velocity = new Vector2(currentRB.velocity.x, currentRB.velocity.y + 2f);
                }
            }
        }
    }
    private void Run(){
        if(Input.GetKey(KeyCode.LeftShift) && FloorTouching.isTouchingFloor) {
            currentRB.velocity = new Vector2(moveValue.x * moveSpeed * runningSpeed, currentRB.velocity.y);
            isRunning = true;
        } 
        else{
            isRunning = false;
        }
    }
    
    private void FindFeetCollider() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(feetCheck.transform.position ,0.5f);
        foreach (Collider2D collider in colliders)
        {
            if(collider.gameObject.tag == "Ground") {
                isGrounded = true;
                return;
            }            
        }
        isGrounded = false;
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
        if(isGrounded) {
            coyotaTimeCounter = coyotaTime;
            jumpCounter = 0;
            Invoke(nameof(SetJumpAnimator), 1f);
        }
        else{
            coyotaTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
            currentAnimator.SetBool("jump", true);
        }
        else{
            jumpBufferCounter -= Time.deltaTime;
        }
        if(coyotaTimeCounter > 0f && jumpBufferCounter > 0f && jumpCounter < 1) {
            currentRB.velocity = new Vector2(currentRB.velocity.x,jumpValue);
            jumpBufferCounter = 0f;
            jumpCounter++;
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
            currentAnimator.SetBool("jump", true);
            currentRB.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            if(current.gameObject.transform.localScale.x != wallJumpingDirection) {
                FlipSprite();
            }
            Invoke(nameof(SetJumpAnimator), 1f);
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
        currentAnimator.SetBool("jump", false);
    }

    private void DoubleJump() {
        if(FloorTouching.isTouchingFloor) {
            coyotaTimeCounter = coyotaTime;
            Invoke(nameof(SetJumpAnimator), 1f);
        }
        else{
            coyotaTimeCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
            currentAnimator.SetBool("jump", true);
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

    private void LeaveBody()
    {
        mainObject.gameObject.SetActive(true);
        mainObject.transform.position = current.transform.position;
        mainObject.GetComponent<SpriteRenderer>().enabled = true;
        current = mainObject;
        feetCheck = current.transform.Find("Feet");
        enemyData = current.GetComponent<EnemyData>();
        wallCheck = current.transform.Find("WallCheck");
        currentRB = current.GetComponentInChildren<Rigidbody2D>();
        currentAnimator = current.GetComponent<Animator>();
    }

    private void StopLeavingBody()
    {
        canLeftBody = false;
    }

    private void climbLadder()
    {
        if (Physics2D.OverlapCircle(currentRB.gameObject.transform.position, 0.5f, ladderLayer))
        {
            if (moveValue.y > 0)
            {
                currentRB.gravityScale = 0f;
                currentRB.velocity = new Vector2(currentRB.velocity.x, glidingSpeed * moveSpeed);
            }
            else if (moveValue.y < 0)
            {
                currentRB.gravityScale = 0f;
                currentRB.velocity = new Vector2(currentRB.velocity.x, -glidingSpeed * moveSpeed);
            }
            else
            {
                currentRB.gravityScale = 0f;
                currentRB.velocity = new Vector2(currentRB.velocity.x, 0f);
            }
        }
        else
        {
            currentRB.gravityScale = initialGravityScale;
        }
    }

    private void SetJumpAnimator()
    {
        currentAnimator.SetBool("jump", false);
    }
}
