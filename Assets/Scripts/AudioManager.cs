
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [SerializeField] private Transform parentTransform;
    [SerializeField] private AudioClip[] zombieAudioClips;

    /* METHODS: */
    private void Start()
    {
        GetComponent<EnemyAI>();
    }

    void Update()
    {
        // CollectAllOneShot();
    }

    /// Method browse all GOs in the scene and collects them under AudioManager.
    private void CollectAllOneShot()
    {
        var audio = FindObjectsOfType<AudioSource>();
        foreach (var a in audio)
        {
            if (a.clip == null) continue;
            Debug.Log(a.clip.name);
            a.transform.SetParent(transform, true);

            foreach (var z in zombieAudioClips)
            {
                if (a.clip.name == z.name)
                {
                    a.transform.SetParent(gameObject.transform.GetChild(0).transform, true);
                }
            }
        }
    }
    
    /// <summary>
    /// This method is a modified version of AudioSource.PlayClipAtPoint(). It instantiates the
    /// audio game object with world space coordinates and collects it under the given parent object. 
    /// </summary>
    /// <param name="clip">Audio clip ypu want to play.</param>
    /// <param name="position">You can customize the world space coordinates.</param>
    /// <param name="volume">You can customize the volume.</param>
    /// <param name="parent">Audio carrier game object will be put inside this object.</param>
    public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, Transform parent)
    {
        GameObject gameObject = new GameObject("AUDIO_Cancel on death");
        gameObject.transform.position = position;
        gameObject.transform.SetParent(parent.transform, true);
        AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy((Object) gameObject,
            clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
    
    /// <summary>
    /// This method is a modified version of AudioSource.PlayClipAtPoint(). It instantiates the
    /// audio game object with world space coordinates and collects it under the given parent object.
    /// In this version you can select a name for the audio object so later you can easily find it. :)
    /// </summary>
    /// <param name="name">You can set a name for the audio object.</param>
    /// <param name="clip">Audio clip ypu want to play.</param>
    /// <param name="position">You can customize the world space coordinates.</param>
    /// <param name="volume">You can customize the volume.</param>
    /// <param name="parent">Audio carrier game object will be put inside this object.</param>
    public static void PlayClipAtPoint(string name, AudioClip clip, Vector3 position, float volume, Transform parent)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.position = position;
        gameObject.transform.SetParent(parent.transform, true);
        AudioSource audioSource = (AudioSource) gameObject.AddComponent(typeof (AudioSource));
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f;
        audioSource.volume = volume;
        audioSource.Play();
        Object.Destroy((Object) gameObject, 
            clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
}
