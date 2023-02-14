using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private PlayerInputActions playerInputActions;

    private float horizontal;
    private bool isFacingRight = true;

    //Jump Logic
    private Vector2 move;
    private bool jump;
    private bool isGrounded;
    private float coyoteTime = 0.1f;
    private float coyoteCounter;
    public float jumpBufferLength = 0.05f;
    private float jumpBufferCount;

    // Edge logic

    // Wall Logic
    private bool isWallSliding, isWallJumping;
    private float wallJumpingCounter, wallJumpingDirection; 
    [SerializeField] float wallJumpingTime = 0.8f, wallJumpingDuration = 0.8f;
    [SerializeField] Vector2 wallJumpingPower = new Vector2(4f, 1f);
    [SerializeField] float wallSlidingSpeed;

    [SerializeField] float movementSpeed, jumpHeight;                                    
    [SerializeField] private LayerMask groundLayer, wallLayer;
    [SerializeField] private Transform groundCheck, wallCheck;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        GetPlayerInputActions();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");                                    // Flips the image by moving
        MovementInput();
        OnJump();
        WallSlide();
        WallJump();;

        if(!isWallJumping)
        {
            FlipImage();
        }
    }

    private void FixedUpdate()
    {
        if(!isWallJumping)
        {
            rb2D.velocity = new Vector2(horizontal, rb2D.velocity.y);                             // Control players speed    
        }

        MovementOutput();
        IsGrounded();
    }

    void GetPlayerInputActions()
    {
        playerInputActions = new PlayerInputActions();                                                  // Access to the PlayerInputActions script
        playerInputActions.Player.Enable();
        //playerInputActions.Player.Jump.performed += JumpLogic;
    }

    void MovementInput()
    {
        move = playerInputActions.Player.Movement.ReadValue<Vector2>();
    }

    void MovementOutput()
    {
        rb2D.AddForce(move * movementSpeed * (100 * Time.fixedDeltaTime), ForceMode2D.Force);
    }

    void OnJump()
    {
        jump = playerInputActions.Player.Jump.WasPerformedThisFrame();      //Same as GetButtonDown

        //Jump buffer
        if(jump)
        {
            jumpBufferCount = jumpBufferLength;
        }
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }

        //Coyote Time
        if(isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else 
        {
            coyoteCounter -= Time.deltaTime;
        }

        //if Coyote Time and Jump Buffer are true, Jump
        if(jumpBufferCount >= 0 && coyoteCounter > 0f)
        {
            rb2D.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            coyoteCounter = 0;
            jumpBufferCount = 0;
        }
        else if(!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        else if(isGrounded)
        {
            animator.SetBool("isJumping", false);
        }

        //Jump higher if Jump is pressed longer
        if (playerInputActions.Player.Jump.WasReleasedThisFrame() && rb2D.velocity.y > 0)       //Same as GetButtonUp
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.5f);
        }
    }

    void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(playerInputActions.Player.Jump.triggered && wallJumpingCounter > 0f)
        {
            playerInputActions.Player.Movement.Disable();
            isWallJumping = true;
            rb2D.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if(transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }

    }

    void StopWallJumping()
    {
        playerInputActions.Player.Movement.Enable();        
        isWallJumping = false;
    }

    void WallSlide()
    {
        if(OnWall() && !isGrounded && horizontal != 0f)
        {
            isWallSliding = true;
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void IsGrounded()                                                                           // Check if player is on ground
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool OnWall()                                                                               // Check if player is on wall
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void FlipImage()
    {
        // Flip player image to left or right while moving
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
