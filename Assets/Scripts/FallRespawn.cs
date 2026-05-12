using UnityEngine;

public class FallDetector : MonoBehaviour
{
    // Drag your moving platform or its specific respawn point here
    [SerializeField] private Transform respawnPoint; 

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object falling is actually the player
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        // If using CharacterController, it must be disabled before moving the transform
        CharacterController cc = player.GetComponent<CharacterController>();
        
        if (cc != null) cc.enabled = false;

        // Move player to the current position of the platform/anchor
        player.transform.position = respawnPoint.position;
        player.transform.rotation = respawnPoint.rotation;

        if (cc != null) cc.enabled = true;
        
        // Optional: Reset velocity if using Rigidbody
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}