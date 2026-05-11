using UnityEngine;
using EasyPeasyFirstPersonController;
using System.Collections.Generic;

public class PlayerLock : MonoBehaviour
{
    [Header("References")]
    public Transform risingPlatform;

    [Header("Checkpoints")]
    public List<float> teleportAtY;
    public List<float> releaseAtY;
    public List<Transform> teleportTargets;

    private FirstPersonController _fpc;
    private bool _locked = false;
    private int _nextCheckpoint = 0;
    private Transform _currentTarget;

    void Awake()
    {
        _fpc = GetComponent<FirstPersonController>();
    }

    void Update()
    {
        if (_nextCheckpoint < teleportAtY.Count && _nextCheckpoint < teleportTargets.Count)
        {
            if (risingPlatform.position.y >= teleportAtY[_nextCheckpoint])
            {
                TeleportAndLock(teleportTargets[_nextCheckpoint]);
                _nextCheckpoint++;
            }
        }

        if (!_locked) return;
        Debug.Log("Player is locked, pinning position");

        transform.position = _currentTarget.position;
        _fpc.moveDirection = Vector3.zero;

        int currentCheckpoint = _nextCheckpoint - 1;
        if (currentCheckpoint >= 0 && currentCheckpoint < releaseAtY.Count)
        {
            if (risingPlatform.position.y >= releaseAtY[currentCheckpoint])
                Release();
        }
    }

    public void TeleportAndLock(Transform target)
    {
        _currentTarget = target;
        transform.position = target.position;
        _locked = true;
        Debug.Log("TeleportAndLock called, locked: " + _locked + " target: " + target.name);
    }

    private void Release()
    {
        _locked = false;
    }
}