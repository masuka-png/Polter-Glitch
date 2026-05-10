using UnityEngine;

public class ElevatorEntranceDetector : MonoBehaviour
{
    [SerializeField] private ElevatorController _elevator;
    [SerializeField] private Transform _playerTransform; // Drag player here or it will find it
    [SerializeField] private float _detectionRadius = 2f;
    
    private bool _playerInside = false;
    private bool _sequenceTriggered = false;

    private void Start()
    {
        if (_elevator == null)
        {
            Debug.LogError("ElevatorEntranceDetector: Elevator not assigned!");
            return;
        }

        // Auto-find player if not assigned
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("ElevatorEntranceDetector: Could not find Player tag!");
            }
        }
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        // Check distance between player and elevator entrance
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer < _detectionRadius)
        {
            if (_elevator.AreDoorsOpen && !_sequenceTriggered)
            {
                _sequenceTriggered = true;
                _playerInside = true;
                Debug.Log("Player detected in elevator!");
                
                // Trigger elevator sequence
                _elevator.PlayerEntered(_playerTransform.gameObject);
            }
        }
        else
        {
            _sequenceTriggered = false;
            _playerInside = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }

    public bool IsPlayerInside => _playerInside;
}
