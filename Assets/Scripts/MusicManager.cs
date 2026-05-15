using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    private void Awake()
    {
        // If one already exists, destroy this duplicate and keep the original
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}