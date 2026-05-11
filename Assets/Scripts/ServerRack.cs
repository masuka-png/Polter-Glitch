using UnityEngine;
using System.Collections;

public class ServerRack : MonoBehaviour
{
    [Header("Settings")]
    public float riseHeight = 2f;        // How far above the platform surface it rises
    public float riseSpeed = 3f;
    public float sinkSpeed = 3f;

    private Vector3 _hiddenPosition;
    private Vector3 _raisedPosition;
    private Coroutine _animationCoroutine;

    void Start()
    {
        // Current position is the hidden position (below platform surface)
        _hiddenPosition = transform.localPosition;
        _raisedPosition = _hiddenPosition + Vector3.up * riseHeight;
    }

    public void Rise()
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(Animate(_raisedPosition, riseSpeed));
    }

    public void Sink()
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(Animate(_hiddenPosition, sinkSpeed));
    }

    public IEnumerator SinkAndWait()
    {
        Sink();
        float duration = 1f / sinkSpeed;
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator Animate(Vector3 target, float speed)
    {
        float duration = 1f / speed;
        float elapsed = 0f;
        Vector3 start = transform.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localPosition = target;
    }
}