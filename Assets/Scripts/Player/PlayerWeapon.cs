using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float rateOfFire = 0.1f;
    [SerializeField] private float weaponRange = 100.0f;
    [SerializeField] private Transform weaponBarrel;
    [SerializeField] private float damageCaused;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Animator playersAnimator;
    [SerializeField] private Transform aimingRef;
    [SerializeField] private Ammo ammoSlot;
    [SerializeField] private AmmoType ammoType;

    private float _timeOfLastShot;
    private bool _canShoot = true;
    
    /* Animator Param References - Player Character's Animator!: */
    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");

    [SerializeField] private Magazine magazine;
    [Serializable]
    private class Magazine
    {
        public int magSize = 12;
        public int ammoAmountInMag = 0;
    }
    
    private void Start()
    {
        // Binding Important Fields:
        _timeOfLastShot = Time.time;
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        ReloadMagazine(KeyCode.R);
        Shoot(Input.GetMouseButtonDown(0), 
            playersAnimator.GetBool(IsPistolHolstered), 
            playersAnimator.GetBool(IsAiming));
    }
    
    /**************************************************************************************************************/
    private void OnEnable()
    {
        _canShoot = true;
    }

    private void ReloadMagazine(KeyCode key)
    {
        // Condition:
        if (!Input.GetKeyUp(key)) return;

        var totalAmmo  = ammoSlot.GetTotalAmmo(ammoType);
        while ((magazine.ammoAmountInMag < totalAmmo) && totalAmmo > 0 && magazine.ammoAmountInMag < magazine.magSize)
        {
            ammoSlot.ReduceTotalAmmo(ammoType);
            magazine.ammoAmountInMag++;
        }
        Debug.Log("Ammo am. in mag = " + magazine.ammoAmountInMag);
    }

    // LMB triggers this method. A timer is checking the elapsed time between shots.
    // Recoil is generated accordingly.
     private void Shoot(bool mouseBtnDown, bool isGunHolstered, bool isAiming)
    {
        // Conditions:
        if (!mouseBtnDown) return;
        if (isGunHolstered) return;
        if (!isAiming) return;
        if (FeedingNextBulletIntoBarrel()) return;

        if (magazine.ammoAmountInMag > 0)
        {
            ProcessBulletHit();
            PlayMuzzleFlash();
            magazine.ammoAmountInMag--;
            StartCoroutine(ProcessRecoil());
        }
    }

     // Method Generates muzzle flash after each shot.
     private void PlayMuzzleFlash()
     {
         muzzleFlash.Play();
     }

     // Method generates recoil after each shot. A coroutine is used to generate upward and downward amplitude.
     private IEnumerator ProcessRecoil()
    {
        // Before Yield:
        var aimAt = aimingRef.transform;
        aimAt.position = RecoilUpward(aimAt.position, 8f * Time.fixedDeltaTime);
        // Yield:
        yield return new WaitForSeconds(0.1f);
        // Continue:
        aimAt.position = RecoilDownward(aimAt.position, 8f * Time.fixedDeltaTime);
    }

    // Method is using a raycast to determine what has been hit.
     private void ProcessBulletHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(weaponBarrel.position, weaponBarrel.forward, out hit, weaponRange))
        {
            CreateHitImpact(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null) //Hitting inert objects (walls) wont throw "Null Ref error".
                target.TakeDamage(damageCaused);
                
        }
    }

     // Method generates visual effects at the spot of impact.
    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 1);
    }

    // Method regulates how fast the player can shoot with the equipped weapon. 
    private bool FeedingNextBulletIntoBarrel()
    {
        var currentTime = Time.time;
        if (_timeOfLastShot + rateOfFire >= currentTime) return true;
        _timeOfLastShot = currentTime; 
        return false; // Next bullet inserted into the barrel -> FIRE!
    }

    // Methods are used to generate weapon recoil when player is shooting.
    // They can be kept static.
    private static Vector3 RecoilUpward(Vector3 vector, float y)
    {
        vector.y += y;
        return vector;
    }
    private static Vector3 RecoilDownward(Vector3 vector, float y)
    {
        vector.y -= y;
        return vector;
    }
}
