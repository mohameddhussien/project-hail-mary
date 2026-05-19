using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action<GameState> GameStateChanged = delegate { };

    public GameState GameState { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this); // destroy component only, not the GameObject
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
    }

    void SetGameState(GameState gameState)
    {
        if (gameState == GameState) return;
        GameState = gameState;
        GameStateChanged(gameState);
    }

    void OnEnable()
    {
        
        GameState = GameState.GameOver;
        SetGameState(GameState.Patrol);
        MusicManager.Instance?.PlayPatrolMusic();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F1))
            Time.timeScale = 0f;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible
                ? CursorLockMode.None
                : CursorLockMode.Confined;
        }
    }

    public void InCombat(bool inCombat)
    {
        if (inCombat)
        {
            if (GameState == GameState.Combat) return;
            MusicManager.Instance?.PlayCombatMusic();
            SetGameState(GameState.Combat);
        }
        else
        {
            if (GameState != GameState.Combat) return;
            MusicManager.Instance?.PlayPatrolMusic();
            SetGameState(GameState.Patrol);
        }
    }

    public void PlayerWon()
    {
        MusicManager.Instance?.PlayGameOverMusic();
        SetGameState(GameState.GameOver);
    }

   
    public void ResetState()
    {
        // Guarantee we are in GameOver so AddPoints stays blocked
        // during the scene unload / old-object destruction phase.
        GameState = GameState.GameOver;

        // Register for exactly one scene load, then self-remove.
        SceneManager.sceneLoaded += OnRetrySceneLoaded;
    }

    private void OnRetrySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnRetrySceneLoaded;

        // Now it is safe to zero the score — nothing left can add to it.
        ScoreManager.Instance?.ResetScore();

        // Transition to Patrol; force the value to differ first so
        // SetGameState always broadcasts (same trick as OnEnable).
        GameState = GameState.GameOver;
        SetGameState(GameState.Patrol);
        MusicManager.Instance?.PlayPatrolMusic();

        // Restore cursor (mirrors GameOverPanel.TryAgain cursor reset,
        // but this is the authoritative place since it runs after load).
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }


    void OnDestroy()
    {
        Debug.Log(
            $"[GameManager] OnDestroy! Am I the singleton? {Instance == this}\n" +
            System.Environment.StackTrace,
            this
        );
    }
}