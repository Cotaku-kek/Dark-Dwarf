using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlattform : MonoBehaviour
{

    Animator animator;
    
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();    
    }

    void OnCollisionEnter2D()
    {
        animator.SetTrigger("triggered");
    }
}
