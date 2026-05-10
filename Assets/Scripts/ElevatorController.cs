using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private List<ElevatorDoor> _doors = new List<ElevatorDoor>();
    [SerializeField] private float _doorsOpenDuration = 3f;
    [SerializeField] private bool _autoCloseDoors = true;

    [Header("Fake Movement Settings")]
    [SerializeField] private float _fakeTravelDuration = 3f;
    [SerializeField] private float _moveDelay = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _doorsOpenSound;
    [SerializeField] private AudioClip _doorsCloseSound;
    [SerializeField] private AudioClip _elevatorMovingSound;

    [Header("Camera")]
    [SerializeField] private ElevatorCameraManager _cameraManager;

    [Header("Scene Transition")]
    [SerializeField] private string _destinationScene;
    [SerializeField] private float _sceneLoadDelay = 3f;

    private float _doorsOpenTimer = 0f;
    private bool _doorsOpened = false;
    private bool _elevatorMoving = false;
    private Vector3 _startPosition;

    private void Start()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        if (_cameraManager == null)
            _cameraManager = FindObjectOfType<ElevatorCameraManager>();

        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_doorsOpened && _autoCloseDoors)
        {
            _doorsOpenTimer -= Time.deltaTime;

            if (_doorsOpenTimer <= 0f)
            {
                CloseDoors();
                _doorsOpened = false;
            }
        }
    }

    public void Interact(Interactor interactor)
    {
        OpenDoors();
    }

    public void OpenDoors()
    {
        if (_doorsOpened) return;

        Debug.Log("Elevator doors opening!");

        foreach (ElevatorDoor door in _doors)
        {
            door.Open();
        }

        if (_doorsOpenSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_doorsOpenSound);
        }

        if (_cameraManager != null)
        {
            _cameraManager.ShowDoorOpeningSequence();
        }

        _doorsOpened = true;
        _doorsOpenTimer = _doorsOpenDuration;
    }

    public void CloseDoors()
    {
        if (!_doorsOpened) return;

        Debug.Log("Elevator doors closing!");

        foreach (ElevatorDoor door in _doors)
        {
            door.Close();
        }

        if (_doorsCloseSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_doorsCloseSound);
        }

        _doorsOpened = false;
    }

    public void PlayerEntered(GameObject player)
    {
        Debug.Log("Player entered elevator!");

    
        CloseDoors();


        player.transform.SetParent(transform);


        StartCoroutine(ElevatorSequence());
    }

    private IEnumerator ElevatorSequence()
    {

        yield return new WaitForSeconds(0.5f);




        yield return new WaitForSeconds(_moveDelay);


        yield return StartCoroutine(SimulateElevatorTravel());


        if (!string.IsNullOrEmpty(_destinationScene))
        {
            yield return new WaitForSeconds(_sceneLoadDelay);
            SceneManager.LoadScene(_destinationScene);
        }
    }

    private IEnumerator SimulateElevatorTravel()
    {
        _elevatorMoving = true;

        Debug.Log("Elevator travelling...");


        if (_elevatorMovingSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(_elevatorMovingSound);
        }


        yield return new WaitForSeconds(_fakeTravelDuration);

        _elevatorMoving = false;

        Debug.Log("Elevator arrived at destination!");
    }

    /// <summary>
    /// Resets elevator state
    /// </summary>
    public void ResetPosition()
    {
        transform.position = _startPosition;

        foreach (ElevatorDoor door in _doors)
        {
            door.Close();
        }

        _doorsOpened = false;
        _elevatorMoving = false;
    }

    public bool AreDoorsOpen => _doorsOpened;
    public bool IsMoving => _elevatorMoving;
}