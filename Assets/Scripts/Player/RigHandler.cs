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
    private bool _isReloading;
    private float _reloadDuration;

    private void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
    }

    private void Update()
    {
        AdjustLeftHandIK(_reloadDuration, _isReloading);
    }

    /// This method enables rig layer modifications when player is reloading. This should be
    /// called from the weapon object when player wants to reload.
    /// <param name="isReloading"></param>
    /// <param name="reloadDuration"></param>
    public void EnableReloadAdjustments(bool isReloading, float reloadDuration)
    {
        _isReloading = isReloading;
        _reloadDuration = reloadDuration;
    }
    
    /// Method need to be placed in the Update() section. When player calls for a mag exchange
    /// The EnableReloadAdjustment() method will change the necessary fields so the method will
    /// be executed carrying out the physical reload process via the HandIK (Rig Layers).
    /// <param name="duration"></param>
    /// <param name="isReloading"></param>
    private void AdjustLeftHandIK(float duration, bool isReloading)
    {
        MoveLeftHandToAmmoBelt(duration, isReloading);
        MoveLeftHandBackToWeapon(duration, isReloading);
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
    /// <param name="isReloading"></param>
    private void MoveLeftHandToAmmoBelt(float duration, bool isReloading)
    {
        // Condition:
        if (!isReloading) return;
        
        leftHandIK.weight -= Time.deltaTime / duration;
        if (leftHandIK.weight <= leftArmTurningPoint)
            _isReloading = false;
    }

    /// Method should be executed after the MoveLeftHandToAmmoBelt() function has been called.
    /// It modifies the LeftHandIK's weight setting thus player will move the left hand back to the weapon.
    /// (Looks like a new mag is inserted into the weapon.)
    /// <param name="duration"></param>
    /// <param name="isReloading"></param>
    private void MoveLeftHandBackToWeapon(float duration, bool isReloading)
    {
        // Condition:
        if (isReloading) return;
        
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
}
