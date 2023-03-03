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
    private AudioSource audioSource;

    private bool isFacingRight = true;
    Vector2 lookDirection = new Vector2(1,0);

    [Header("Health Mana")]
    [SerializeField] private float maxHealth = 5;
    [SerializeField] private float health {get {return currentHealth;}}
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
    [SerializeField] private bool isGrounded;
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteCounter;
    [SerializeField] private float jumpBufferLength = 0.05f;
    private float jumpBufferCount;

    [Header("Attack Logic")]
    [SerializeField] private bool isAttack;
    [SerializeField] private float attackBufferLength = 0.3f;
    private float attackBufferCount;

    [SerializeField] private float attack1Power = 1;
    [SerializeField] private float attack1Cooldown = 0.5f;
    [SerializeField] private float attack2Power = 0.8f;
    [SerializeField] private float attack2Cooldown = 0.4f;
    [SerializeField] private float attack3Power = 1.5f;
    [SerializeField] private float attack3Cooldown = 0.8f;
    [SerializeField] private float attackSwitch = 0;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackReset;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private bool isThrow;
    [SerializeField] private float throwCooldown;
    [SerializeField] private float throwCooldownTime = 1f;
    [SerializeField] private float throwingStrength = 2000;
    [SerializeField] public float throwingAxePower = 1;

    //Audio Logic
    //public AudioClip attack1Audio;
    //public AudioClip attack2Audio;
    //public AudioClip attack3Audio;

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
        audioSource = GetComponent<AudioSource>();
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
        isThrow = playerInputActions.Player.Throw.WasPerformedThisFrame();

        attackCooldown -= Time.deltaTime;
        throwCooldown -= Time.deltaTime;

        if (isThrow)
            LaunchAxe();

        if (isAttack)
            attackBufferCount = attackBufferLength;
        else 
            attackBufferCount -= Time.deltaTime;

        if (attackBufferCount > 0 && attackCooldown < 0)
        {
            switch(attackSwitch)
            {
                case 0:
                    animator.SetTrigger("Attack1");
                    AttackCalculation(attack1Power);
                    attackCooldown = attack1Cooldown;
                    attackSwitch++;
                    attackBufferCount = 0;
                    break;
                case 1:
                    animator.SetTrigger("Attack2");
                    AttackCalculation(attack2Power);
                    attackCooldown = attack2Cooldown;
                    attackSwitch++;
                    attackBufferCount = 0;
                    break;
                case 2:
                    animator.SetTrigger("Attack3");
                    AttackCalculation(attack3Power);
                    attackCooldown = attack3Cooldown;
                    attackSwitch = 0;
                    attackBufferCount = 0;
                    break;
                default:
                    attackSwitch = 0;
                    break;
            }
        }
        if (attackCooldown < -attackReset)
        {
            attackSwitch = 0;
        }
    }

    void AttackCalculation(float attackPower)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, attackMask);
        foreach (Collider2D hit in hits)
        {
            if (hit != null)
            {
                CutTree tree = hit.GetComponent<CutTree>();
                if(tree != null)
                {
                    tree.TreeFalling();
                }
                Enemy enemy = hit.GetComponent<Enemy>();
                if(enemy != null)
                {
                    enemy.ChangeHealth(-attackPower);
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void LaunchAxe()
    {
        if (throwCooldown < 0)
        {
            GameObject projectileObject = ThrowingAxePool.SharedInstance.GetPooledObject();  //Instantiate(projectilePrefab, rb2D.position + Vector2.up * 0.5f, Quaternion.identity);
            if (projectileObject != null)
            {
                projectileObject.transform.position = rb2D.position + Vector2.up * 0.5f;
                projectileObject.transform.rotation = Quaternion.identity;
                projectileObject.SetActive(true);
            }
            ThrowingAxe projectile = projectileObject.GetComponent<ThrowingAxe>();
            projectile.Throw(new Vector2(lookDirection.x,1f), throwingStrength);
            throwCooldown = throwCooldownTime;
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
        UIHealthBar.instance.SetValue(currentHealth / maxHealth);
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
        if (Mathf.Approximately(currentHealth, 0))
        {
            isDead = true;
            animator.SetTrigger("Death");
            playerInputActions.Player.Disable();
            rb2D.simulated = false;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}