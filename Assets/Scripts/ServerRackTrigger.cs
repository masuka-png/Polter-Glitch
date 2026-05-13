using UnityEngine;
using UnityEngine.Events;

public class ServerRackTrigger : MonoBehaviour
{
    [Header("Hack Settings")]
    [SerializeField] private int _totalHacks = 3;
    [SerializeField] private float _fillRate = 0.08f;
    [SerializeField] private float _drainRate = 0.5f;

    [Header("References")]
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private HackUI _hackUI;
    [SerializeField] private ServerRack _serverRack;
    [SerializeField] private ComputerTrigger _computerTrigger;
    [SerializeField] private ComputerFaceDisplay _computerFaceDisplay;

    [Header("Fall On Hack Complete")]
    [SerializeField] private GameObject _serverRackObject;
    [SerializeField] private GameObject _computerObject;

    [Header("Events")]
    public UnityEvent onHackComplete;

    private int _hacksCompleted = 0;
    private bool _playerInRange = false;
    private bool _isHacking = false;
    private bool _allHacksDone = false;
    private float _progress = 0f;

    private void Start()
    {
        _hackUI.SetProgress(0f, 0, _totalHacks);
    }

    private void Update()
    {
        if (!_playerInRange || _allHacksDone) return;

        if (Input.GetKeyDown(KeyCode.E) && !_isHacking)
        {
            _isHacking = true;
            _hackUI.ShowPrompt(false);
            _hackUI.ShowProgressBar(true);
        }

        if (_isHacking)
        {
            if (Input.GetKeyDown(KeyCode.E))
                _progress += _fillRate;

            _progress -= _drainRate * Time.deltaTime;
            _progress = Mathf.Clamp01(_progress);

            _hackUI.SetProgress(_progress, _hacksCompleted, _totalHacks);

            if (_progress >= 1f)
                CompleteHack();
        }
    }

    public void CancelHack()
    {
        _playerInRange = false;
        _isHacking = false;
        _progress = 0f;

        _hackUI.ShowPrompt(false);
        _hackUI.ShowProgressBar(false);
        _hackUI.SetProgress(0f, _hacksCompleted, _totalHacks);
        _computerFaceDisplay?.StopFlicker();
    }

    private void CompleteHack()
    {
        _hacksCompleted++;
        _progress = 0f;
        _isHacking = false;

        _hackUI.ShowProgressBar(false);
        _hackUI.SetProgress(0f, _hacksCompleted, _totalHacks);

        _computerTrigger?.FireTrigger();

        if (_hacksCompleted >= _totalHacks)
        {
            _allHacksDone = true;
            _hackUI.ShowPrompt(false);
            TriggerFall();
            onHackComplete.Invoke();
        }
        else
        {
            _playerInRange = false;
            _hackUI.ShowPrompt(false);
        }
    }

    private void TriggerFall()
    {
        EnableFall(_serverRackObject);
        EnableFall(_computerObject);
    }

    private void EnableFall(GameObject obj)
    {
        if (obj == null) return;

        ServerRack rack = obj.GetComponent<ServerRack>();
        if (rack != null) rack.enabled = false;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddTorque(new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.2f, 0.2f),
            Random.Range(-1f, 1f)) * 2f, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_allHacksDone) return;
        if (!other.CompareTag(_playerTag)) return;

        _playerInRange = true;
        _serverRack?.Rise();
        _hackUI.ShowPrompt(true);
        _computerFaceDisplay?.StartFlicker();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;

        _playerInRange = false;
        _isHacking = false;
        _progress = 0f;

        _hackUI.ShowPrompt(false);
        _hackUI.ShowProgressBar(false);
        _hackUI.SetProgress(0f, _hacksCompleted, _totalHacks);
        _computerFaceDisplay?.StopFlicker();
    }
}