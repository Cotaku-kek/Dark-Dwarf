using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;

    public float movementSpeed;             
    public float jumpHeight;                
    public float walljumpPower;
    //public float walljumpPowerSide;          

    public Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private float wallJumpCooldown;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider = GetComponent<BoxCollider2D>();
        GetPlayerInputActions();
    }

    void Update()
    {
        FlipImage();
        Movement();
        WallJump();
        horizontal = Input.GetAxisRaw("Horizontal");
        //animator.SetBool("isJumping", false);                                                         // temporaer

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
        else if(context.performed && !IsGrounded() && OnWall())
        {
            rigidbody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x), walljumpPower);       // * wallJumpPowerSide, 
            wallJumpCooldown = 0;
        }
    }

    void WallJump()
    {
        if(wallJumpCooldown > 0.2f)
        {
            if(OnWall() && !IsGrounded())
            {
                rigidbody.gravityScale = 0;
                rigidbody.velocity = Vector2.zero;
            }
            else
            {
                rigidbody.gravityScale = 3;
            }
            playerInputActions.Player.Jump.performed += Jump;
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(horizontal, rigidbody.velocity.y);                         // Control players speed    
    }

    private bool IsGrounded()                                                                       // Check if player is on ground
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()                                                                           // Check if player is on wall
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
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
