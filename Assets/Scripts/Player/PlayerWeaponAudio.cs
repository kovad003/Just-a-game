using UnityEngine;

public class PlayerWeaponAudio : MonoBehaviour
{
    private AudioSource _audioPlayer;

    public AudioClip fireAudioClip; 
    public AudioClip reloadAudioClip;
    public AudioClip emptyClipAudioClip;
    
    private void Start()
    {
        _audioPlayer = GetComponent<AudioSource>();
    }

    /// Method plays shooting sound effect when called. The "one shot" member enables sound overlapping.
    public void PlayFireSfx() { _audioPlayer.PlayOneShot(fireAudioClip); }
    
    /// Method plays reload sound effect when called. The "one shot" member enables sound overlapping.
    public void PlayReloadSfx() { _audioPlayer.PlayOneShot(reloadAudioClip);
    }
    
    public void PlayEmptyClipSfx() { _audioPlayer.PlayOneShot(emptyClipAudioClip); }
}