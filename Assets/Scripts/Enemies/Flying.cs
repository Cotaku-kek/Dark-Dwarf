using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : Enemy
{
    private Rigidbody2D rb2D;

    private float direction = 1;
    private float directionTimer;
    [SerializeField] private float directionTime;
    Vector2 movement;

    protected override void Awake()
    {
        base.Awake();
        rb2D = GetComponent<Rigidbody2D>();
        directionTimer = directionTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        FlyingMovement();
    }

    private void FlyingMovement()
    {
        directionTimer -= Time.fixedDeltaTime;
        if(directionTimer < 0)
        {
            Flip();
            directionTimer = directionTime;
        }
        movement = new Vector2(direction,0);
        rb2D.velocity = movement * speed;
    }

    void OnTriggerEnter2D(Collider2D other)                                                       // Damage if hit by Bad Guy
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-damage);
            Debug.Log("Collided with " + player.name);
        }
    }

    public override void ChangeHealth(float amount)
    {
        base.ChangeHealth(amount);
        Debug.Log(currentHealth + "+" + isDead);
        if(isDead)
        {
            rb2D.simulated = false;
        }
    }

    private void Flip()                                                                             // flip walking direktion and sprite
    {
        direction = -direction;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    /* Angriff
    Wenn Player in Sichtweite
    speichere Standort und fliege auf Player zu
        !berechnung der direction pausieren
    Wenn Angriff beendet 
        1. Player erneut angreifen wenn in Reichweite, eventuell "anlauf nehmen", Cooldown
        2. zur gespeicherten Position zurückfliegen
    Patrollieren wiederaufnehmen
    */
    
    /* Sichtweite
    eventuell raytrace oder Physics2D.OverlapCircleAll wie im Wurfhaken-Skript
        
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, overlapRadius, layerMask);
        if (collider.Length >= 1)                                           // If Object in Collider Array, get its infos. 0 is always the nearest Object
        {
            currentHitObject = collider[0].transform.gameObject;
        }

    */
}
