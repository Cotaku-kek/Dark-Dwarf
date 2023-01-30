using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private bool isFacingRight = true;
    private float movementSpeed = 50f;
    private float jumpHeight = 15f;
    private Rigidbody2D rigidbody;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    public Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        GetPlayerInputActions();
    }

    void Update()
    {
        Flip();
        Movement();
        horizontal = Input.GetAxisRaw("Horizontal");
        //animator.SetBool("isJumping", false);                                                     // temporaer
    }

    void GetPlayerInputActions()
    {
        playerInputActions = new PlayerInputActions();                                              // Access to the PlayerInputActions script
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
            print("Jump! " + context.phase);                                                        // Check which phase is active (started, performed or canceled)
            rigidbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);                       
            animator.SetBool("isJumping", true);
        }
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(horizontal, rigidbody.velocity.y);                         // Control players speed    
    }

    private bool IsGrounded()
    {
        // Invisible circle around player, which collides with ground
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
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
