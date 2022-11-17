using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class AmmoPickUp : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 5;
    [SerializeField] private AmmoType _ammoType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<Ammo>().IncreaseCurrentAmmo(_ammoType, ammoAmount);
            Destroy(gameObject);
        }
    }
}
