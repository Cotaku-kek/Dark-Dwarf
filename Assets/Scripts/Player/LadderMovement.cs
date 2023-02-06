using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LadderMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;                                // rb = rigidbody

    private float vertical;                                                 // Player moves vertically
    private float speed = 8f;                                               // Climbing speed
    private bool isLadder;                                                  // Check if player stands next to a ladder
    //private bool isClimbing;                                                // Check if player is climbing
    private PlayerInputActions playerInputActions;                          // Access to the PlayerInputActions Script

    void Awake()
    {
        GetPlayerInputActions();
    }


    void Update()                                                           // Update is called once per frame
    {
        vertical = Input.GetAxis("Vertical");
/*
        if(isLadder && Mathf.Abs(vertical) > 0f)                            // If player enters ladder and the vertical input is greater than 0, then climbing is true
        {
            isClimbing = true;
        }
        else
        {
            rb.gravityScale = 4f;                                           // If player is not climbing, gravity is set to normal value
        }
*/
    }

    private void FixedUpdate()
    {
/*        
        if(isClimbing)
        {
            rb.gravityScale = 0f;                                           // If player is climbing, there is no gravity
            rb.velocity = new Vector2(rb.velocity.x, vertical*speed);
        }
*/
    }

    void GetPlayerInputActions()
    {
        playerInputActions = new PlayerInputActions();                     // Access to the PlayerInputActions script
        playerInputActions.Player.Enable();
        playerInputActions.Player.Ladder.performed += LadderClimbing;
    }

    public void LadderClimbing(InputAction.CallbackContext context)
    {
        if(context.performed && isLadder == true)
        {
            Vector2 inputVector = playerInputActions.Player.Ladder.ReadValue<Vector2>();
            //rb.AddForce(new Vector3(rb.inputVector.x, 0, vertical*speed), ForceMode2D.Force);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)                     // Method from unity, player enters ladder
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
            //isClimbing = false;
        }
    }


}
