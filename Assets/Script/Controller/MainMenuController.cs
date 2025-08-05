using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("IntroDialogue1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
