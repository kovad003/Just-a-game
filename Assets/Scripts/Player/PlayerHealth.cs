using System;
using TMPro;
using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [Header("PLAYER: ")] 
    [SerializeField] private float hitPoints = 100.0f;
    
    [Header("UI: ")] 
    [SerializeField] private TextMeshProUGUI ammoText;
    
    /* HIDDEN FIELDS: */
    private PlayerDisplayDamage _playerDisplayDamage;
    private static readonly int Die1 = Animator.StringToHash("Die");

    /* METHODS: */
    private void Start()
    {
        _playerDisplayDamage = GetComponent<PlayerDisplayDamage>();
    }

    private void Update()
    {
        DisplayHealthStat();
    }

    /// Method display player's current health stats on the in-game canvas.
    private void DisplayHealthStat()
    {
        ammoText.text = "Health: " + hitPoints;
    }
    
    /// Method administers the damage taken by the player.
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0.0f)
        {
            hitPoints = 0.0f;
            GetComponent<DeathHandler>().HandleDeath();
        }
        _playerDisplayDamage.ShowPlayerDamage();
    }

    private void Die()
    {
        GetComponent<DeathHandler>().HandleDeath();
    }
}
