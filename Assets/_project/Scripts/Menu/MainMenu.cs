using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private UIDocument _document;

    void OnEnable()
    {
        var root = _document.rootVisualElement;

        root.Q<Button>("playBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Gameplay"));
        root.Q<Button>("storyBtn").RegisterCallback<ClickEvent>(e => SceneManager.LoadScene("Intro"));
        root.Q<Button>("exitBtn").RegisterCallback<ClickEvent>(OnExitButtonPressed);
    }

    public void OnExitButtonPressed(ClickEvent e)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}