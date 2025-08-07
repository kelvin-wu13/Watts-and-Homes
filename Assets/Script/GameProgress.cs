using UnityEngine;

public static class GameProgress
{
    private const string UNLOCKED_LEVEL_KEY = "UnlockedLevel";
    private const string LEVEL_STARS_KEY_PREFIX = "LevelStars_";

    public static void UnlockNextLevel(int currentLevelIndex)
    {
        int highestUnlocked = GetHighestUnlockedLevel();
        if (currentLevelIndex + 1 > highestUnlocked)
        {
            PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, currentLevelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public static int GetHighestUnlockedLevel()
    {
        return PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 0);
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex <= GetHighestUnlockedLevel();
    }

    public static void SaveStars(int levelIndex, int starCount)
    {
        if (starCount > GetStars(levelIndex))
        {
            PlayerPrefs.SetInt(LEVEL_STARS_KEY_PREFIX + levelIndex, starCount);
            PlayerPrefs.Save();
        }
    }

    public static int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(LEVEL_STARS_KEY_PREFIX + levelIndex, 0);
    }
    //public static void ResetAllProgress()
    //{
    //    PlayerPrefs.DeleteAll();
    //    Debug.Log("Semua progres game telah direset.");
    //}
}
