using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Components
    public Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private PlayerInputActions playerInputActions;

    private bool isFacingRight = true;
    Vector2 lookDirection = new Vector2(1,0);

    //Health/Mana
    public float maxHealth = 5;
    public float health {get {return currentHealth;}}
    float currentHealth;
    private bool isDead = false;

    private bool isInvinsible;
    private float invinsibleTimer;
    private float timeInvincible = 1.5f;

    //Movement Logic
    private Vector2 moveInput;
    [SerializeField] float movementSpeed;

    //Jump Logic
    private bool jump;
    [SerializeField] float jumpHeight;
    private bool isGrounded;
    private float coyoteTime = 0.1f;
    private float coyoteCounter;
    public float jumpBufferLength = 0.05f;
    private float jumpBufferCount;

    //Attack Logic
    private bool isAttack;
    private float attack1Power = 1;
    private bool isAttack2;
    private float attack2Power = 0.8f;
    private bool isAttack3;
    private float attack3Power = 1.5f;

    // Edge logic

    // Wall Logic
    private bool isWallSliding, isWallJumping;
    private float wallJumpingCounter, wallJumpingDirection; 
    [SerializeField] float wallJumpingTime = 0.8f, wallJumpingDuration = 0.8f;
    [SerializeField] Vector2 wallJumpingPower = new Vector2(4f, 1f);
    [SerializeField] float wallSlidingSpeed;

    [SerializeField] private LayerMask groundLayer, wallLayer;
    [SerializeField] private Transform groundCheck, wallCheck;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        GetPlayerInputActions();
        
        currentHealth = maxHealth;
    }

    void Update()
    {
        MovementInput();
        OnJump();
        OnAttack();
        WallSlide();
        WallJump();
        Invinsible();

        if(!isWallJumping)
        {
            FlipImage();
        }
    }

    private void FixedUpdate()
    {
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
        moveInput = playerInputActions.Player.Movement.ReadValue<Vector2>();
        
        //Calculate lookDirection for Raycast
        if (!Mathf.Approximately(moveInput.x, 0.0f))                                                     //Only X because Sidescroller? If needed || !Mathf.Approximately(moveInput.y, 0.0f)
        {
            lookDirection.Set(moveInput.x, 0);                                                           //y is 0 because Sidescroller? If needed lookDirection.Set(moveInput.x, moveInput.y)
            lookDirection.Normalize();
        }
    }

    void MovementOutput()
    {
        rb2D.velocity = new Vector2(moveInput.x, rb2D.velocity.y);
        rb2D.AddForce(moveInput * movementSpeed * (100 * Time.fixedDeltaTime), ForceMode2D.Force);

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
        isAttack = playerInputActions.Player.Attack.WasPressedThisFrame();
        isAttack2 = playerInputActions.Player.Attack2.WasPressedThisFrame();
        if(isAttack)
        {
            animator.SetTrigger("Attack1");
            AttackCalculation(attack1Power);
        }
        else if (isAttack2)
        {
            animator.SetTrigger("Attack2");
            AttackCalculation(attack2Power);
        }
        else if (isAttack3)
        {
            animator.SetTrigger("Attack3");
            AttackCalculation(attack3Power);            
        }
    }

    void AttackCalculation(float attackPower)
    {
        RaycastHit2D hit = Physics2D.Raycast(rb2D.position + Vector2.up * 0.2f, lookDirection, 1.3f, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            CutTree tree = hit.collider.GetComponent<CutTree>();
            if(tree != null)
            {
                tree.TreeFalling();
            }
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.ChangeHealth(-attackPower);
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
        if(OnWall() && !isGrounded && moveInput.x != 0f)
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
        if(lookDirection.x < 0 && isFacingRight || !isFacingRight && lookDirection.x > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    //Methods regarding Health

    public void ChangeHealth(float amount)
    {
        if (amount < 0)
        {
            if (isInvinsible)
            {
                return;
            }
            animator.SetTrigger("Hurt");
            isInvinsible = true;
            invinsibleTimer = timeInvincible;
            isDeath();
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    private void Invinsible()
    {
        if(isInvinsible)
        {
            invinsibleTimer -= Time.deltaTime;
            if (invinsibleTimer < 0)
            {
                isInvinsible = false;
            }
        }
    }

    private void isDeath()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Death");
            playerInputActions.Player.Disable();
            rb2D.simulated = false;
        }
    }
}