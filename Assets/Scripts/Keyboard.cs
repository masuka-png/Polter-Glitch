using UnityEngine;
using System.Collections.Generic;

public class Keyboard : MonoBehaviour, IInteractable
{
    [SerializeField] private string _keyboardName = "Security Keyboard";
    [SerializeField] private List<KeyboardAction> _actions = new List<KeyboardAction>();
    [SerializeField] private bool _requiresCorrectPassword = false;
    [SerializeField] private string _correctPassword = "1234";
    
    private KeyboardUI _ui;
    private bool _isActive = false;
    
    private void Start()
    {
        _ui = FindObjectOfType<KeyboardUI>();
        if (_ui == null)
        {
            Debug.LogWarning("KeyboardUI not found in scene!");
        }
    }
    
    public void Interact(Interactor interactor)
    {
        if (_isActive)
            return;
        
        _isActive = true;
        
        if (_requiresCorrectPassword)
        {
            if (_ui != null)
            {
                _ui.ShowPasswordPrompt(_keyboardName, _correctPassword, OnPasswordEntered);
            }
            else
            {
                Debug.LogWarning("Cannot show UI - KeyboardUI not found");
                _isActive = false;
            }
        }
        else
        {
            ExecuteActions();
            _isActive = false;
        }
    }
    
    private void OnPasswordEntered(bool correct)
    {
        if (correct)
        {
            ExecuteActions();
        }
        else
        {
            Debug.Log("Incorrect password!");
        }
        _isActive = false;
    }
    
    private void ExecuteActions()
    {
        foreach (var action in _actions)
        {
            action.Execute();
        }
    }
}

/// <summary>
/// Represents a single action that the keyboard can perform
/// </summary>
[System.Serializable]
public class KeyboardAction
{
    [SerializeField] private ActionType _actionType;
    [SerializeField] private Door _door;
    [SerializeField] private string _keyCode;
    
    public enum ActionType
    {
        UnlockDoor,
        OpenDoor,
        ToggleDoor,
        LockDoor
    }
    
    public void Execute()
    {
        if (_door == null)
        {
            Debug.LogWarning("Door reference is null in KeyboardAction!");
            return;
        }
        
        switch (_actionType)
        {
            case ActionType.UnlockDoor:
                if (string.IsNullOrEmpty(_keyCode))
                    _door.UnlockDoor();
                else
                    _door.TryUnlockWithCode(_keyCode);
                break;
                
            case ActionType.OpenDoor:
                if (!_door.IsOpen)
                    _door.ToggleDoor();
                break;
                
            case ActionType.ToggleDoor:
                _door.ToggleDoor();
                break;
                
            case ActionType.LockDoor:
                _door.LockDoor(_keyCode);
                break;
        }
    }
}
