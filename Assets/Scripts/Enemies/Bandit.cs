using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy
{

    Rigidbody2D rb2D;
    Animator animator;

    private float direction = -1;
    private float standStill = 1;

    private float attackCooldown;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool isGrounded = true;

    // Start is called before the first frame update
    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        IsGrounded();
        PatrolingArea();
        Attack();
    }

    protected override void Attack()                                                                // Attack and hit player
    {
        //base.Attack();

        RaycastHit2D hit = Physics2D.Raycast(rb2D.position + Vector2.up * 0.2f, new Vector2(direction,0), 1f, LayerMask.GetMask("Player"));
        if (hit.collider != null)
        {
            standStill = 0;
            attackCooldown -= Time.fixedDeltaTime;
            if(attackCooldown < 0)                                                                  // Cooldown for Animation duration
            {
                animator.SetTrigger("Attack");
                PlayerController player = hit.collider.GetComponent<PlayerController>();
                player.ChangeHealth((int)-damage * 2);
                attackCooldown = 0.8f;
            }
        }
        else
            standStill = 1;
    }

    void OnCollisionStay2D(Collision2D other)                                                       // Damage if touch Bad Guy
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth((int)-damage);
            Debug.Log("Collided with" + player.name);
        }
    }

    private void PatrolingArea()                                                                    //Walk from left to right until Ground ends
    {
        Vector2 position = rb2D.position;
        position.x += Time.deltaTime * speed * direction * standStill;
        if(!isGrounded)
        {
            Flip();
        }

        rb2D.MovePosition(position);
        animator.SetInteger("AnimState", 2);
    }

    private void IsGrounded()                                                                       // Check if Enemy is right before cliff
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()                                                                             // flip walking direktion and sprite
    {
        direction = -direction;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}