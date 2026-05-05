using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(LoadScene("Gameplay"));
    }

    public void OpenChronicles()
    {
        StartCoroutine(LoadScene("Intro"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    System.Collections.IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f); // for animation
        SceneManager.LoadScene(sceneName);
    }
}