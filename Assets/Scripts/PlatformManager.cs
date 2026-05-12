using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyPeasyFirstPersonController;
using Unity.AI.Navigation;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    public Transform spawnPoint;
    public Transform[] patrolPoints;
}

[System.Serializable]
public class PlatformLevel
{
    public float stopHeight;
    public EnemySpawnData[] enemies;
    public GameObject[] levelGeometry;
    public ServerRackFormation rackFormation;
    public Transform checkpointPosition;   // Where player respawns on death
}

public class PlatformManager : MonoBehaviour
{
    [Header("Platform Settings")]
    public float riseSpeed = 2f;
    public float slowRiseSpeed = 0.5f;

    public GameObject globalAttackUI;

    [Header("Platform Mesh")]
    public Renderer platformRenderer;

    [Header("Levels")]
    public PlatformLevel[] levels;

    [Header("References")]
    public string playerTag = "Player";
    public ComputerTrigger computerTrigger;

    private int _currentLevel = -1;
    private bool _isRising = false;
    private bool _isStopped = false;
    private bool _isSinking = false;
    private List<GameObject> _activeEnemies = new List<GameObject>();

    private CharacterController _playerController;
    private FirstPersonController _playerFPC;
    private Transform _playerTransform;
    private bool _playerOnPlatform = false;
    private NavMeshSurface _navMeshSurface;

    void Awake()
    {
        _navMeshSurface = GetComponentInChildren<NavMeshSurface>();
        _navMeshSurface.BuildNavMesh();
    }

    void Start()
    {
        if (platformRenderer != null)
            platformRenderer.enabled = false;
    }

    public void StartRising()
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            _playerOnPlatform = true;
            _playerTransform = player.transform;
            _playerController = player.GetComponentInChildren<CharacterController>();
            _playerFPC = player.GetComponent<FirstPersonController>();
            if (_playerFPC != null) _playerFPC.onMovingPlatform = true;
        }

        _isRising = true;
        _isStopped = false;
    }

    void Update()
    {
        if (!_isRising || _isStopped || _isSinking) return;

        int nextLevel = _currentLevel + 1;
        if (nextLevel < levels.Length)
        {
            if (transform.position.y >= levels[nextLevel].stopHeight)
            {
                StopAtLevel(nextLevel);
                return;
            }
        }

        float speed = _currentLevel == -1 ? riseSpeed : slowRiseSpeed;
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        if (_playerOnPlatform && _playerController != null)
            _playerController.Move(Vector3.up * speed * Time.deltaTime);
    }

    private void StopAtLevel(int levelIndex)
    {
        _isStopped = true;
        _playerOnPlatform = false;
        if (_playerFPC != null) _playerFPC.onMovingPlatform = false;
        _currentLevel = levelIndex;

        Vector3 pos = transform.position;
        pos.y = levels[levelIndex].stopHeight;
        transform.position = pos;

        // Release player lock when platform stops
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            PlayerLock playerLock = player.GetComponent<PlayerLock>();
            if (playerLock != null)
            playerLock.Release();
        }

        _navMeshSurface.BuildNavMesh();

        if (levels[levelIndex].rackFormation != null)
            levels[levelIndex].rackFormation.RiseAll();

        PlatformLevel level = levels[levelIndex];
        foreach (EnemySpawnData data in level.enemies)
        {
            if (data.prefab == null || data.spawnPoint == null) continue;

            GameObject enemy = Instantiate(data.prefab, data.spawnPoint.position, data.spawnPoint.rotation);

            AIController ai = enemy.GetComponent<AIController>();
            if (ai != null)
            {
                ai.player = _playerTransform;
                ai.controller = _playerFPC;
                ai.patrolPoints = data.patrolPoints;
                ai.attackUI = this.globalAttackUI;
            }

            _activeEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerOnPlatform = false;
            if (_playerFPC != null) _playerFPC.onMovingPlatform = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerOnPlatform = true;
            if (_playerFPC != null) _playerFPC.onMovingPlatform = true;
        }
    }

    public void RespawnPlayer()
    {
        if (_currentLevel < 0) return;

        Transform checkpoint = levels[_currentLevel].checkpointPosition;
        if (checkpoint == null) return;

        // Teleport player to checkpoint
        if (_playerController != null)
        {
            _playerController.enabled = false;
            _playerTransform.position = checkpoint.position;
            _playerController.enabled = true;
        }

        // Resume player control
        if (_playerFPC != null)
            _playerFPC.ExitUIMode();

        // Reset all active enemies back to patrol
        foreach (GameObject enemy in _activeEnemies)
        {
            if (enemy == null) continue;
            AIController ai = enemy.GetComponent<AIController>();
            if (ai != null)
                ai.ResetToPatrol();
        }
    }

    public void OnPlayerReachedComputer()
    {

        foreach (GameObject enemy in _activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        _activeEnemies.Clear();

        if (_currentLevel >= 0)
        {
            foreach (GameObject geo in levels[_currentLevel].levelGeometry)
            {
                if (geo != null)
                    geo.SetActive(false);
            }
        }

        StartCoroutine(SinkThenRise());
    }

    private IEnumerator SinkThenRise()
    {
        _isSinking = true;

        if (_currentLevel >= 0 && levels[_currentLevel].rackFormation != null)
            yield return StartCoroutine(levels[_currentLevel].rackFormation.SinkAllAndWait());

        if (computerTrigger != null)
            computerTrigger.Reset();

        _isSinking = false;
        _isStopped = false;
        _isRising = true;
    }
}