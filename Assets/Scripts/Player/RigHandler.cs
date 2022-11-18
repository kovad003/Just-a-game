using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigHandler : MonoBehaviour
{
    [SerializeField] private GameObject rigLayers;
    [SerializeField] private float enableRigAfterSeconds;
    [SerializeField] private float disableRigAfterSeconds;
    [SerializeField] private Rig aimLayer;
    
    private RigBuilder _rigBuilder;

    private void Start()
    {
        _rigBuilder = GetComponent<RigBuilder>();
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

    public void CloseJoints()
    {
        StartCoroutine(EnableRigLayers(/*2.0f*/));
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
