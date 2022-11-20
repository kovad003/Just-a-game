using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    public AudioSource audioPlayer;
    public AudioClip fireAudioClip; 
    public AudioClip reloadAudioClip; 

    [SerializeField] private float rateOfFire = 0.3f;
    [SerializeField] private float weaponRange = 200.0f;
    [SerializeField] private Transform weaponBarrel;
    [SerializeField] private float damageCaused;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Animator playersAnimator;
    [SerializeField] private Transform aimingRef;
    [SerializeField] private Ammo ammoSlot;
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private float reloadDuration = 0.6f;

    private RigHandler _rigHandler;
    private float _timeOfLastShot;
    private bool _isBeingReloaded;

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
        _rigHandler = FindObjectOfType<RigHandler>();

        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        ReloadWeapon(KeyCode.R);
        Shoot(Input.GetMouseButtonDown(0), 
            playersAnimator.GetBool(IsPistolHolstered), 
            playersAnimator.GetBool(IsAiming));
    }
    

    /// Method plays shooting sound effect when called. The "one shot" member enables sound overlapping.
    private void PlayFireSfx()
    {
        audioPlayer.PlayOneShot(fireAudioClip);
    }

    /// Method plays reload sound effect when called. The "one shot" member enables sound overlapping.
    private void PlayReloadSfx()
    {
        audioPlayer.PlayOneShot(reloadAudioClip);
    }
    
    /**************************************************************************************************************/
    /// This method contains multiple parts. Relies on user input so needs to be placed in the Update() method.
    /// By executing it, player will eject current magazine from the weapon, then takes ammo from the
    /// "ammo pouch" (ammo slot). As a final step a new mag is inserted in to the weapon.
    private void ReloadWeapon(KeyCode key)
    {
        // Conditions:
        if (!Input.GetKeyUp(key)) return;
        if (_isBeingReloaded) return; // While reloading another process cannot be initiated!
        PlayReloadSfx();
        EjectCurrentMag();
        FetchAmmo();
        InsertNewMag();
    }

    
    /// This method is an integral part of the ReloadWeapon() function. It drops the current magazine from
    /// the weapon. The ammunition stored in it will be disposed as well. 
    private void EjectCurrentMag()
    {
        magazine.ammoAmountInMag = 0;
    }
    
    /// This method fills up the new mag with ammunition fetched from the "ammo pouch" (ammunition slot).
    private void FetchAmmo()
    {
        var totalAmmo = ammoSlot.GetTotalAmmo(ammoType);
        while ((magazine.ammoAmountInMag < totalAmmo) && totalAmmo > 0 && magazine.ammoAmountInMag < magazine.magSize)
        {
            ammoSlot.ReduceTotalAmmo(ammoType);
            magazine.ammoAmountInMag++;
        }
    }
    
    /// This method starts a coroutine that processes the weapon magazine replacement.
    private void InsertNewMag()
    {
        StartCoroutine(ProcessMagReplacement());
    }

    /// LMB triggers this method. A timer is checking the elapsed time between shots.
    /// Recoil is generated accordingly.
     private void Shoot(bool mouseBtnDown, bool isGunHolstered, bool isAiming)
    {
        // Conditions:
        if (!mouseBtnDown) return;
        if (isGunHolstered) return;
        if (!isAiming) return;
        if (FeedingNextBulletIntoBarrel()) return;
        if (_isBeingReloaded) return;

        if (magazine.ammoAmountInMag > 0)
        {
            ProcessBulletHit();
            PlayMuzzleFlash();
            PlayFireSfx();
            magazine.ammoAmountInMag--;
            StartCoroutine(ProcessRecoil());
        }
    }

     /// Method Generates muzzle flash after each shot.
     private void PlayMuzzleFlash()
     {
         muzzleFlash.Play();
     }

     /// Method generates recoil after each shot. A coroutine is used to generate upward and downward amplitude.
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

     
     ///<summary> This method is a coroutine. <br/><br/> 1) The first part will utilize the
     /// RigHandler class to modify the Rig Layers, so the mag reload process can actually take
     /// place in the world space. The "reloading flag" is turned to true, so other methods relying
     /// on it can be executed / blocked accordingly. <br/><br/> 2) The second part will suspend
     /// the method till the reload process is finished. When all is done the flag is turned back
     /// to false, so a new reload process can take place. </summary>
     private IEnumerator ProcessMagReplacement()
     {
         // Before Yield:
         _isBeingReloaded = true;
         if (ammoSlot.GetTotalAmmo(ammoType) > 0)
             _rigHandler.EnableLeftHandIKUpdate(_isBeingReloaded, reloadDuration);
         // Yield:
         // need 2x multiplier bc
         yield return new WaitForSeconds(2*reloadDuration);
         // Continue:
         _isBeingReloaded = false;
     }
     
    /// Method is using a raycast to determine what has been hit. The acquired info is stored in an out param.
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

     /// Method generates visual effects at the spot of impact. It need to be uses inside of the
     /// ProcessBulletHit() method.
    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 1);
    }

    /// Method regulates how fast the player can shoot with the equipped weapon. It imitates an actual
    /// bullet insertion mechanism.
    private bool FeedingNextBulletIntoBarrel()
    {
        var currentTime = Time.time;
        if (_timeOfLastShot + rateOfFire >= currentTime) return true;
        _timeOfLastShot = currentTime; 
        return false; // Next bullet inserted into the barrel -> FIRE!
    }

    /// Method generates upward weapon recoil when player is shooting.
    private static Vector3 RecoilUpward(Vector3 vector, float y)
    {
        vector.y += y;
        return vector;
    }
    /// Method generates downward weapon recoil when player is shooting.
    private static Vector3 RecoilDownward(Vector3 vector, float y)
    {
        vector.y -= y;
        return vector;
    }
}
