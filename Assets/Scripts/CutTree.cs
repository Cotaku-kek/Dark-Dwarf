using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutTree : MonoBehaviour
{

    Animator animator;
    EdgeCollider2D edgeCollider2D;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    public void TreeFalling()
    {
        animator.SetTrigger("Cut");
        edgeCollider2D.enabled = !edgeCollider2D.enabled;
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }
}
