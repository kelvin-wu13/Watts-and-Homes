using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelLobbyUI : MonoBehaviour
{
    [Header("UI Refs")]
    public TextMeshProUGUI levelTitle;
    public Transform objectiveContainer;
    public GameObject objectiveItemPrefab; 
    public Image[] starPreview;  
    public Sprite starOn;
    public Sprite starOff;

    [Header("Buttons")]
    public Button startButton;
    public Button backButton;

    private LevelData data;

    private void Start()
    {
        if (!LevelLaunchData.IsValid)
        {
            SceneManager.LoadScene("Map");
            return;
        }

        data = LevelLaunchData.LevelData;

        if (levelTitle) levelTitle.text = $"Level {data.levelIndex + 1}";
        BuildObjectives();
        BuildStarPreview();

        if (startButton) startButton.onClick.AddListener(StartLevel);
        if (backButton) backButton.onClick.AddListener(BackToMap);
    }

    private void BuildObjectives()
    {
        if (!objectiveContainer || !objectiveItemPrefab) return;
        foreach (Transform c in objectiveContainer) Destroy(c.gameObject);

        var list = data.objectiveDescriptions ?? new System.Collections.Generic.List<string>();
        foreach (var desc in list)
        {
            var go = Instantiate(objectiveItemPrefab, objectiveContainer);
            var ui = go.GetComponent<ObjectiveItemUI>();
            if (ui && ui.objectiveText) ui.objectiveText.text = desc;

            if (ui && ui.statusIcon && starOff) ui.statusIcon.sprite = starOff;
        }
    }

    private void BuildStarPreview()
    {
        int stars = GameProgress.GetStars(data.levelIndex);
        for (int i = 0; i < starPreview.Length; i++)
        {
            if (!starPreview[i]) continue;
            starPreview[i].sprite = (i < stars && starOn) ? starOn : starOff;
        }
    }

    private void StartLevel()
    {
        SceneManager.LoadScene(data.sceneNameToLoad);
    }

    private void BackToMap()
    {
        LevelLaunchData.Clear();
        SceneManager.LoadScene("Map");
    }
}
