using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThrowingHook : MonoBehaviour
{

    public GameObject hookSprite;

    GameObject curHook;

    public GameObject currentHitObject;
    private Rigidbody2D rb;
    int layerMask;
    private float overlapRadius = 12f;
    public float pullStrengh = 20;
    public bool ropeActive;


        private PlayerInputActions playerInputActions;


    Color color;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMask = LayerMask.GetMask("Anchor");
        GetPlayerInputActions();
    }

    void GetPlayerInputActions()
    {
        playerInputActions = new PlayerInputActions();                                            // Access to the PlayerInputActions script
        playerInputActions.Player.Enable();
        //playerInputActions.Player.Jump.performed += JumpLogic;
    }

    // Update is called once per frame
    void Update()
    {
        
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, overlapRadius, layerMask);       // Get Collider within Layer "Anchor"

        if (collider.Length >= 1)                                           // If Object in Collider Array, get its infos. 0 is always the nearest Object
        {
            currentHitObject = collider[0].transform.gameObject;
        }
        else                                                                // If not in Range, set to null
        {
            currentHitObject = null;
        }
        GoToAnchor();
    }

    void GoToAnchor()                                                       // Spawn Rope to Anchor
    {
        if (playerInputActions.Player.Rope.WasPressedThisFrame() && currentHitObject != null)
        {
            if(!ropeActive)
            {
                Vector2 destiny = currentHitObject.transform.position;                              //Get Position from Anchor
                curHook = Instantiate(hookSprite,transform.position,Quaternion.identity);           //Create Hook
                curHook.GetComponent<RopeScript>().destiny = destiny;                               //Get Script Component from Rope
                ropeActive = true;                                                                  //Rope is now active
            }
            else
            {
                Destroy(curHook);                                                                   //destroy Rope
                ropeActive = false;
            }

        } 

    }


}







/*
        if(Input.GetButton("Fire1") && currentHitObject != null)                                                        // STRG to "throw Hook" if Object in sight
        {
            if(transform.position.y < currentHitObject.transform.position.y)                                            // no Force if Player higher than Anchor Object
            {
                rb.AddForce(new Vector2(vectorToAnchor.x, vectorToAnchor.y / 5) * pullStrengh, ForceMode2D.Force);      // temp. removed Strengh from y Axis to avoid flying
            }
        }
    }

     Draw Gizmos for Debug
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
    */


