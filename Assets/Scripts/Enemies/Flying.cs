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

    private float overlapRadius = 3;
    [SerializeField] private GameObject currentHitObject;
    private Vector2 lookDirection;

    private Vector3 idlePosition;
    private float savedDirectionTimer;
    [SerializeField] private bool isPositionSaved;

    protected override void Awake()
    {
        base.Awake();
        rb2D = GetComponent<Rigidbody2D>();
        directionTimer = directionTime;
    }

    // Update is called once per frame
    void Update()
    {
        Scanning();
    }

    void FixedUpdate()
    {
        if (!isDead)
            Behaviour();
        ConditionalFlip();
    }

    private void Scanning()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, overlapRadius, LayerMask.GetMask("Player"));
        if (collider != null)
        {
            currentHitObject = collider.transform.gameObject;
        }
        else currentHitObject = null;
    }

    private void Behaviour()
    {
        // Patrollierendes Hin- und Herfliegen
        if(currentHitObject == null && !isPositionSaved)
        {
            directionTimer -= Time.fixedDeltaTime;
            if(directionTimer < 0)
            {
                direction = -direction;
                directionTimer = directionTime;
            }
            Movement(new Vector2(direction,0));
        }

        // Idle Position speichern und auf Spieler zufliegen
        if (currentHitObject != null)
        {
            if(!isPositionSaved)
            {
                idlePosition = transform.position;
                savedDirectionTimer = directionTimer;
                isPositionSaved = true;
            }
            else if(isPositionSaved)
            {
                lookDirection = ((currentHitObject.transform.position + currentHitObject.transform.up) - transform.position).normalized;
                Movement(lookDirection);
            }
        }

        // Player verschwindet aus Sichtweite, fliege zurück zu Ursprungsposition
        if (currentHitObject == null && isPositionSaved)
        {
            lookDirection = (idlePosition - transform.position).normalized;
            Movement(lookDirection);
            if (Vector2.Distance(transform.position, idlePosition) < 0.5f)                  //Mathf.Approximately(transform.position.x, idlePosition.x) || Mathf.Approximately(transform.position.y, idlePosition.y)
            {
                isPositionSaved = false;
                directionTimer = savedDirectionTimer;
            }
        }
    }

    private void Movement(Vector2 movement)
    {
        rb2D.velocity = movement * speed;
    }

    void OnTriggerStay2D(Collider2D other)                                                       // Damage if hit by Bad Guy
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
            rb2D.gravityScale = 3;
        }
    }
    
    private void ConditionalFlip()
    {
        if (rb2D.velocity.x > 0)
        {
            Vector2 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
        }
        if (rb2D.velocity.x < 0)
        {
            Vector2 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
        }
    }
}