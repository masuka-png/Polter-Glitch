using UnityEngine;
using System.Collections;

public class ComputerFaceDisplay : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material _materialA;
    [SerializeField] private Material _materialB;
    [SerializeField] private Material _materialWin;

    [Header("Settings")]
    [SerializeField] private float _switchInterval = 1f;

    private MeshRenderer _meshRenderer;
    private bool _isFlickering = false;
    private bool _winTriggered = false;
    private float _timer = 0f;
    private bool _showingA = true;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (_winTriggered || !_isFlickering) return;

        _timer += Time.deltaTime;
        if (_timer >= _switchInterval)
        {
            _timer = 0f;
            _showingA = !_showingA;
            SetElement1(_showingA ? _materialA : _materialB);
        }
    }

    public void StartFlicker()
    {
        if (_winTriggered) return;
        Debug.Log("StartFlicker called");
        StartCoroutine(DelayedStartFlicker());
    }

    private IEnumerator DelayedStartFlicker()
    {
        yield return new WaitForEndOfFrame();
        _isFlickering = true;
        _timer = 0f;
        _showingA = true;
        SetElement1(_materialA);
    }

    public void StopFlicker()
    {
        if (_winTriggered) return;
        _isFlickering = false;
        SetElement1(_materialA);
    }

    public void TriggerWinMaterial()
    {
        _winTriggered = true;
        _isFlickering = false;
        SetElement1(_materialWin);
    }

    private void SetElement1(Material mat)
    {
        if (_meshRenderer == null) return;
        Material[] mats = _meshRenderer.materials;
        if (mats.Length > 1)
        {
            mats[1] = mat;
            _meshRenderer.materials = mats;
        }
        else
        {
            Debug.LogWarning("ComputerFaceDisplay: MeshRenderer has fewer than 2 materials!");
        }
    }
}