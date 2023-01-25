using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool pressJump;
    private bool releaseJump;

    public Animator animator;

    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Update()
    {
        pressJump = Input.GetButtonDown("Jump");
        releaseJump = Input.GetButtonUp("Jump");
        horizontal = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(horizontal));                  // Mathf.Abs makes sure, that the horizontal value is always positive
        
        if(pressJump && IsGrounded())                     // If player is on ground and press jump, he jumps
        {
            print("jump");
            pressJump = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpingPower);
            animator.SetBool("isJumping", true);
        }

        if(releaseJump)          // Control of jump height
        {
            print("release");
            releaseJump = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
            animator.SetBool("isJumping", false);           // temporaer
        }

        Flip();
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);
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
