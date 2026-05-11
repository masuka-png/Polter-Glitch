using UnityEngine;
using System.Collections;

public class ServerRack : MonoBehaviour
{
    [Header("Settings")]
    public bool startsRaised = false; 
    public float riseHeight = 2f;        // How far above the platform surface it rises
    public float riseSpeed = 3f;
    public float sinkSpeed = 3f;

    private Vector3 _hiddenPosition;
    private Vector3 _raisedPosition;
    private Coroutine _animationCoroutine;

    void Start()
    {
        if (startsRaised)
        {
            _raisedPosition = transform.localPosition;
            _hiddenPosition = _raisedPosition - Vector3.up * riseHeight;
        }
        else
        {
            _hiddenPosition = transform.localPosition;
            _raisedPosition = _hiddenPosition + Vector3.up * riseHeight;
        }
}

    public void Rise()
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(Animate(_raisedPosition, riseSpeed));
    }

    public void Sink()
    {  
        Debug.Log("Sink called, hidden position: " + _hiddenPosition + " current: " + transform.localPosition);
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