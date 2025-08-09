using UnityEngine;

public static class GameProgress
{
    private const string UnlockedLevelKey = "UnlockedLevel";
    private static string StarsKey(int lv) => $"LevelStars_{lv}";

    private const string MapIntroSeenKey = "MapIntroSeen";
    private static string HouseIntroSeenKey(int lv) => $"House_{lv}_IntroSeen";
    private static string HousePostSeenKey(int lv) => $"House_{lv}_PostSeen";

    private const string PendingPostLevelKey = "PendingPostLevel";
    public static int GetUnlockedLevel() => PlayerPrefs.GetInt("UnlockedLevel", 0);

    public static bool IsLevelUnlocked(int lv) => lv <= PlayerPrefs.GetInt(UnlockedLevelKey, 0);
    public static void UnlockLevel(int lv) { if (lv > PlayerPrefs.GetInt(UnlockedLevelKey, 0)) { PlayerPrefs.SetInt(UnlockedLevelKey, lv); PlayerPrefs.Save(); } }
    public static int GetStars(int lv) => PlayerPrefs.GetInt(StarsKey(lv), 0);
    public static void SaveStars(int lv, int stars) { PlayerPrefs.SetInt(StarsKey(lv), Mathf.Clamp(stars, 0, 3)); PlayerPrefs.Save(); }

    public static bool HasSeenMapIntro() => PlayerPrefs.GetInt(MapIntroSeenKey, 0) == 1;
    public static void MarkMapIntroSeen() { PlayerPrefs.SetInt(MapIntroSeenKey, 1); PlayerPrefs.Save(); }

    public static bool HasSeenHouseIntro(int lv) => PlayerPrefs.GetInt(HouseIntroSeenKey(lv), 0) == 1;
    public static void MarkHouseIntroSeen(int lv) { PlayerPrefs.SetInt(HouseIntroSeenKey(lv), 1); PlayerPrefs.Save(); }

    public static bool HasSeenHousePost(int lv) => PlayerPrefs.GetInt(HousePostSeenKey(lv), 0) == 1;
    public static void MarkHousePostSeen(int lv) { PlayerPrefs.SetInt(HousePostSeenKey(lv), 1); PlayerPrefs.Save(); }

    public static void SetPendingPostLevel(int lv) { PlayerPrefs.SetInt(PendingPostLevelKey, lv); PlayerPrefs.Save(); }
    public static int ConsumePendingPostLevel() { int v = PlayerPrefs.GetInt(PendingPostLevelKey, -1); if (v != -1) { PlayerPrefs.SetInt(PendingPostLevelKey, -1); PlayerPrefs.Save(); } return v; }

    public static bool HasAnySave() => PlayerPrefs.HasKey(UnlockedLevelKey) || PlayerPrefs.HasKey(MapIntroSeenKey);

    public static int GetTotalStarsEarned(int maxLevels)
    {
        int sum = 0;
        for (int i = 0; i < maxLevels; i++) sum += GetStars(i);
        return sum;
    }
    public static void ClearDialogueKey(string key)
    {
        if (!string.IsNullOrEmpty(key)) PlayerPrefs.DeleteKey(key);
    }
    public static void ResetAllProgress(int maxLevels)
    {
        PlayerPrefs.DeleteKey(UnlockedLevelKey);
        for (int i = 0; i < maxLevels; i++) PlayerPrefs.DeleteKey(StarsKey(i));

        PlayerPrefs.DeleteKey(MapIntroSeenKey);
        for (int i = 0; i < maxLevels; i++)
        {
            PlayerPrefs.DeleteKey(HouseIntroSeenKey(i));
            PlayerPrefs.DeleteKey(HousePostSeenKey(i));
        }
        PlayerPrefs.DeleteKey(PendingPostLevelKey);

        PlayerPrefs.DeleteKey("MapIntro");

        PlayerPrefs.Save();
    }
    public static void InitializeNewGame()
    {
        PlayerPrefs.SetInt(UnlockedLevelKey, 0);
        PlayerPrefs.DeleteKey("MapIntro");
        PlayerPrefs.Save();
    }
}
