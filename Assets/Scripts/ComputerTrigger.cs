using UnityEngine;

public class ComputerTrigger : MonoBehaviour
{
    [SerializeField] private PlatformManager _platformManager;
    [SerializeField] private PlayerLock _playerLock;
    [SerializeField] private Transform _teleportTarget;  // assign per-trigger in inspector
    
    private bool _triggered = false;

    public void Reset()
    {
        _triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ComputerTrigger hit by: " + other.name + " tag: " + other.tag);
    
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
    
        Debug.Log("PlayerLock null? " + (_playerLock == null) + " Target null? " + (_teleportTarget == null));
    
        if (_playerLock != null && _teleportTarget != null)
        _playerLock.TeleportAndLock(_teleportTarget);
    
        _platformManager.OnPlayerReachedComputer();
    }
}