using UnityEngine;
using EasyPeasyFirstPersonController;

public class PlayerLock : MonoBehaviour
{
    [Header("References")]
    public Transform risingPlatform;

    private FirstPersonController _fpc;
    private bool _locked = false;
    private Vector3 _lockedPosition;
    private float _platformYAtLock;
    private float _localOffsetY;

    void Awake()
    {
        _fpc = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (!_locked) return;

        // Follow platform's Y movement while keeping X and Z fixed
        float platformDelta = risingPlatform.position.y - _platformYAtLock;
        Vector3 newPos = _lockedPosition;
        newPos.y = _lockedPosition.y + platformDelta;
        transform.position = newPos;
        _fpc.moveDirection = Vector3.zero;
    }

    public void TeleportAndLock(Transform target)
    {
        _lockedPosition = target.position;
        _platformYAtLock = risingPlatform.position.y;
        transform.position = _lockedPosition;
        _locked = true;
    }

    public void Release()
    {
        _locked = false;
    }
}