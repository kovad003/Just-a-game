using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigHandler : MonoBehaviour
{
    [SerializeField] private GameObject rigLayers;
    [SerializeField] private float enableRigAfterSeconds;
    [SerializeField] private float disableRigAfterSeconds;
    [SerializeField] private Rig aimLayer;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private float leftArmTurningPoint = 0.2f;
    
    private RigBuilder _rigBuilder;

    private void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
    }

    /// Method adjust weapon height and angle upon aiming.
    /// <param name="duration"></param>
    /// <param name="isAiming"></param>
    public void AdjustAimLayer(float duration, bool isAiming)
    {
        if (isAiming)
            aimLayer.weight += Time.deltaTime / duration;
        else
            aimLayer.weight -= Time.deltaTime / duration;
    }
    
    /// Method starts DisableRigLayers() coroutine.
    public void RelaxJoints()
    {
        StartCoroutine(DisableRigLayers(/*0.4f*/));
    }
    
    /// Method starts EnableRigLayers() coroutine.
    public void LockJoints()
    {
        StartCoroutine(EnableRigLayers(/*2.0f*/));
    }

    /// Method modifies the LeftHandIK's weight setting thus player will reach for a new mag
    /// with the left hand. Should be called before the MoveLeftHandBackToWeapon() function.
    /// <param name="duration"></param>
    private void MoveLeftHandToAmmoBelt(float duration)
    {
        leftHandIK.weight -= Time.deltaTime / duration;
    }

    /// Method should be executed after the MoveLeftHandToAmmoBelt() function has been called.
    /// It modifies the LeftHandIK's weight setting thus player will move the left hand back to the weapon.
    /// (Looks like a new mag is inserted into the weapon.)
    /// <param name="duration"></param>
    private void MoveLeftHandBackToWeapon(float duration)
    {
        leftHandIK.weight += Time.deltaTime / duration;
    }
    
    /// Coroutine. The temporary suspension of the Rig Layers enables the proper execution of
    /// player animations. (Equipping gun.)
    private IEnumerator EnableRigLayers()
    {
        yield return new WaitForSeconds(enableRigAfterSeconds);
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = true;
        rigLayers.SetActive(true); // This needs to be here so weapon is only spawned when hands are together!
    }
    
    /// Coroutine. The temporary suspension of the Rig Layers enables the proper execution of
    /// player animations. (Holstering gun.)
    private IEnumerator DisableRigLayers()
    {
        rigLayers.SetActive(false);
        yield return new WaitForSeconds(disableRigAfterSeconds);
        // The named one will be set to true, the rest will be turned off:
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = i.name == "RigLayerBodyAim";
    }

    public IEnumerator ExecuteReloadMovements(float reloadDuration, Action<bool> callback)
    {
        // Before Yield:
        MoveLeftHandToAmmoBelt(reloadDuration);
        // Yield:
        yield return new WaitForSeconds(reloadDuration);
        // Continue:
        MoveLeftHandBackToWeapon(reloadDuration);
        callback(false);
    }
    
}
