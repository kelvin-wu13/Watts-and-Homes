using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    public GameObject levelPopupPanel;
    public GameObject blockerPanel;
    public TextMeshProUGUI levelTitleText;
    public Transform popupObjectiveContainer;
    public GameObject objectiveItemPrefab;

    public Sprite completedSprite;
    public Sprite incompleteSprite;

    private HouseController selectedHouse;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        levelPopupPanel.SetActive(false);
        blockerPanel.SetActive(false);
    }

    public void ShowLevelPopup(HouseController house)
    {
        selectedHouse = house;
        levelTitleText.text = $"Level {house.levelData.levelIndex + 1}";

        foreach (Transform child in popupObjectiveContainer) Destroy(child.gameObject);
        int stars = GameProgress.GetStars(house.levelData.levelIndex);

        for (int i = 0; i < house.levelData.objectiveDescriptions.Count; i++)
        {
            GameObject itemUI_GO = Instantiate(objectiveItemPrefab, popupObjectiveContainer);
            ObjectiveItemUI uiController = itemUI_GO.GetComponent<ObjectiveItemUI>();
            if (uiController != null)
            {
                uiController.objectiveText.text = house.levelData.objectiveDescriptions[i];
                bool isComplete = i < stars;
                uiController.statusIcon.sprite = isComplete ? completedSprite : incompleteSprite;
            }
        }

        levelPopupPanel.SetActive(true);
        blockerPanel.SetActive(true);
    }

    public void CloseLevelPopup()
    {
        levelPopupPanel.SetActive(false);
        blockerPanel.SetActive(false);
        selectedHouse = null;
    }

    public void OnClick_PlayLevel()
    {
        if (selectedHouse != null)
        {
            SceneManager.LoadScene(selectedHouse.levelData.sceneNameToLoad);
        }
    }
}
