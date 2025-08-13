using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject objectivePanel;
    public GameObject blockerPanel;
    public GameObject resultPanel;
    public TextMeshProUGUI titleText;
    public Transform objectiveListContainer;
    public Button nextLevelButton;
    public Button restartButton;

    public GameObject popupObjectiveItemPrefab;
    public Sprite completedSprite;
    public Sprite incompleteSprite;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        if (resultPanel != null) resultPanel.SetActive(false);
        if (blockerPanel != null) resultPanel.SetActive(false);
    }

    public void ShowResultPanel(bool wasSuccessful, List<Objective> finalObjectives)
    {
        if (resultPanel == null) return;

        blockerPanel.SetActive(true);

        PopulatePopupObjectiveList(objectiveListContainer, finalObjectives);

        objectivePanel.SetActive(false);
        resultPanel.SetActive(true);
    }
    private void PopulatePopupObjectiveList(Transform container, List<Objective> objectives)
    {
        if (container == null || popupObjectiveItemPrefab == null) return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        foreach (Objective obj in objectives)
        {
            GameObject itemUI_GO = Instantiate(popupObjectiveItemPrefab, container);
            ObjectiveItemUI uiController = itemUI_GO.GetComponent<ObjectiveItemUI>();

            if (uiController != null)
            {
                uiController.objectiveText.text = obj.description;

                if (obj.IsComplete)
                {
                    uiController.statusIcon.sprite = completedSprite;
                }
                else
                {
                    uiController.statusIcon.sprite = incompleteSprite;
                }
            }
        }
    }

    public void OnClick_RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClick_NextLevel()
    {
        SceneManager.LoadScene("Map");
    }
}
