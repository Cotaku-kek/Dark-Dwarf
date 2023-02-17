using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead = false;

    [SerializeField] protected float attackRange;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Attack()
    {
        Debug.Log("Enemy class: Attack method called");
    }

    public virtual void ChangeHealth(float amount)
    {
        currentHealth = currentHealth + amount;
        OnDeath();
    }

    protected virtual void OnDeath()
    {
        if(currentHealth <= 0)
        {
            isDead = true;
        }
        else
            isDead = false;
    }
}