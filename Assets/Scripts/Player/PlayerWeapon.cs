using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    /* EXPOSED FIELDS */
    [Header("GENERAL: ")]
    [SerializeField] [Range(0.1f, 1.0f)] private float rateOfFire = 0.3f;
    [SerializeField] private float weaponRange = 200.0f;
    [SerializeField] [Range(8f, 16f)] private float recoilModifier = 8f;
    [SerializeField] private float damageCaused = 10f;
    [SerializeField] private float reloadDuration = 0.6f;
    [SerializeField] private Ammo ammoSlot;
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private Magazine magazine;

    [Header("EFFECTS: ")]
    [SerializeField] private Transform weaponBarrel;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    
    [Header("PLAYER: ")]
    [Tooltip("Drag the Player Object here.")]
    [SerializeField] private Animator playersAnimator;
    [Tooltip("Should be under MainCam")]
    [SerializeField] private Transform aimingRef;

    [Header("UI: ")] 
    [SerializeField] private TextMeshProUGUI ammoText;
    
    /* HIDDEN FIELDS */
    private Animator _weaponsAnimator;
    private PlayerWeaponAudio _playerWeaponAudio;
    private RigHandler _rigHandler;
    private float _timeOfLastShot;
    private bool _isBeingReloaded;

    // Animator Hash (Player):
    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");
    // Animator Hash (Weapon):
    private static readonly int Fire = Animator.StringToHash("Fire");

    /* INNER CLASS */
    [Serializable] private class Magazine
    {
        public int magSize = 12;
        public int ammoAmountInMag = 0;
    }
    
    private void Start()
    {
        // Binding Important Fields:
        _timeOfLastShot = Time.time;
        _rigHandler = FindObjectOfType<RigHandler>();
        _playerWeaponAudio = GetComponent<PlayerWeaponAudio>();
        _weaponsAnimator = GetComponent<Animator>();
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        DisplayAmmo();
        ReloadWeapon(KeyCode.R);
        Shoot(Input.GetMouseButtonDown(0), 
            playersAnimator.GetBool(IsPistolHolstered), 
            playersAnimator.GetBool(IsAiming));
    }

    /**************************************************************************************************************/
    /* METHODS:  */
    /// Method display available ammo amount on the in-game canvas.
    private void DisplayAmmo()
    {
        int totalAmmo = ammoSlot.GetTotalAmmo(ammoType);
        int magAmmo = magazine.ammoAmountInMag;
        ammoText.text = magAmmo + " / " + totalAmmo;
    }
    
    /// This method contains multiple parts. Relies on user input so needs to be placed in the Update() method.
    /// By executing it, player will eject current magazine from the weapon, then takes ammo from the
    /// "ammo pouch" (ammo slot). As a final step a new mag is inserted in to the weapon.
    private void ReloadWeapon(KeyCode key)
    {
        // Conditions:
        if (!Input.GetKeyUp(key)) return;
        if (_isBeingReloaded) return; // While reloading another process cannot be initiated!
        _playerWeaponAudio.PlayReloadSfx();
        ChangeMag();
        FetchAmmo();
    }

    /// This method fills up the new mag with ammunition fetched from the "ammo pouch" (ammunition slot).
    private void FetchAmmo()
    {
        var totalAmmo = ammoSlot.GetTotalAmmo(ammoType);
        while ((magazine.ammoAmountInMag < totalAmmo) 
               && magazine.ammoAmountInMag < magazine.magSize 
               && totalAmmo > 0)
        {
            ammoSlot.ReduceTotalAmmo(ammoType);
            magazine.ammoAmountInMag++;
        }
    }
    
    /// This method starts a coroutine that processes the weapon magazine replacement. It drops
    /// the current magazine from  the weapon. The ammunition stored in it will be disposed as well.
    private void ChangeMag()
    {
        magazine.ammoAmountInMag = 0;
        StartCoroutine(MagExchangeRoutine());
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
            StartCoroutine(ProcessFxsRoutine());
            _playerWeaponAudio.PlayFireSfx();
            magazine.ammoAmountInMag--;
            StartCoroutine(RecoilRoutine());
        }
        else
            _playerWeaponAudio.PlayEmptyClipSfx();
    }

     /// Method Generates muzzle flash and plays the weapon slide animation after each shot.
     private IEnumerator ProcessFxsRoutine()
     {
         muzzleFlash.Play();
         _weaponsAnimator.SetTrigger(Fire);
         yield return new WaitForSeconds(0.1f);
         _weaponsAnimator.ResetTrigger(Fire);
     }

     /// Method generates recoil after each shot. A coroutine is used to generate upward and downward amplitude.
     private IEnumerator RecoilRoutine()
    {
        // Before Yield:
        var aimAt = aimingRef.transform;
        aimAt.position = RecoilUpward(aimAt.position, recoilModifier * Time.fixedDeltaTime);
        // Yield:
        yield return new WaitForSeconds(0.1f);
        // Continue:
        aimAt.position = RecoilDownward(aimAt.position, recoilModifier * Time.fixedDeltaTime);
    }

     
     ///<summary> This method is a coroutine. <br/><br/> 1) The first part will utilize the
     /// RigHandler class to modify the Rig Layers, so the mag reload process can actually take
     /// place in the world space. The "reloading flag" is turned to true, so other methods relying
     /// on it can be executed / blocked accordingly. <br/><br/> 2) The second part will suspend
     /// the method till the reload process is finished. When all is done the flag is turned back
     /// to false, so a new reload process can take place. </summary>
     private IEnumerator MagExchangeRoutine()
     {
         // Before Yield:
         _isBeingReloaded = true; // Prevents reload overlapping.
         if (ammoSlot.GetTotalAmmo(ammoType) > 0)
             _rigHandler.EnableLeftHandIKUpdate(_isBeingReloaded, reloadDuration);
         // Yield:
         // need 2x multiplier -> back and forth movement of arm
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
