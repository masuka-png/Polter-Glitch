using UnityEngine;

public class EnemyAmbientAudio : MonoBehaviour
{
    [Header("Ambient Sound")]
    public AudioClip ambientClip;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = ambientClip;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 1f; // 3D so it gets quieter as player moves away
        _audioSource.volume = volume;
        _audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (ambientClip != null)
            _audioSource.Play();
    }

    // Lets you adjust volume live from the Inspector during Play mode
    private void Update()
    {
        _audioSource.volume = volume;
    }
}