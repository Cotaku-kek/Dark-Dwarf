using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThrowingHook : MonoBehaviour
{

    public GameObject currentHitObject;
    private Rigidbody2D rb;
    public Vector2 vectorToAnchor;
    int layerMask;
    private float overlapRadius = 12f;
    public float pullStrengh = 20;

    Color color;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        layerMask = LayerMask.GetMask("Anchor");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, overlapRadius, layerMask);       // Get Collider within Layer "Anchor"

        if (collider.Length >= 1)                                           // If Object in Collider Array, get its infos. 0 is always the nearest Object
        {
            currentHitObject = collider[0].transform.gameObject;
            vectorToAnchor = collider[0].transform.position - transform.position;

            color = Color.green;
        }
        else                                                                // If not in Range, set to null
        {
            color = Color.red;
            currentHitObject = null;
        }

        GoToAnchor();
    }

    void GoToAnchor()                                                       // Pull movement to Anchor
    {
        if(Input.GetButton("Fire1") && currentHitObject != null)                                                        // STRG to "throw Hook" if Object in sight
        {
            if(transform.position.y < currentHitObject.transform.position.y)                                            // no Force if Player higher than Anchor Object
            {
                rb.AddForce(new Vector2(vectorToAnchor.x, vectorToAnchor.y / 5) * pullStrengh, ForceMode2D.Force);      // temp. removed Strengh from y Axis to avoid flying
            }
        }
    }

    /* Draw Gizmos for Debug
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);
    }
    */
}