using UnityEngine;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public enum GameState { Normal, PlacingCable }

    public static GameState currentState;

    void Start()
    {
        SetState(GameState.Normal);
        ShowTutorialAtStart();
    }

    private void ShowTutorialAtStart()
    {
        var settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager != null)
        {
            settingsManager.OpenTutorial();
        }
    }

    public void StartCablePlacement()
    {
        SetState(GameState.PlacingCable);
        CablePlacer.Instance.StartPlacingCable();
    }

    public static void SetState(GameState newState)
    {
        currentState = newState;

        StackTrace stackTrace = new StackTrace();
        string callingMethod = stackTrace.GetFrame(1).GetMethod().Name;
        string callingClass = stackTrace.GetFrame(1).GetMethod().DeclaringType.Name;
    }
}
