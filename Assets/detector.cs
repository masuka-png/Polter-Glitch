using UnityEngine;

public class SpotlightDetector : MonoBehaviour
{
    public Transform player;        // Assign your player
    public Light spotLight;         // Assign the spotlight

    private bool isPlayerInLight = false;

    void Update()
    {
        if (player == null || spotLight == null)
        {
            Debug.Log("Missing references!");
            return;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        Debug.Log("Distance: " + distanceToPlayer);

        if (distanceToPlayer <= spotLight.range)
        {
            Debug.Log("Within range");

            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            Debug.Log("Angle: " + angle);

            if (angle <= spotLight.spotAngle / 2f)
            {
                Debug.Log("Inside cone");

                Ray ray = new Ray(transform.position, directionToPlayer.normalized);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, spotLight.range))
                {
                    Debug.Log("Ray hit: " + hit.transform.name);

                    if (hit.transform == player)
                    {
                        Debug.Log("✅ PLAYER DETECTED");
                    }
                }
                else
                {
                    Debug.Log("Raycast hit nothing");
                }
            }
        }
    }
}