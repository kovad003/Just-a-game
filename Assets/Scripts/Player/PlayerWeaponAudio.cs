using UnityEngine;

public class PlayerWeaponAudio : MonoBehaviour
{
    /* EXPOSED FIELDS */
    [Header("AUDIO CLIPS: ")]
    [Tooltip("Drag audio clips here.")]
    public AudioClip fireAudioClip; 
    public AudioClip reloadAudioClip;
    public AudioClip emptyClipAudioClip;
    
    /* HIDDEN FIELDS */
    private AudioSource _audioPlayer;
    
    private void Start()
    {
        _audioPlayer = GetComponent<AudioSource>();
    }

    /// Method plays shooting sound effect when called. The "one shot" member enables sound overlapping.
    public void PlayFireSfx() { _audioPlayer.PlayOneShot(fireAudioClip); }
    
    /// Method plays reload sound effect when called. The "one shot" member enables sound overlapping.
    public void PlayReloadSfx() { _audioPlayer.PlayOneShot(reloadAudioClip);
    }
    
    /// Method plays a clicking sound effect when called. The "one shot" member enables sound overlapping.
    public void PlayEmptyClipSfx() { _audioPlayer.PlayOneShot(emptyClipAudioClip); }
}