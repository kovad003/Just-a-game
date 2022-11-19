using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.PlayerLoop;

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
        AdjustLeftHandIK();
    }

    public void EnableReloadAdjustments(bool b, float reloadDuration)
    {
        _isReloading = b;
        _reloadDuration = reloadDuration;
    }

    private void AdjustLeftHandIK()
    {
        RelaxLeftArm();
        LockLeftArm();
    }
    
    public void AdjustAimLayer(float duration, bool isAiming)
    {
        if (isAiming)
            aimLayer.weight += Time.deltaTime / duration;
        else
            aimLayer.weight -= Time.deltaTime / duration;
    }
    
    public void RelaxJoints()
    {
        StartCoroutine(DisableRigLayers(/*0.4f*/));
    }

    public void LockJoints()
    {
        StartCoroutine(EnableRigLayers(/*2.0f*/));
    }

    private void RelaxLeftArm()
    {
        // Condition:
        if (!_isReloading) return;
        
        leftHandIK.weight -= Time.deltaTime / _reloadDuration;
        if (leftHandIK.weight <= leftArmTurningPoint)
            _isReloading = false;
    }

    private void LockLeftArm()
    {
        // Condition:
        if (_isReloading) return;
        
        leftHandIK.weight += Time.deltaTime / _reloadDuration;
    }
    
    // The delay makes the transition better between animation states!
    // Events weren't working really well bc of the rig layers!
    private IEnumerator EnableRigLayers()
    {
        yield return new WaitForSeconds(enableRigAfterSeconds);
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = true;
        rigLayers.SetActive(true); // This needs to be here so weapon is only spawned when hands are together!
    }
    private IEnumerator DisableRigLayers()
    {
        rigLayers.SetActive(false);
        yield return new WaitForSeconds(disableRigAfterSeconds);
        // The named one will be set to true, the rest will be turned off:
        foreach (RigLayer i in _rigBuilder.layers)
            i.active = i.name == "RigLayerBodyAim";
    }
}
