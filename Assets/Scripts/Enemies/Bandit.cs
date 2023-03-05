using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy
{
    private float direction = -1;
    private float standStill = 1;
    Vector2 movement;

    private float attackCooldown;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool isGrounded = true;
    [SerializeField] private bool isWall = true;

    private Rigidbody2D rb2D;
    private Animator animator;

    // Start is called before the first frame update

    void FixedUpdate()
    {
        IsGrounded();
        IsWall();
        PatrolingArea();
    }

    void Update()
    {
        Attack();
    }


    protected override void Awake()
    {
        base.Awake();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }    

    protected override void Attack()                                                                // Attack and hit player
    {
        //base.Attack();

        RaycastHit2D hit = Physics2D.Raycast(rb2D.position + Vector2.up * 0.2f, new Vector2(direction,0), attackRange, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            standStill = 0;
            attackCooldown -= Time.fixedDeltaTime;
            if(attackCooldown < 0)                                                                  // Cooldown for Animation duration
            {
                animator.SetTrigger("Attack");
                attackCooldown = 0.8f;
            }
        }
        else
            standStill = 1;
    }

    void OnTriggerEnter2D(Collider2D other)                                                       // Damage if hit by Bad Guy
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-damage);
            Debug.Log("Collided with " + player.name);
        }
    }

    public override void ChangeHealth(float amount)
    {
        base.ChangeHealth(amount);
        animator.SetTrigger("Hurt");
        Debug.Log(currentHealth + "+" + isDead);
        if(isDead)
        {
            animator.SetTrigger("Death");
            standStill = 0;
            rb2D.simulated = false;
        }
    }

    private void PatrolingArea()                                                                    //Walk from left to right until Ground ends
    {
        if(!isGrounded && rb2D.velocity.y == 0)
        {
            Flip();
        }
        if(!isGrounded)
        {
            movement = new Vector2 (0,-rb2D.gravityScale);
            rb2D.velocity = movement * speed;
            animator.SetInteger("AnimState", 0);
        }
        if(isGrounded)
        {
            movement = new Vector2 (direction,0);
            rb2D.velocity = movement * speed * standStill;
            animator.SetInteger("AnimState", 2);
        }
        if (isWall)
        {
            Flip();
        }
    }

    private void IsGrounded()                                                                       // Check if Enemy is right before cliff
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void IsWall()                                                                           //Check if Enemy is approching a wall
    {
        isWall = Physics2D.OverlapCircle(wallCheck.position, 0.02f, groundLayer);
    }

    private void Flip()                                                                             // flip walking direktion and sprite
    {
        direction = -direction;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}