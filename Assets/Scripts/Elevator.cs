using UnityEngine;
using System.Collections.Generic;

public class Elevator : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ElevatorDoor> _doors = new List<ElevatorDoor>();
    [SerializeField] private float _doorsOpenDuration = 3f; // How long doors stay open
    [SerializeField] private bool _autoCloseDoors = true; // Auto-close after duration
    
    private float _doorsOpenTimer = 0f;
    private bool _doorsOpened = false;

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

        _doorsOpened = true;
        _doorsOpenTimer = _doorsOpenDuration;
    }

    public void CloseDoors()
    {
        Debug.Log("Elevator doors closing!");
        
        foreach (ElevatorDoor door in _doors)
        {
            door.Close();
        }

        _doorsOpened = false;
    }

    public bool AreDoorsOpen => _doorsOpened;
}
