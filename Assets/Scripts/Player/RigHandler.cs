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
    
    private RigBuilder _rigBuilder;
    private bool _isReloading;

    private void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
    }

    // float my_value=0f;
    // float min_value=10.0f;
    // float max_value=20.0f;
    private void Update()
    {
        RelaxLeftArm();
        LockLeftArm();
    }

    public void SetReloadBool(bool b)
    {
        _isReloading = b;
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
        
        leftHandIK.weight -= Time.deltaTime / 0.5f;
        if (leftHandIK.weight <= 0.2f)
            SetReloadBool(false);
    }

    private void LockLeftArm()
    {
        // Condition:
        if (_isReloading) return;
        
        leftHandIK.weight += Time.deltaTime / 0.5f;
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
