using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayDamage : MonoBehaviour
{
    [SerializeField] private Canvas dmgImpactCanvas;
    [SerializeField] private float impactTime = 0.3f;
    
    // Start is called before the first frame update
    void Start()
    {
        dmgImpactCanvas.enabled = false;
    }

    public void ShowPlayerDamage()
    {
        StartCoroutine(PlayerDamageRoutine());
    }
    
    private IEnumerator PlayerDamageRoutine()
    {
        dmgImpactCanvas.enabled = true;
        yield return new WaitForSeconds(impactTime);
        dmgImpactCanvas.enabled = false;
    }
}
