using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWeapon : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private ParticleSystem muzzleFlash;
    private Animator _weaponsAnimator;
    private static readonly int Fire = Animator.StringToHash("Fire");

    private void Start()
    {
        _weaponsAnimator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        StartCoroutine(WeaponFxsRoutine());
    }

    private IEnumerator WeaponFxsRoutine()
    {
        muzzleFlash.Play();
        _weaponsAnimator.SetTrigger(Fire);
        yield return new WaitForSeconds(0.1f);
        _weaponsAnimator.ResetTrigger(Fire);
    }
}
