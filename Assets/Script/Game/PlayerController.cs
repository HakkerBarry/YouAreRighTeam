using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    None,
    Plane
}
public enum PlayerState
{
    Alive,
    Dead
}

public class PlayerController : MonoBehaviour
{
    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("正常状态")]
    [SerializeField] float NormalSpeed = 0.0f;
    [SerializeField] float NormalJumpSpeed = 0.0f;
    [SerializeField] float NormalMinFlipSpeed = 0.1f;
    [SerializeField] float NormalJumpGravityScale = 1.0f;
    [SerializeField] float NormalFallGravityScale = 1.0f;

    [Header("梦游状态")]
    [SerializeField] float SleepSpeed = 0.0f;
    [SerializeField] float SleepJumpSpeed = 0.0f;
    [SerializeField] float SleepMinFlipSpeed = 0.1f;
    [SerializeField] float SleepJumpGravityScale = 1.0f;
    [SerializeField] float SleepFallGravityScale = 1.0f;
    [SerializeField] float DashDuration = 0.2f;
    [SerializeField] float DashSpeed = 100.0f;

    [Header("Other")]
    [SerializeField] bool resetSpeedOnLand = false;
    [SerializeField] Transform footPoint;
    [SerializeField] GameObject dashTrail;

    [Header("Input")]


    private float Speed = 0.0f;
    private float JumpSpeed = 0.0f;
    private float MinFlipSpeed = 0.1f;
    private float JumpGravityScale = 1.0f;
    private float FallGravityScale = 1.0f;


    // Input
    private Vector2 movementInput;
    [SerializeField] private bool jumpInput;
    private bool transformInput;
    private bool dashInput;

    // Player Component
    private Animator animator;
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private LayerMask middleGroundMask;

    // Player State
    private Vector2 prevVelocity;
    private GroundType groundType;
    private Vector2 preDashVelocity;
    private float ConstantDashDuration;
    [Header("Debug")]
    [SerializeField] bool isFlipped;
    [SerializeField] bool isJumping;
    [SerializeField] bool isFalling;
    [SerializeField] bool isSleeping = false;
    [SerializeField] bool canJumpAgain = false;
    [SerializeField] bool inAir;
    [SerializeField] bool isDashing = false;

    // Animator paramater
    private int animatorGroundedBool;
    private int animatorRunningSpeed;
    private int animatorJumpTrigger;
    private int animatorTransformTrigger;
    private int animatorFlipTrigger;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        middleGroundMask = LayerMask.GetMask("MiddleBackground");

        // Animator paramater Hash id
        animatorGroundedBool = Animator.StringToHash("Grounded");
        animatorRunningSpeed = Animator.StringToHash("RunningSpeed");
        animatorJumpTrigger = Animator.StringToHash("Jump");
        animatorTransformTrigger = Animator.StringToHash("Transform");
        animatorFlipTrigger = Animator.StringToHash("Flip");

        // init player property
        SetNormalProperty();
        dashTrail.SetActive(false);
        ConstantDashDuration = DashDuration;
    }
    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        float moveHorizontal = 0.0f;

        if (Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow))
            moveHorizontal = -1.0f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveHorizontal = 1.0f;
        movementInput = new Vector2(moveHorizontal, 0);

        // Jumping input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!isSleeping)
            {
                if (!isJumping&&!inAir)
                    jumpInput = true;
            }
            else
            {
                jumpInput = true;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashInput = true;
        }
            

        if (Input.GetKeyDown(KeyCode.Tab))
            transformInput = true;
        
    }

    private void FixedUpdate()
    {
        UpdateTranform();
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateDash();
        UpdateJump();
        UpdateGravityScale();
        UpdatePlayerState();
    }

    #region 更新函数 
    private void UpdateDash()
    {
        if (!isSleeping)
        {
            dashInput = false;
            return;
        }
        if(dashInput)
        {
            preDashVelocity = rigidbody.velocity;
            if(isFlipped)
                rigidbody.velocity = new Vector2(-DashSpeed, preDashVelocity.y);
            else
                rigidbody.velocity = new Vector2(DashSpeed, preDashVelocity.y);
            isDashing = true;
            dashTrail.SetActive(true);
            dashInput = false;
        }
        if(isDashing)
        {
            ConstantDashDuration -= Time.fixedDeltaTime;
            if(ConstantDashDuration<0)
            {
                ConstantDashDuration = DashDuration;
                rigidbody.velocity = preDashVelocity;
                dashTrail.SetActive(false);
                isDashing = false;
            }
        }


    }
    private void UpdateTranform()
    {
        if (transformInput)
        {
            transformInput = false;
            // 判断是否睡眠，修改玩家参数
            if (isSleeping)
                SetNormalProperty();
            else
                SetSleepProperty();
            isSleeping = !isSleeping;
            animator.SetTrigger(animatorTransformTrigger);
        }
    }
    private void UpdateGrounding()
    {
        // Use character collider to check if touching ground layers
        if (collider.IsTouchingLayers(middleGroundMask))
            groundType = GroundType.Plane;
        else
            groundType = GroundType.None;

        // Update animator
        animator.SetBool(animatorGroundedBool, groundType != GroundType.None);
    }
    void UpdateVelocity()
    {
        if (isDashing)
            return;
        // No acceleration
        Vector2 velocity = rigidbody.velocity;
        velocity.x = (movementInput * Speed).x;
        movementInput = Vector2.zero;
        rigidbody.velocity = velocity;

        // Update animator
        var horizontalSpeedNormalized = Mathf.Abs(velocity.x) / Speed;
        animator.SetFloat(animatorRunningSpeed, horizontalSpeedNormalized);

        // TODO Play audio
       
    }
    void UpdateDirection()
    {
        if (rigidbody.velocity.x > MinFlipSpeed && isFlipped)
        {
            isFlipped = false;
            transform.localScale = Vector3.one;
        }
        else if (rigidbody.velocity.x < -MinFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            transform.localScale = flippedScale;
        }
    }
    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = JumpGravityScale;
        gravityScale = rigidbody.velocity.y > 0.0f ? JumpGravityScale : FallGravityScale;
        rigidbody.gravityScale = gravityScale;
    }
    void UpdateJump()
    {
        // 正常模式
        if(!isSleeping)
        {
            // Set falling flag
            if (isJumping && rigidbody.velocity.y < 0)
                isFalling = true;

            // Jump
            // Ray cast Ground
            RaycastHit2D hit = Physics2D.Raycast(footPoint.position, new Vector2(0, -1), 0.2f, middleGroundMask);
            if (jumpInput && groundType != GroundType.None && hit)
            {
                Vector2 velocity = rigidbody.velocity;
                velocity.y = JumpSpeed;
                rigidbody.velocity = velocity;
                animator.SetTrigger(animatorJumpTrigger);
                jumpInput = false;
                isJumping = true;
            }

            // Landed
            else if (isJumping && isFalling && groundType != GroundType.None)
            {
                if (resetSpeedOnLand)
                {
                    prevVelocity.y = rigidbody.velocity.y;
                    rigidbody.velocity = prevVelocity;
                }
                isJumping = false;
                isFalling = false;
                // TODO Play audio
            }
            else if (isJumping && !isFalling)
            {
                isJumping = false;
            }
        }
        // 梦游模式
        else
        { 
            // Set falling flag
            if (isJumping && rigidbody.velocity.y < 0)
                isFalling = true;

            // Jump
            // Ray cast Ground
            if(jumpInput)
            {
                if(inAir)
                {
                    if(canJumpAgain)
                    {
                        Vector2 velocity = rigidbody.velocity;
                        velocity.y = JumpSpeed;
                        rigidbody.velocity = velocity;
                        animator.SetTrigger(animatorJumpTrigger);
                        jumpInput = false;
                        isJumping = true;
                        canJumpAgain = false;
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(footPoint.position, new Vector2(0, -1), 0.2f, middleGroundMask);
                    if (groundType != GroundType.None && hit)
                    {
                        Vector2 velocity = rigidbody.velocity;
                        velocity.y = JumpSpeed;
                        rigidbody.velocity = velocity;
                        animator.SetTrigger(animatorJumpTrigger);
                        jumpInput = false;
                        isJumping = true;
                        canJumpAgain = true;
                    }
                }
                
            }
            // Landed
            else if (isJumping && isFalling && groundType != GroundType.None)
            {
                if (resetSpeedOnLand)
                {
                    prevVelocity.y = rigidbody.velocity.y;
                    rigidbody.velocity = prevVelocity;
                }
                isJumping = false;
                isFalling = false;
                canJumpAgain = false;
                // TODO Play audio
            }
            //else if (isJumping && !isFalling)
            //{
            //    isJumping = false;
            //}
        }
        
    }
    void UpdatePlayerState()
    {
        RaycastHit2D hit = Physics2D.Raycast(footPoint.position, new Vector2(0, -1), 0.3f, middleGroundMask);
        Debug.DrawLine(footPoint.position, footPoint.position + new Vector3(0, -0.3f, 0));
        if (!hit)
        {
            if (!inAir)
                canJumpAgain = true;
            inAir = true;
        }
        else
            inAir = false;
    }
    #endregion

    void SetSleepProperty()
    {
        Speed = SleepSpeed;
        JumpSpeed = SleepJumpSpeed;
        MinFlipSpeed = SleepMinFlipSpeed;
        JumpGravityScale = SleepJumpGravityScale;
        FallGravityScale = SleepFallGravityScale;
    }

    void SetNormalProperty()
    {
        Speed = NormalSpeed;
        JumpSpeed = NormalJumpSpeed;
        MinFlipSpeed = NormalMinFlipSpeed;
        JumpGravityScale = NormalJumpGravityScale;
        FallGravityScale = NormalFallGravityScale;
    }
}
