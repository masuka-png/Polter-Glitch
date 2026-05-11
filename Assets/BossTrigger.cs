using UnityEngine;
using EasyPeasyFirstPersonController;

public class BossTrigger : MonoBehaviour
{
    [Header("Object Names in Scene")]
    public string entryPlatformName = "Bridge";
    public string bossScreenRendererName = "Screen_Boss";

    [Header("TV Screen")]
    public Material eyeballMaterial;

    [Header("Intro Object")]
    public ServerRack introObject;

    [Header("References")]
    public PlatformManager platformManager;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;
        triggered = true;

        // Disable trigger so it never fires again
        GetComponent<Collider>().enabled = false;

        // 1. Disappear the entry platform
        GameObject entryPlatform = GameObject.Find(entryPlatformName);
        if (entryPlatform != null)
            entryPlatform.SetActive(false);

        // 2. Swap only the screen material (Element 1), leaving Element 0 untouched
        GameObject bossScreen = GameObject.Find(bossScreenRendererName);
        if (bossScreen != null)
        {
            Renderer rend = bossScreen.GetComponent<Renderer>();
            if (rend != null && eyeballMaterial != null)
            {
                Material[] mats = rend.materials;
                mats[1] = eyeballMaterial;
                rend.materials = mats;
            }
        }

        // 3. Sink the intro object
        if (introObject != null)
            introObject.Sink();

        // 4. Tell PlatformManager to start rising
        if (platformManager != null)
            platformManager.StartRising();
    }
}