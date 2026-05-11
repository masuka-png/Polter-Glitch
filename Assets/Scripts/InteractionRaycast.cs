using UnityEngine;

public class InteractionRaycast : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private float _raycastDistance = 100f;
    [SerializeField] private InteractionUIManager _uiManager;
    [SerializeField] private LayerMask _interactableLayer;
    
    private IInteractable _lastInteractable = null;

    private void Start()
    {
        if (_playerCamera == null)
            _playerCamera = Camera.main;

        if (_uiManager == null)
            _uiManager = FindObjectOfType<InteractionUIManager>();

        if (_interactableLayer == 0)
        {
            _interactableLayer = LayerMask.GetMask("Interactable");
        }
    }

    private void Update()
    {
        // Cast ray from center of screen
        Ray ray = _playerCamera.ScreenPointToRay(_playerCamera.pixelRect.center);
        
        IInteractable hitInteractable = null;
        InteractableType interactableType = InteractableType.Door;

        // Check if we hit something interactable
        if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                hitInteractable = interactable;
                interactableType = DetermineInteractableType(hit.collider.gameObject);
            }
        }

        // Handle UI changes
        if (hitInteractable != _lastInteractable)
        {
            // Hide old UI
            if (_lastInteractable != null)
            {
                _uiManager.HidePromptIfCurrent(_lastInteractable);
            }

            // Show new UI
            if (hitInteractable != null)
            {
                _uiManager.ShowPrompt(hitInteractable, interactableType);
            }

            _lastInteractable = hitInteractable;
        }
    }

    private InteractableType DetermineInteractableType(GameObject gameObject)
    {
        // Check the component type to determine what we're looking at
        if (gameObject.GetComponent<ElevatorKeyboard>() != null)
            return InteractableType.Keyboard;

        if (gameObject.GetComponent<ElevatorDoor>() != null)
            return InteractableType.Door;

        if (gameObject.GetComponent<ElevatorController>() != null)
            return InteractableType.ClosedElevator;

        return InteractableType.Door; // Default
    }

    private void OnDrawGizmos()
    {
        if (_playerCamera == null)
            _playerCamera = Camera.main;

        if (_playerCamera == null)
            return;

        // Draw raycast line in scene view
        Ray ray = _playerCamera.ScreenPointToRay(_playerCamera.pixelRect.center);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ray.origin, ray.direction * _raycastDistance);
    }
}
