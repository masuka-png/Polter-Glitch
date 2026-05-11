using UnityEngine;

public class ComputerTrigger : MonoBehaviour
{
    [SerializeField] private PlatformManager _platformManager;

    private bool _triggered = false;

    public void Reset()
    {
        _triggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        _platformManager.OnPlayerReachedComputer();
    }
}