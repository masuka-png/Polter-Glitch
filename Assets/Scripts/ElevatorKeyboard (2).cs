using UnityEngine;

public class ElevatorKeyboard : MonoBehaviour, IInteractable
{
    [SerializeField] private ElevatorController _targetElevator;
    [SerializeField] private AudioClip _keyboardBeepSound;
    [SerializeField] private InteractionUIManager _uiManager;
    private bool _hasBeenUsed = false;

    private AudioSource _audioSource;


    private void Start()
    {


        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (_uiManager == null)
        {
            _uiManager = FindObjectOfType<InteractionUIManager>();
        }
    }

    private void Update()
    {

    }

    public void Interact(Interactor interactor)
    {


        if (_targetElevator == null) return;


        _hasBeenUsed = true;

        _targetElevator.OpenDoors();

        if (_keyboardBeepSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_keyboardBeepSound);
        }


        if (_uiManager != null)
        {
            _uiManager.HidePromptIfCurrent(this);
        }

        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

    }


}
