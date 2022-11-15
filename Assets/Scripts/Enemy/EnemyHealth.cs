using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float hitPoints = 100.0f;

    public void TakeDamage(float damageTaken)
    {
        hitPoints -= damageTaken;
        if (hitPoints <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

}
