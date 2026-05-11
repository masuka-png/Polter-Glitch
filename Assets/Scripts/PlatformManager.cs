using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyPeasyFirstPersonController;

public class PlatformManager : MonoBehaviour
{
    [System.Serializable]
    public class PlatformLevel
    {
        public float stopHeight;
        public GameObject[] enemyPrefabs;
        public Transform[] spawnPoints;
        public GameObject[] levelGeometry;
        public ServerRackFormation rackFormation;  // Formation for this level
    }

    [Header("Platform Settings")]
    public float riseSpeed = 2f;
    public float slowRiseSpeed = 0.5f;

    [Header("Platform Mesh")]
    public Renderer platformRenderer;              // Drag the platform mesh renderer here

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
    private bool _playerOnPlatform = false;

    void Start()
    {
        // Hide the platform mesh
        if (platformRenderer != null)
            platformRenderer.enabled = false;
    }

    public void StartRising()
    {
        // Grab player reference once when rising starts
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            _playerOnPlatform = true;
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

        // Check if we've reached the next stop
        int nextLevel = _currentLevel + 1;
        if (nextLevel < levels.Length)
        {
            if (transform.position.y >= levels[nextLevel].stopHeight)
            {
                StopAtLevel(nextLevel);
                return;
            }
        }

        // Move platform up
        float speed = _currentLevel == -1 ? riseSpeed : slowRiseSpeed;
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);

        // Carry player
        if (_playerOnPlatform && _playerController != null)
            _playerController.Move(Vector3.up * speed * Time.deltaTime);
    }

    private void StopAtLevel(int levelIndex)
    {
        _isStopped = true;
        _currentLevel = levelIndex;

        // Snap to exact height
        Vector3 pos = transform.position;
        pos.y = levels[levelIndex].stopHeight;
        transform.position = pos;

        // Rise server racks for this level
        if (levels[levelIndex].rackFormation != null)
            levels[levelIndex].rackFormation.RiseAll();

        // Spawn enemies
        PlatformLevel level = levels[levelIndex];
        for (int i = 0; i < level.enemyPrefabs.Length; i++)
        {
            if (i >= level.spawnPoints.Length) break;
            if (level.enemyPrefabs[i] == null || level.spawnPoints[i] == null) continue;

            GameObject enemy = Instantiate(level.enemyPrefabs[i], level.spawnPoints[i].position, level.spawnPoints[i].rotation);
            _activeEnemies.Add(enemy);
        }
    }

    public void OnPlayerReachedComputer()
    {
        // Reset computer trigger so it can fire again
        if (computerTrigger != null)
            computerTrigger.Reset();

        // Despawn all active enemies
        foreach (GameObject enemy in _activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        _activeEnemies.Clear();

        // Despawn current level geometry
        if (_currentLevel >= 0)
        {
            foreach (GameObject geo in levels[_currentLevel].levelGeometry)
            {
                if (geo != null)
                    geo.SetActive(false);
            }
        }

        // Sink racks then resume rising
        StartCoroutine(SinkThenRise());
    }

    private IEnumerator SinkThenRise()
    {
        _isSinking = true;

        if (_currentLevel >= 0 && levels[_currentLevel].rackFormation != null)
            yield return StartCoroutine(levels[_currentLevel].rackFormation.SinkAllAndWait());

        _isSinking = false;
        _isStopped = false;
        _isRising = true;
    }
}