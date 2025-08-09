using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Popup UI")]
    public GameObject levelPopupPanel;
    public TextMeshProUGUI levelTitleText;
    public Transform popupObjectiveContainer;
    public GameObject objectiveItemPrefab;
    public Button blockerButton;

    [Header("Sprites for objectives")]
    public Sprite completedSprite;
    public Sprite incompleteSprite;

    [Header("One-time Dialogs")]
    public DialogueTrigger mapIntroDialogue;
    public int totalLevels = 5;   

    private HouseController houseForPopup;
    private HouseController selectedHouse;
    public string levelLobbySceneName = "LevelLobby";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (levelPopupPanel) levelPopupPanel.SetActive(false);
        if (blockerButton)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.gameObject.SetActive(false);
            var img = blockerButton.GetComponent<Image>();
            if (img) img.raycastTarget = true;
        }
    }

    private void OnEnable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
        }
    }

    private void Start()
    {
        RefreshAllHouses();

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsRunning)
        {
            DialogueManager.Instance.OnDialogueEnd += OnMapEntryDialogueFinished;
        }
        else
        {
            TryPlayPendingPostDialogue();
        }
    }
    private void OnMapEntryDialogueFinished()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnd -= OnMapEntryDialogueFinished;

        TryPlayPendingPostDialogue();
    }

    public void PrepareLevelPopup(HouseController house)
    {
        houseForPopup = house;
        if (DialogueManager.Instance == null || !DialogueManager.Instance.IsRunning)
        {
            ShowLevelPopup(houseForPopup);
        }
    }

    private void HandleDialogueEnd()
    {
        if (houseForPopup != null)
        {
            ShowLevelPopup(houseForPopup);
        }
    }

    private void OnMapIntroFinished()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnd -= OnMapIntroFinished;

        GameProgress.MarkMapIntroSeen();
        TryPlayPendingPostDialogue();
    }

    private void TryPlayPendingPostDialogue()
    {
        int pending = GameProgress.ConsumePendingPostLevel();
        if (pending < 0) return;

        var house = FindHouseByLevelIndex(pending);
        if (house == null || house.levelPostDialogue == null) return;

        if (!GameProgress.HasSeenHousePost(pending)
            && house.levelPostDialogue.dialogueLines != null
            && house.levelPostDialogue.dialogueLines.Count > 0)
        {
            houseForPopup = null;

            DialogueManager.Instance.OnDialogueEnd += () =>
            {
                GameProgress.MarkHousePostSeen(pending);
            };
            house.levelPostDialogue.TriggerDialogue();
        }
    }

    private HouseController FindHouseByLevelIndex(int lv)
    {
        var all = GameObject.FindObjectsOfType<HouseController>(true);
        foreach (var h in all)
            if (h && h.levelData && h.levelData.levelIndex == lv) return h;
        return null;
    }
    private void RefreshAllHouses()
    {
        var allHouses = GameObject.FindObjectsOfType<HouseController>(true);
        foreach (var h in allHouses)
        {
            if (h != null && h.levelData != null)
            {
                h.UpdateVisualState();
            }
        }
    }
    public void ShowLevelPopup(HouseController house)
    {
        if (house == null || house.levelData == null) return;

        selectedHouse = house;

        int levelIndex = house.levelData.levelIndex;
        if (levelTitleText) levelTitleText.text = $"Level {levelIndex + 1}";

        if (popupObjectiveContainer)
        {
            for (int i = popupObjectiveContainer.childCount - 1; i >= 0; i--)
                Destroy(popupObjectiveContainer.GetChild(i).gameObject);
        }

        var objectives = house.levelData.objectiveDescriptions;
        if (objectives == null)
        {
            objectives = new System.Collections.Generic.List<string>();
        }
        int stars = GameProgress.GetStars(levelIndex);

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectiveItemPrefab == null || popupObjectiveContainer == null) break;
            var go = Instantiate(objectiveItemPrefab, popupObjectiveContainer);

            var ui = go.GetComponent<ObjectiveItemUI>();
            if (ui != null)
            {
                if (ui.objectiveText) ui.objectiveText.text = objectives[i];
                bool isComplete = i < stars;
                if (ui.statusIcon && completedSprite && incompleteSprite)
                    ui.statusIcon.sprite = isComplete ? completedSprite : incompleteSprite;
            }
        }

        if (levelPopupPanel) levelPopupPanel.SetActive(true);
        if (blockerButton != null)
        {
            blockerButton.onClick.RemoveAllListeners();
            blockerButton.onClick.AddListener(CloseLevelPopup);
            blockerButton.gameObject.SetActive(true);
            var img = blockerButton.GetComponent<Image>();
            if (img) img.raycastTarget = true;
        }
    }

    public void CloseLevelPopup()
    {
        if (levelPopupPanel) levelPopupPanel.SetActive(false);
        if (blockerButton)
        {
            blockerButton.gameObject.SetActive(false);
            var img = blockerButton.GetComponent<Image>();
            if (img) img.raycastTarget = false;
        }
        houseForPopup = null;
    }

    public void OnClick_PlayLevel()
    {
        if (selectedHouse != null && selectedHouse.levelData != null)
        {
            SceneManager.LoadScene(selectedHouse.levelData.sceneNameToLoad);
        }
    }
    public void OnClick_OpenLevelLobby()
    {
        if (selectedHouse == null || selectedHouse.levelData == null) return;

        LevelLaunchData.SetFromHouse(selectedHouse);
        SceneManager.LoadScene(levelLobbySceneName);
    }

}
