using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingAxe : MonoBehaviour
{
    Rigidbody2D rb2D;
    GameObject player;
    [SerializeField] float throwingAxePower;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        throwingAxePower = player.throwingAxePower;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.magnitude > 500.0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void Throw(Vector2 direction, float force)
    {
        float axeRotation = 360;
        if (direction.x < 0)
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
            rb2D.AddForce(direction * force);
            rb2D.AddTorque(axeRotation, ForceMode2D.Force);         
        }
        else
        {
            rb2D.AddForce(direction * force);
            rb2D.AddTorque(-axeRotation, ForceMode2D.Force);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Enemy e = other.collider.GetComponent<Enemy>();
            e?.ChangeHealth(-throwingAxePower);
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy e = other.gameObject.GetComponent<Enemy>();
            e?.ChangeHealth(-throwingAxePower);
        gameObject.SetActive(false);
    }
}
