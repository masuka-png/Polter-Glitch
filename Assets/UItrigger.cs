using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    private AIController _ai;

    private void Awake()
    {
        _ai = GetComponentInParent<AIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER ENTERED ATTACK RANGE");
            _ai.OnPlayerEnteredAttackRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ai.OnPlayerExitedAttackRange();
        }
    }
}