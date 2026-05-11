using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _doorTransform;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private float _openSpeed = 2f;
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private string _requiredKeyCode = "";

    public AudioSource audioSource;
    public AudioClip openSound;
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private bool _isOpen = false;
    private bool _isAnimating = false;
    private Coroutine _animationCoroutine;
    
    private void Start()
    {
        _closedRotation = _doorTransform.localRotation;
        _openRotation = _closedRotation * Quaternion.Euler(0, 0, _openAngle);
    }
    
    public void Interact(Interactor interactor)
    {
        if (_isLocked && !string.IsNullOrEmpty(_requiredKeyCode))
        {
            Debug.Log("Door is locked! Need key: " + _requiredKeyCode);
            return;
        }
        
        ToggleDoor();
    }
    
    public void ToggleDoor()
    {
        if (_isAnimating)
            return;
        
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        
        _isOpen = !_isOpen;
        _animationCoroutine = StartCoroutine(AnimateDoor(_isOpen));
        audioSource.PlayOneShot(openSound);
    }
    
    private IEnumerator AnimateDoor(bool opening)
    {
        _isAnimating = true;
        Quaternion targetRotation = opening ? _openRotation : _closedRotation;
        float elapsedTime = 0f;
        float duration = 1f / _openSpeed;
        Quaternion startRotation = _doorTransform.localRotation;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _doorTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        
        _doorTransform.localRotation = targetRotation;
        _isAnimating = false;
    }
    

    public void LockDoor(string requiredKeyCode = "")
    {
        _isLocked = true;
        _requiredKeyCode = requiredKeyCode;
    }
    

    public void UnlockDoor()
    {
        _isLocked = false;
        _requiredKeyCode = "";
    }
    

    public bool TryUnlockWithCode(string keyCode)
    {
        if (!_isLocked || _requiredKeyCode != keyCode)
            return false;
        
        UnlockDoor();
        Debug.Log("Door unlocked!");
        return true;
    }
    
    public bool IsOpen => _isOpen;
    public bool IsLocked => _isLocked;
}
