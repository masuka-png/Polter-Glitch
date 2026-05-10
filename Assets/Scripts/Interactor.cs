using UnityEngine;
using UnityEngine.InputSystem;


public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionPointRadius = 0.05f;
    [SerializeField] private LayerMask _interactableMask;

    private readonly Collider[] _colliders = new Collider[3];
    [SerializeField] private int _numFound;

    private void Start()
    {
        Debug.Log("Interactor started");
    }
    private void Update()
    {
        _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

        for (int i = 0; i < _numFound; i++)
        {
            var interactable = _colliders[i].GetComponent<IInteractable>();

            if (interactable != null && Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact(this);
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
    }

}
