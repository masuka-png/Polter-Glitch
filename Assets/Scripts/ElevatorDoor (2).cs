using UnityEngine;
using System.Collections;

public class ElevatorDoor : MonoBehaviour
{
    [SerializeField] private float _openSpeed = 2f;
    [SerializeField] private float _closeSpeed = 2f;
    [SerializeField] private Vector3 _openDirection = Vector3.right;
    [SerializeField] private float _openDistance = 1.5f;
    [SerializeField] private bool _autoOpenOnStart = true;
    [SerializeField] private float _autoOpenDelay = 3f;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;
    private Coroutine _animationCoroutine;

    private void Start()
    {
        _closedPosition = transform.localPosition;
        _openPosition = _closedPosition + (_openDirection.normalized * _openDistance);

        if (_autoOpenOnStart)
            StartCoroutine(AutoOpenOnStart());
    }

    private IEnumerator AutoOpenOnStart()
    {
        yield return new WaitForSeconds(_autoOpenDelay);
        Open();
    }

    public void Open()
    {
        if (_isOpen) return;

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(AnimateDoor(_openPosition, _openSpeed, true));
    }

    public void Close()
    {
        if (!_isOpen) return;

        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(AnimateDoor(_closedPosition, _closeSpeed, false));
    }

    private IEnumerator AnimateDoor(Vector3 targetPosition, float speed, bool opening)
    {
        while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * speed
            );
            yield return null;
        }

        transform.localPosition = targetPosition;
        _isOpen = opening;
    }

    public bool IsOpen => _isOpen;
}