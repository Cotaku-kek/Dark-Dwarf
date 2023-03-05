using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingFloor : MonoBehaviour
{
    [SerializeField] float damageAmount;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        controller?.ChangeHealth(damageAmount);
    }
}
