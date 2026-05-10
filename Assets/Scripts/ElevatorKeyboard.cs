using UnityEngine;

public class ElevatorKeyboard : MonoBehaviour, IInteractable
{
    [SerializeField] private Elevator _targetElevator;
    [SerializeField] private Material _activeKeyboardMaterial;
    private Material _originalMaterial;
    private MeshRenderer _meshRenderer;
    private float _feedbackDuration = 0.5f;
    private float _feedbackTimer = 0f;
    private bool _showingFeedback = false;

    private void Start()
    {
        if (_targetElevator == null)
        {
            Debug.LogError("ElevatorKeyboard: Target Elevator not assigned!");
            return;
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer != null)
        {
            _originalMaterial = _meshRenderer.material;
        }
    }

    private void Update()
    {
        if (_showingFeedback)
        {
            _feedbackTimer -= Time.deltaTime;
            if (_feedbackTimer <= 0f)
            {
                ResetMaterial();
                _showingFeedback = false;
            }
        }
    }

    public void Interact(Interactor interactor)
    {
        if (_targetElevator == null) return;

        Debug.Log("Keyboard activated! Opening elevator doors...");
        
        // Trigger the elevator
        _targetElevator.OpenDoors();
        
        // Visual feedback
        ShowActivationFeedback();
    }

    private void ShowActivationFeedback()
    {
        if (_meshRenderer != null && _activeKeyboardMaterial != null)
        {
            _meshRenderer.material = _activeKeyboardMaterial;
            _feedbackTimer = _feedbackDuration;
            _showingFeedback = true;
        }
    }

    private void ResetMaterial()
    {
        if (_meshRenderer != null && _originalMaterial != null)
        {
            _meshRenderer.material = _originalMaterial;
        }
    }
}
