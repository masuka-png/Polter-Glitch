using UnityEngine;
using EasyPeasyFirstPersonController;

public class FootstepAudio : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip[] walkClips;
    public AudioClip[] sneakClips;
    public AudioClip[] runClips;

    [Header("Step Intervals (seconds between steps)")]
    public float walkInterval = 0.5f;
    public float sneakInterval = 0.75f;
    public float runInterval = 0.3f;

    [Header("Volume")]
    [Range(0f, 1f)] public float walkVolume = 0.6f;
    [Range(0f, 1f)] public float sneakVolume = 0.25f;
    [Range(0f, 1f)] public float runVolume = 1f;

    [Header("Speed Thresholds")]
    public float sneakMaxSpeed = 2f;
    public float walkMaxSpeed = 4f;

    private AudioSource audioSource;
    private FirstPersonController fpc;
    private float stepTimer;
    private bool wasMoving = false;

    private enum MovementMode { Walk, Sneak, Run }

    private void Awake()
    {
        fpc = GetComponent<FirstPersonController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
    }

    private void Update()
    {
        float speed = new Vector3(
            fpc.characterController.velocity.x,
            0f,
            fpc.characterController.velocity.z
        ).magnitude;

        bool isMoving = speed >= 0.1f && fpc.isGrounded;

        if (!isMoving)
        {
            if (wasMoving)
            {
                // Stopped moving - cut the sound immediately
                audioSource.Stop();
                stepTimer = 0f;
            }
            wasMoving = false;
            return;
        }

        wasMoving = true;
        MovementMode mode = GetMovementMode(speed);

        stepTimer += Time.deltaTime;
        if (stepTimer >= GetInterval(mode))
        {
            stepTimer = 0f;
            PlayFootstep(mode);
        }
    }

    private MovementMode GetMovementMode(float speed)
    {
        if (speed <= sneakMaxSpeed) return MovementMode.Sneak;
        if (speed <= walkMaxSpeed) return MovementMode.Walk;
        return MovementMode.Run;
    }

    private float GetInterval(MovementMode mode)
    {
        switch (mode)
        {
            case MovementMode.Sneak: return sneakInterval;
            case MovementMode.Run:   return runInterval;
            default:                 return walkInterval;
        }
    }

    private void PlayFootstep(MovementMode mode)
    {
        AudioClip[] clips;
        float volume;

        switch (mode)
        {
            case MovementMode.Sneak:
                clips = sneakClips;
                volume = sneakVolume;
                break;
            case MovementMode.Run:
                clips = runClips;
                volume = runVolume;
                break;
            default:
                clips = walkClips;
                volume = walkVolume;
                break;
        }

        if (clips == null || clips.Length == 0) return;

        audioSource.Stop();
        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.volume = volume;
        audioSource.Play();
    }
}