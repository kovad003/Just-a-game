
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private AudioClip[] zombieAudioClips;
    // Update is called once per frame

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
        // Object.Destroy((UnityEngine.Object)gameObject, clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
    
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
        // Object.Destroy((UnityEngine.Object)gameObject, clip.length * ((double) Time.timeScale < 0.009999999776482582 ? 0.01f : Time.timeScale));
    }
    
}
