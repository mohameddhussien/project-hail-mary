using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private bool ShouldQuitGame => Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (ShouldQuitGame)
        {
            ShouldGame();
        }
    }

    private void ShouldGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
