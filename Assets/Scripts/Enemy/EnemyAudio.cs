using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [Header("AI Audio Sources")]
    [Range(0f, 1f)] [SerializeField] private float volume = 1f;
    [SerializeField] private float minDelayIdle = 8;
    [SerializeField] private float maxDelayIdle = 14;
    public AudioClip[] idleAudioClips;
    [SerializeField] private float minDelayChase = 8;
    [SerializeField] private float maxDelayChase = 14;
    public AudioClip[] chaseAudioClips;
    [SerializeField] private float minDelayAttack = 8;
    [SerializeField] private float maxDelayAttack = 14;
    public AudioClip[] attackAudioClips;
    
    [Header("Animation Event Audio")] 
    public AudioClip collapseOnDeath;
    
    [Header("Enemy Health Audio")] 
    public AudioClip deathAudio;
    
    /* HIDDEN FIELDS: */
    private float _audioTimer;
    private Rigidbody _enemyRb;
    
    private void Start()
    {
        _audioTimer = Time.time;
        _enemyRb = GetComponent<Rigidbody>();
    }
    
    ///  Method plays random idle sound effects whe it is called.
    public void PlayIdleSfx()
    {
        if (!(Time.time >= _audioTimer)) return;
        var index = Random.Range(0, idleAudioClips.Length);
        AudioManager.PlayClipAtPoint(idleAudioClips[index], 
            transform.TransformPoint(_enemyRb.centerOfMass), volume, gameObject.transform);
        _audioTimer = Time.time + Random.Range(minDelayIdle, maxDelayIdle);
    }
   
    ///  Method plays random chase sound effects whe it is called.
    public void PlayChaseSfx()
    {
        if (!(Time.time >= _audioTimer)) return;
        var index = Random.Range(0, chaseAudioClips.Length);
        AudioManager.PlayClipAtPoint(chaseAudioClips[index], 
            transform.TransformPoint(_enemyRb.centerOfMass), volume, gameObject.transform);
        _audioTimer = Time.time + Random.Range(minDelayChase, maxDelayChase);
    }
    
    ///  Method plays random attack sound effects whe it is called.
    public void PlayAttackSfx()
    {
        if (!(Time.time >= _audioTimer)) return;
        var index = Random.Range(0, attackAudioClips.Length);
        AudioManager.PlayClipAtPoint(attackAudioClips[index], 
            transform.TransformPoint(_enemyRb.centerOfMass), volume, gameObject.transform);
        _audioTimer = Time.time + Random.Range(minDelayAttack, maxDelayAttack);
    }

    ///  Method plays death sound effects when it is called.
    public void PlayDeathSfx()
    {
        AudioManager.PlayClipAtPoint("AUDIO_Death", deathAudio, 
            transform.TransformPoint(_enemyRb.centerOfMass), volume, gameObject.transform);
    }

    ///  Method plays a "body has been dropped on the floor" sound effects when it is called.
    public void PlayCollapseSfx()
    {
        AudioManager.PlayClipAtPoint("AUDIO_Collapse", collapseOnDeath, 
            transform.TransformPoint(_enemyRb.centerOfMass), volume, gameObject.transform);
    }
}
