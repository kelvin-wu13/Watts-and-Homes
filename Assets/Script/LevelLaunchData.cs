using UnityEngine;

public static class LevelLaunchData
{
    public static LevelData LevelData;

    public static bool IsValid => LevelData != null;

    public static void SetFromHouse(HouseController house)
    {
        LevelData = (house != null) ? house.levelData : null;
    }

    public static void Clear()
    {
        LevelData = null;
    }
}
