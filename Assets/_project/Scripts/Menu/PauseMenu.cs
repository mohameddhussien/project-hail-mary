using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
   
    public GameObject container;

    void Start()
    {
       
        container.SetActive(false);

        
        Time.timeScale = 1f;
    }

    void Update()
    {

        Debug.Log("Update is running"); 
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Escape pressed!");
            if (container.activeSelf)
                ResumeBtn();
            else
                Pause();
        }
    }

    public void Pause()
    {
        container.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeBtn()
    {
        container.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void MainMenuBtn()
    {

        container.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game Menu");
    }

    public void ExitBtn()
    {
        
        Time.timeScale = 1f;

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}