using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _health_bar_player;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Button _retryButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Awake is the EARLIEST point — hide before anything else runs
        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(false);
        else
            Debug.LogError("[GameOverManager] _gameOverPanel is NOT assigned in Inspector!", this);
    }

  
    public void ShowGameOver()
    {
        Debug.Log("[GameOverManager] ShowGameOver called");

        if (_gameOverPanel != null)
            _gameOverPanel.SetActive(true);

        if (_gameOverText != null)
            _gameOverText.text = "GAME OVER\nYou Lose";
    }

    /*private void OnRetryClicked()
    {
        Debug.Log("[GameOverManager] Retry clicked — reloading scene");

        Instance = null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }*/

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameOverPanel.SetActive(false);
        _health_bar_player.SetActive(false);
        SceneManager.LoadScene("Game Menu");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}