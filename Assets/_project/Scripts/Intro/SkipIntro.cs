using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SkipIntro : MonoBehaviour
{
    [SerializeField] private PlayableDirector _timeline;
    private string _gameMenuSceneName = "Game Menu";

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            skipIntro();
        }
    }

    private void skipIntro()
    {
        if (_timeline != null)
            _timeline.Stop();

        SceneManager.LoadScene(_gameMenuSceneName);
    }
}