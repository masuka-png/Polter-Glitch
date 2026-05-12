using UnityEngine;

public class ComputerTrigger : MonoBehaviour
{
    [SerializeField] private PlatformManager _platformManager;
    [SerializeField] private PlayerLock _playerLock;
    [SerializeField] private Transform _teleportTarget;

    public void Reset()
    {
        // kept for PlatformManager compatibility
    }

    public void FireTrigger()
    {
        Debug.Log("FireTrigger - playerLock null? " + (_playerLock == null) + " target null? " + (_teleportTarget == null));

        if (_playerLock != null && _teleportTarget != null)
            _playerLock.TeleportAndLock(_teleportTarget);

        _platformManager.OnPlayerReachedComputer();
    }
}