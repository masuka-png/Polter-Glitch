using UnityEngine;
using System.Collections;

public class ElevatorDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;
    [SerializeField] private float _openDistance = 1.5f; // How far doors slide apart
    [SerializeField] private float _openSpeed = 2f; // How fast doors open
    
    private Vector3 _leftDoorClosedPos;
    private Vector3 _rightDoorClosedPos;
    private bool _isOpen = false;
    private bool _isMoving = false;

    private void Start()
    {
        // Store the initial closed positions of the doors
        _leftDoorClosedPos = _leftDoor.localPosition;
        _rightDoorClosedPos = _rightDoor.localPosition;
    }

    public void Interact(Interactor interactor)
    {
        if (!_isMoving)
        {
            if (_isOpen)
            {
                CloseDoors();
            }
            else
            {
                OpenDoors();
            }
        }
    }

    private void OpenDoors()
    {
        if (_isMoving) return;

        _isMoving = true;
        _isOpen = true;

        // Animate doors opening
        StartCoroutine(AnimateDoors(
            _leftDoorClosedPos,
            _leftDoorClosedPos + Vector3.left * _openDistance,
            _rightDoorClosedPos,
            _rightDoorClosedPos + Vector3.right * _openDistance
        ));
    }

    private void CloseDoors()
    {
        if (_isMoving) return;

        _isMoving = true;
        _isOpen = false;

        // Animate doors closing
        StartCoroutine(AnimateDoors(
            _leftDoor.localPosition,
            _leftDoorClosedPos,
            _rightDoor.localPosition,
            _rightDoorClosedPos
        ));
    }

    private IEnumerator AnimateDoors(Vector3 leftStart, Vector3 leftEnd, Vector3 rightStart, Vector3 rightEnd)
    {
        float elapsed = 0f;
        float duration = 1f / _openSpeed; // Convert speed to duration

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Use Lerp for smooth animation
            _leftDoor.localPosition = Vector3.Lerp(leftStart, leftEnd, progress);
            _rightDoor.localPosition = Vector3.Lerp(rightStart, rightEnd, progress);

            yield return null;
        }

        // Ensure final positions are exact
        _leftDoor.localPosition = leftEnd;
        _rightDoor.localPosition = rightEnd;
        _isMoving = false;
    }

    // Optional: Draw gizmos to visualize door positions in editor
    private void OnDrawGizmosSelected()
    {
        if (_leftDoor == null || _rightDoor == null) return;

        Gizmos.color = Color.green;
        Vector3 leftOpenPos = _leftDoor.position + Vector3.left * _openDistance;
        Vector3 rightOpenPos = _rightDoor.position + Vector3.right * _openDistance;

        Gizmos.DrawWireCube(_leftDoor.position, Vector3.one * 0.2f);
        Gizmos.DrawWireCube(leftOpenPos, Vector3.one * 0.2f);
        Gizmos.DrawLine(_leftDoor.position, leftOpenPos);

        Gizmos.DrawWireCube(_rightDoor.position, Vector3.one * 0.2f);
        Gizmos.DrawWireCube(rightOpenPos, Vector3.one * 0.2f);
        Gizmos.DrawLine(_rightDoor.position, rightOpenPos);
    }
}
