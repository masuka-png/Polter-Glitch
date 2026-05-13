using UnityEngine;

public class BossAlarmTrigger : MonoBehaviour
{
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private ComputerFaceDisplay _computerFaceDisplay;
    [SerializeField] private BossTrigger _bossTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;
        if (!_bossTrigger.HasTriggered) return;

        _computerFaceDisplay?.StartFlicker();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;

        _computerFaceDisplay?.StopFlicker();
    }
}