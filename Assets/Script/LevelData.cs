using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public string sceneNameToLoad;
    public List<string> objectiveDescriptions;
}