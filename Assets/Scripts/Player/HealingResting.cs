using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingResting : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float timeBetweenHealing {get;} = 2;
    private bool isHealing;
    private float healingTimer;

    void Update()
    {
        HealingTimer(timeBetweenHealing);
    }

    void OnTriggerStay2D(Collider2D other)                                                       // Healing if within Healing Trigger
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            if(!isHealing)
            player.ChangeHealth(damage);
            isHealing = true;
            Debug.Log("Healing " + player.name);
        }
    }

    void HealingTimer(float timer)
    {
        if (isHealing)
        {
            healingTimer -= Time.deltaTime;
            if (healingTimer < 0)
            {
                isHealing = false;
                healingTimer = timer;
            }
        }
    }
}
