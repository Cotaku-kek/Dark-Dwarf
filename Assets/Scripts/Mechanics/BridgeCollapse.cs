using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCollapse : MonoBehaviour
{
    public Rigidbody2D rg2D;
    public RelativeJoint2D relativeJoint2D;
    private FixedJoint2D fixedJoint2D;
    
    // Start is called before the first frame update
    void Awake()
    {
        relativeJoint2D = GetComponent<RelativeJoint2D>();
        fixedJoint2D = GetComponent<FixedJoint2D>();
    }

    void OnTriggerEnter2D()
    {
        // Ger�usch, dann kurzer Timer hier einf�gen.
        if (relativeJoint2D != null)
        {
            relativeJoint2D.breakForce = 0;
            fixedJoint2D.enabled = false;
        }   
    }
}