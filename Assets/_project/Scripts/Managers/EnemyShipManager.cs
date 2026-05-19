using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyShipManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyShipPrefabs;
    [SerializeField] private int _maxEnemies = 1;
    [SerializeField] private float _firstSpawnInterval = 10f;
    [SerializeField] private float _spawnInterval = 30f;
    [SerializeField] private float _minSpawnRange = 500f;
    [SerializeField] private float _maxSpawnRange = 2000f;

    private List<EnemyShipController> _enemyShips;
    private float _spawnDelay;
    private Transform _transform;

    private int MaxEnemies { get; set; }
    private float SpawnInterval { get; set; }

    private int ActiveEnemies =>
        _enemyShips.Count(e => e != null && e.gameObject.activeSelf);

    private bool CanSpawnEnemyShip
    {
        get
        {
            _spawnDelay -= Time.deltaTime;
            return _spawnDelay <= 0f && ActiveEnemies < MaxEnemies;
        }
    }

    private GameObject RandomPrefab =>
        _enemyShipPrefabs[Random.Range(0, _enemyShipPrefabs.Length)];

    // -------------------------------------------------------------------------
    // Unity lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        _transform = transform;
        MaxEnemies = _maxEnemies;
        SpawnInterval = _spawnInterval;
    }

    void OnEnable()
    {
        // Fresh list and delay every time this object is enabled
        // (covers both first load and scene reload).
        _enemyShips = new List<EnemyShipController>();
        _spawnDelay = _firstSpawnInterval;
    }

    void Start()
    {
        Debug.Log($"[EnemyShipManager] Start. GameManager exists: {GameManager.Instance != null}");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[EnemyShipManager] GameManager.Instance is null.", this);
            return;
        }

        Debug.Log($"[EnemyShipManager] GameState on start: {GameManager.Instance.GameState}");
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        // Always unsubscribe to prevent stale delegates when the scene reloads.
        if (GameManager.Instance != null)
            GameManager.Instance.GameStateChanged -= OnGameStateChanged;
    }

    void Update()
    {
        if (CanSpawnEnemyShip)
            SpawnEnemyShip();
    }

    // -------------------------------------------------------------------------
    // Spawning
    // -------------------------------------------------------------------------

    void SpawnEnemyShip()
    {
        Debug.Log($"[EnemyShipManager] SpawnEnemyShip called. GameManager: {GameManager.Instance != null}, State: {GameManager.Instance?.GameState}, Active: {ActiveEnemies}/{MaxEnemies}");
        if (GameManager.Instance == null ||
            GameManager.Instance.GameState == GameState.GameOver)
            return;

        Vector3 spawnPosition =
            Random.insideUnitSphere * Random.Range(_minSpawnRange, _maxSpawnRange);

        GameObject enemy = Instantiate(RandomPrefab, _transform);
        enemy.transform.position = spawnPosition;

        var controller = enemy.GetComponent<EnemyShipController>();
        if (controller == null)
        {
            Debug.LogError($"[EnemyShipManager] Prefab '{enemy.name}' has no EnemyShipController.", this);
            Destroy(enemy);
            return;
        }

        controller.ShipDestroyed.AddListener(OnShipDestroyed);
        _enemyShips.Add(controller);

        _spawnDelay = SpawnInterval;
    }

    // -------------------------------------------------------------------------
    // Ship destroyed callback
    // -------------------------------------------------------------------------

    void OnShipDestroyed(int id)
    {
        for (int i = 0; i < _enemyShips.Count; i++)
        {
            var ship = _enemyShips[i];

            if (ship == null || ship.gameObject.GetInstanceID() != id)
                continue;

            // Clean up listener and list entry first.
            ship.ShipDestroyed.RemoveListener(OnShipDestroyed);
            _enemyShips.RemoveAt(i);
            Destroy(ship.gameObject);

            // Difficulty ramp: each kill allows one more simultaneous enemy
            // on the next wave.
            MaxEnemies++;

            // NOTE: Do NOT call GameManager.PlayerWon() here.
            // The game is continuous — only the player dying ends a round.
            // Calling PlayerWon() on every kill was setting GameState.GameOver
            // after the first kill, which stopped score from accumulating and
            // wiped all remaining enemies via OnGameStateChanged.
            return;
        }

        // ID not found ship was already cleaned up (e.g. by OnGameStateChanged).
        // No action needed.
    }

    // -------------------------------------------------------------------------
    // Game state handling
    // -------------------------------------------------------------------------

    void OnGameStateChanged(GameState gameState)
    {
        if (gameState != GameState.GameOver) return;

        // Deactivate all remaining enemies immediately on game over.
        // We iterate in reverse to safely remove while iterating.
        for (int i = _enemyShips.Count - 1; i >= 0; i--)
        {
            var ship = _enemyShips[i];
            if (ship == null) continue;

            ship.ShipDestroyed.RemoveListener(OnShipDestroyed);
            ship.gameObject.SetActive(false);
        }

        _enemyShips.Clear();

        // Reset difficulty so the next round starts at the base value.
        MaxEnemies = _maxEnemies;
    }
}