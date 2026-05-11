using UnityEngine;

public class ServerRackFormation : MonoBehaviour
{
    public ServerRack[] racks;

    public void RiseAll()
    {
        foreach (ServerRack rack in racks)
        {
            if (rack != null)
                rack.Rise();
        }
    }

    public void SinkAll()
    {
        foreach (ServerRack rack in racks)
        {
            if (rack != null)
                rack.Sink();
        }
    }

    // Returns when all racks have finished sinking
    public System.Collections.IEnumerator SinkAllAndWait()
    {
        foreach (ServerRack rack in racks)
        {
            if (rack != null)
                rack.Sink();
        }

        // Wait for the longest possible sink duration
        float maxDuration = 0f;
        foreach (ServerRack rack in racks)
        {
            if (rack != null)
                maxDuration = Mathf.Max(maxDuration, 1f / rack.sinkSpeed);
        }

        yield return new WaitForSeconds(maxDuration);
    }
}