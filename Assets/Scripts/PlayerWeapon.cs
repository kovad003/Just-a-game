using System.Collections;
using UnityEngine;

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
    
    private float _timeOfLastShot;
    // private bool _isAiming = true;

    /* Animator Param References - Player Character's Animator!: */
    private static readonly int IsPistolHolstered = Animator.StringToHash("isPistolHolstered");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");

    private void Start()
    {
        // Binding Important Fields:
        _timeOfLastShot = Time.time;
    }

    // Update is enough for scanning user input.
    private void Update()
    {
        Shoot(Input.GetMouseButtonDown(0), 
            playersAnimator.GetBool(IsPistolHolstered), 
            playersAnimator.GetBool(IsAiming));
    }
    
    /**************************************************************************************************************/
    // LMB triggers this method. A timer is checking the elapsed time between shots.
    // Recoil is generated accordingly.
     private void Shoot(bool mouseBtnDown, bool isGunHolstered, bool isAiming)
    {
        // Conditions:
        if (!mouseBtnDown) return;
        if (isGunHolstered) return;
        if (!isAiming) return;
        if (FeedingNextBulletIntoBarrel()) return;
        
        ProcessRaycast();
        PlayMuzzleFlash();
        StartCoroutine(ProcessRecoil());
    }

     private void PlayMuzzleFlash()
     {
         muzzleFlash.Play();
         StartCoroutine(HandleFlicker(muzzleFlash));
     }

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

    private void ProcessRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(weaponBarrel.position, weaponBarrel.forward, out hit, weaponRange))
        {
            Debug.DrawRay(weaponBarrel.position, hit.point, Color.blue); 
            Debug.Log("I hit: " + hit.transform.name);

            CreateHitImpact(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null) //Hitting inert objects (walls) wont throw "Null Ref error".
                target.TakeDamage(damageCaused);
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, 1);
    }

    private bool FeedingNextBulletIntoBarrel()
    {
        var currentTime = Time.time;
        if (_timeOfLastShot + rateOfFire >= currentTime) return true;
        _timeOfLastShot = currentTime; 
        return false; // Next bullet inserted into the barrel -> FIRE!
    }

    private IEnumerator HandleFlicker(ParticleSystem p)
    {
        p.GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(0.1f);
        p.GetComponent<Light>().enabled = false;
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
