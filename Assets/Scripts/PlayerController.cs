using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;

    private float horizontal;
    private bool isFacingRight = true;

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
        rigidbody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider = GetComponent<BoxCollider2D>();
        GetPlayerInputActions();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Movement();
        WallSlide();
        WallJump();;

        if(!isWallJumping)
        {
            FlipImage();
        }

        //animator.SetBool("isJumping", false);                                                         // temporaer

    }

    private void FixedUpdate()
    {
        if(!isWallJumping)
        {
            rigidbody.velocity = new Vector2(horizontal, rigidbody.velocity.y);                             // Control players speed    
        }
    }

    void GetPlayerInputActions()
    {
        playerInputActions = new PlayerInputActions();                                                  // Access to the PlayerInputActions script
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;
    }

    void Movement()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        rigidbody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * movementSpeed, ForceMode2D.Force);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded())
        {
            print("Jump! " + context.phase);                                                            // Check which phase is active (started, performed or canceled)
            rigidbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);                       
            animator.SetBool("isJumping", true);
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
            rigidbody.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
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
        if(OnWall() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private bool IsGrounded()                                                                           // Check if player is on ground
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
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
