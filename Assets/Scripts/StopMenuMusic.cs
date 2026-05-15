using UnityEngine;

public class StopMenuMusic : MonoBehaviour
{
    private void Awake()
    {
        MusicManager menuMusic = FindObjectOfType<MusicManager>();
        if (menuMusic != null)
            Destroy(menuMusic.gameObject);
    }
}