using UnityEngine;
using UnityEngine.SceneManagement;
public class DialogueController : MonoBehaviour
{
    public string nextSceneName;  
    
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
