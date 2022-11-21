using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigHandler : MonoBehaviour
{
    /* EXPOSED FIELDS */
    [Header("PLAYER: ")]
    [Tooltip("A collective folder for rig layers on the Player object.")]
    [SerializeField] private GameObject rigLayers;
    [SerializeField] private Rig aimLayer;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    
    [Header("HOLSTERING GUN: ")]
    [SerializeField] [Range(0.1f, 4.0f)] private float enableRigAfterSeconds = 2;
    [SerializeField] [Range(0.1f, 2.0f)]private float disableRigAfterSeconds = 0.4f;
    
    [Header("RELOADING GUN: ")]
    [SerializeField] [Range(0.1f, 1.0f)]private float reloadArmTurningPoint = 0.2f;
    
    /* HIDDEN FIELDS */
    private RigBuilder _rigBuilder;
    private bool _isReloading;
    private float _reloadDuration;

    private void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
    }

    private void Update()
    {
        UpdateLeftHandIK(_reloadDuration, _isReloading);
        // UpdateAimLayer();
    }

    /// This method enables rig layer modifications when player is reloading. This should be
    /// called from the weapon object when player wants to reload.
    /// <param name="isReloading"></param>
    /// <param name="reloadDuration"></param>
    public void EnableLeftHandIKUpdate(bool isReloading, float reloadDuration)
    {
        _isReloading = isReloading;
        _reloadDuration = reloadDuration;
    }
    
    /// Method is called locally in the Update() section. When player calls for a mag exchange
    /// The EnableReloadAdjustment() method will enable the execution of this method so the physical
    /// reload process via the HandIK (Rig Layers) will be carried out.
    /// <param name="duration"></param>
    /// <param name="isReloading"></param>
    private void UpdateLeftHandIK(float duration, bool isReloading)
    {
        if (isReloading)
        {
            leftHandIK.weight -= Time.deltaTime / duration;
            if (leftHandIK.weight <= reloadArmTurningPoint)
                _isReloading = false;
        }
        else
            leftHandIK.weight += Time.deltaTime / duration;
    }
    
    /// Method is called from the Update() section of the Aiming class. It adjusts the weapon's
    /// height and angle upon aiming.
    /// <param name="duration"></param>
    /// <param name="isAiming"></param>
    public void UpdateAimLayer(float duration, bool isAiming)
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
    
    /// Coroutine. The temporary suspension of the Rig Layers enables the proper execution of
    /// player animations. (Equipping gun)
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
