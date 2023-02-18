using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LadderMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;                                // rb = rigidbody

    private Vector2 vertical;                                                 // Player moves vertically
    private float speed = 8f;                                               // Climbing speed
    private bool isLadder;                                                  // Check if player stands next to a ladder
    private bool isClimbing;                                                // Check if player is climbing
    private PlayerInputActions playerInputActions;                          // Access to the PlayerInputActions Script
    private Rigidbody2D rb2D;
    private float currentGravity;

    void Awake()
    {
        GetPlayerInputActions();
        rb2D = GetComponent<Rigidbody2D>();
        currentGravity = rb2D.gravityScale;
    }


    void Update()                                                           // Update is called once per frame
    {
        vertical = playerInputActions.Player.Ladder.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(isLadder && vertical.y != 0)                                     // If player enters ladder and the vertical input is not than 0, then climbing is true
        {
            isClimbing = true;
            rb2D.gravityScale = 0;                                          // No Gravity for authentic Movement
            LadderClimbing();
        }
        else if (!isLadder)
        {
            isClimbing = false;
            rb.gravityScale = currentGravity;                               // If player is leaving ladderCollision, gravity is set to normal value
        }
    }

    void GetPlayerInputActions()                                            // Access to the PlayerInputActions script
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public void LadderClimbing()                                            // if fixed to the Ladder Move up and Down
    {
        Vector2 position = rb2D.position;
        position.y += speed * vertical.y * Time.deltaTime;
        rb2D.MovePosition(position);
    }

    private void OnTriggerStay2D(Collider2D collision)                      // Method from unity, if player stays over ladder collider
    {
        if(collision.CompareTag("Ladder"))                                  // "Ladder"-Tag is set in the inspector
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)                      // Method from unity, player exits ladder
    {
        if(collision.CompareTag("Ladder"))
        {
            isLadder = false;
        }
    }
}