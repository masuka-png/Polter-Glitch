using UnityEngine;
using System.Collections;

public class ElevatorCameraManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _doorCamera;
    [SerializeField] private float _cameraTransitionDuration = 0.5f;
    [SerializeField] private float _doorViewDuration = 2f; // How long to watch doors before returning
    
    private Coroutine _transitionCoroutine;
    private Camera _activeCamera;

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = Camera.main;
        
        if (_doorCamera == null)
        {
            Debug.LogError("ElevatorCameraManager: Door Camera not assigned!");
            return;
        }

        _activeCamera = _mainCamera;
        _doorCamera.enabled = false;
    }

    /// <summary>
    /// Cuts to door camera, watches doors, then cuts back
    /// </summary>
    public void ShowDoorOpeningSequence()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(DoorOpeningSequence());
    }

    private IEnumerator DoorOpeningSequence()
    {
        // Fade to door camera
        yield return StartCoroutine(TransitionCamera(_mainCamera, _doorCamera, _cameraTransitionDuration));
        _activeCamera = _doorCamera;

        // Watch doors for duration
        yield return new WaitForSeconds(_doorViewDuration);

        // Fade back to main camera
        yield return StartCoroutine(TransitionCamera(_doorCamera, _mainCamera, _cameraTransitionDuration));
        _activeCamera = _mainCamera;
    }

    /// <summary>
    /// Shows elevator interior camera - for use during elevator movement
    /// </summary>
    public void ShowElevatorInterior()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionCamera(_mainCamera, _doorCamera, _cameraTransitionDuration));
        _activeCamera = _doorCamera;
    }

    /// <summary>
    /// Returns to main camera
    /// </summary>
    public void ReturnToMainCamera()
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionCamera(_doorCamera, _mainCamera, _cameraTransitionDuration));
        _activeCamera = _mainCamera;
    }

    private IEnumerator TransitionCamera(Camera fromCamera, Camera toCamera, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade out from camera, fade in to camera
            fromCamera.GetComponent<AudioListener>().enabled = Mathf.Lerp(1f, 0f, t) > 0.5f;
            toCamera.GetComponent<AudioListener>().enabled = Mathf.Lerp(0f, 1f, t) > 0.5f;

            fromCamera.enabled = Mathf.Lerp(1f, 0f, t) > 0.01f;
            toCamera.enabled = Mathf.Lerp(0f, 1f, t) > 0.01f;

            yield return null;
        }

        fromCamera.enabled = false;
        toCamera.enabled = true;

        // Ensure proper audio listeners
        if (fromCamera.TryGetComponent<AudioListener>(out var fromListener))
            fromListener.enabled = false;
        if (toCamera.TryGetComponent<AudioListener>(out var toListener))
            toListener.enabled = true;
    }

    public Camera GetActiveCamera => _activeCamera;
}
