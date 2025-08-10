using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class SceneChangeLogger : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
        void OnEnable()
    {
        SceneManager.activeSceneChanged += LogChange;
    }
    void OnDisable()
    {
        SceneManager.activeSceneChanged -= LogChange;
    }
    void LogChange(Scene oldS, Scene newS)
    {
        UnityEngine.Debug.Log($"[Logger] Scene changed to {newS.name}\n{new StackTrace(true)}");
    }
}
