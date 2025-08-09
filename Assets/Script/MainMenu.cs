using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string mapSceneName = "Map";

    public int totalLevels = 5;

    public void OnClick_NewJourney()
    {
        GameProgress.ResetAllProgress(totalLevels);
        GameProgress.InitializeNewGame();
        SceneManager.LoadScene(mapSceneName);
    }

    public void OnClick_Continue()
    {
        if (!GameProgress.HasAnySave())
        {
            OnClick_NewJourney();
            return;
        }
        SceneManager.LoadScene(mapSceneName);
    }
    public void OnClick_Quit()
    {
        Application.Quit();
    }
}
