using System.Collections;
using UnityEngine;

public class LoadingFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float visibleTime = 2f;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // Keep visible
        yield return new WaitForSeconds(visibleTime);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        // Optional: disable object after fade
        gameObject.SetActive(false);
    }
}