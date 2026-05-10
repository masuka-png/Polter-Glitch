using UnityEngine;
using EasyPeasyFirstPersonController;

public class BossTrigger : MonoBehaviour
{
    [Header("Object Names in Scene")]
    public string entryPlatformName = "Bridge";
    public string bossScreenRendererName = "Screen_Boss";
    public string risingPlatformName = "Platform";

    [Header("TV Screen")]
    public Material eyeballMaterial;

    [Header("Rising Platform")]
    public float riseSpeed = 2f;

    private GameObject entryPlatform;
    private GameObject bossScreenRenderer;
    private GameObject risingPlatform;

    private CharacterController playerController;
    private FirstPersonController playerFPC;
    private bool playerOnPlatform = false;
    private bool triggered = false;
    private bool isRising = false;

    void Start()
    {
        entryPlatform = GameObject.Find(entryPlatformName);
        bossScreenRenderer = GameObject.Find(bossScreenRendererName);
        risingPlatform = GameObject.Find(risingPlatformName);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerOnPlatform = true;
        playerController = other.GetComponentInParent<CharacterController>();
        playerFPC = other.GetComponentInParent<FirstPersonController>();
        if (playerFPC != null) playerFPC.onMovingPlatform = true;

        if (triggered) return;
        triggered = true;

        // Disable the trigger so it never fires again
        GetComponent<Collider>().enabled = false;

        // 1. Disappear the entry platform
        if (entryPlatform != null)
            entryPlatform.SetActive(false);

        // 2. Swap only the screen material (Element 1), leaving Element 0 untouched
        if (bossScreenRenderer != null)
        {
            Renderer rend = bossScreenRenderer.GetComponent<Renderer>();
            if (rend != null && eyeballMaterial != null)
            {
                Material[] mats = rend.materials;
                mats[1] = eyeballMaterial;
                rend.materials = mats;
            }
        }

        // 3. Start rising the platform
        isRising = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerOnPlatform = false;
        if (playerFPC != null) playerFPC.onMovingPlatform = false;
    }

    void Update()
    {
        if (!isRising || risingPlatform == null) return;

        Vector3 before = risingPlatform.transform.position;
        risingPlatform.transform.Translate(Vector3.up * riseSpeed * Time.deltaTime, Space.World);
        Vector3 delta = risingPlatform.transform.position - before;
        Debug.Log("onMovingPlatform: " + (playerFPC != null ? playerFPC.onMovingPlatform : false));

        if (playerOnPlatform && playerController != null)
            playerController.Move(delta);
    }
}