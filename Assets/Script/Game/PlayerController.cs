using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    None,
    Plane
}

public class PlayerController : MonoBehaviour
{
    readonly Vector3 flippedScale = new Vector3(-1, 1, 1);

    [Header("Movement")]
    [SerializeField] float Speed = 0.0f;
    [SerializeField] float jumpForce = 0.0f;
    [SerializeField] float minFlipSpeed = 0.1f;
    [SerializeField] float jumpGravityScale = 1.0f;
    [SerializeField] float fallGravityScale = 1.0f;
    [SerializeField] bool resetSpeedOnLand = false;

    // Input
    private Vector2 movementInput;
    private bool jumpInput;

    // Player Component
    private Animator animator;
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private LayerMask middleGroundMask;

    // Player State
    private Vector2 prevVelocity;
    private GroundType groundType;
    private bool isFlipped;
    private bool isJumping;
    private bool isFalling;

    // Animator paramater
    private int animatorGroundedBool;
    private int animatorRunningSpeed;
    private int animatorJumpTrigger;
    private int animatorTransformTrigger;
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
        if (!isJumping && Input.GetKeyDown(KeyCode.Space))
            jumpInput = true;

        
    }

    private void FixedUpdate()
    {
        UpdateGrounding();
        UpdateVelocity();
        UpdateDirection();
        UpdateJump();
        UpdateGravityScale();
        UpdatePlayerState();
    }

    #region ¸üÐÂº¯Êý 
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
        if (rigidbody.velocity.x > minFlipSpeed && isFlipped)
        {
            isFlipped = false;
            transform.localScale = Vector3.one;
        }
        else if (rigidbody.velocity.x < -minFlipSpeed && !isFlipped)
        {
            isFlipped = true;
            transform.localScale = flippedScale;
        }
    }
    private void UpdateGravityScale()
    {
        // Use grounded gravity scale by default.
        var gravityScale = jumpGravityScale;
        gravityScale = rigidbody.velocity.y > 0.0f ? jumpGravityScale : fallGravityScale;
        rigidbody.gravityScale = gravityScale;
    }
    void UpdateJump()
    {
        // Set falling flag
        if (isJumping && rigidbody.velocity.y < 0)
            isFalling = true;

        // Jump
        if (jumpInput && groundType != GroundType.None)
        {
            // Jump using impulse force
            rigidbody.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            // Set animator
            animator.SetTrigger(animatorJumpTrigger);

            // We've consumed the jump, reset it.
            jumpInput = false;

            // Set jumping flag
            isJumping = true;

            // TODO Play audio
        }

        // Landed
        else if (isJumping && isFalling && groundType != GroundType.None)
        {
            // Since collision with ground stops rigidbody, reset velocity
            if (resetSpeedOnLand)
            {
                prevVelocity.y = rigidbody.velocity.y;
                rigidbody.velocity = prevVelocity;
            }

            // Reset jumping flags
            isJumping = false;
            isFalling = false;

            // TODO Play audio
        }
    }
    void UpdatePlayerState()
    {

    }
    #endregion
}
