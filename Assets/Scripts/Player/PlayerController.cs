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
    Vector2 lookDirection = new Vector2(1,0);

    //Jump Logic
    private Vector2 move;
    private bool jump;
    private bool isGrounded;
    private float coyoteTime = 0.1f;
    private float coyoteCounter;
    public float jumpBufferLength = 0.05f;
    private float jumpBufferCount;

    //Attack Logic
    private bool attack;

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
        OnAttack();
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
        playerInputActions = new PlayerInputActions();                                            // Access to the PlayerInputActions script
        playerInputActions.Player.Enable();
        //playerInputActions.Player.Jump.performed += JumpLogic;
    }

    void MovementInput()
    {
        move = playerInputActions.Player.Movement.ReadValue<Vector2>();
        
        //Calculate lookDirection for Raycast
        if (!Mathf.Approximately(move.x, 0.0f))                                                     //Only X because Sidescroller? If needed || !Mathf.Approximately(move.y, 0.0f)
        {
            lookDirection.Set(move.x, 0);                                                           //y is 0 because Sidescroller? If needed lookDirection.Set(move.x, move.y)
            lookDirection.Normalize();
        }
    }

    void MovementOutput()
    {
        rb2D.AddForce(move * movementSpeed * (100 * Time.fixedDeltaTime), ForceMode2D.Force);

        //Animation Stuff
        if(rb2D.velocity.x > 0 || rb2D.velocity.x < 0)
        {
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
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
            animator.SetTrigger("Jump");
            coyoteCounter = 0;
            jumpBufferCount = 0;
        }
        
        animator.SetFloat("AirSpeedY", rb2D.velocity.y);
        animator.SetBool("Grounded", isGrounded);

        //Jump higher if Jump is pressed longer
        if (playerInputActions.Player.Jump.WasReleasedThisFrame() && rb2D.velocity.y > 0)       //Same as GetButtonUp
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y * 0.5f);
        }
    }

    void OnAttack()
    {
        attack = playerInputActions.Player.Attack.WasPressedThisFrame();
        if(attack)
        {
            animator.SetTrigger("Attack1");
            RaycastHit2D hit = Physics2D.Raycast(rb2D.position + Vector2.up * 0.2f, lookDirection, 1.2f, LayerMask.GetMask("Enemy"));
            if (hit.collider != null)
            {
                CutTree tree = hit.collider.GetComponent<CutTree>();
                if(tree != null)
                {
                    tree.TreeFalling();
                }
            }
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
        if(isFacingRight && rb2D.velocity.x < 0 || !isFacingRight && rb2D.velocity.x > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
