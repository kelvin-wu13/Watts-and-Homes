using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject objectivePanel;
    public GameObject blockerPanel;
    public GameObject resultPanel;

    public Transform objectiveListContainer;
    public Button nextLevelButton;
    public Button restartButton;

    public GameObject popupObjectiveItemPrefab;

    public Sprite completedSprite;
    public Sprite incompleteSprite;

    public Image resultStatusImage;
    public Sprite successSprite;
    public Sprite failSprite;

    [SerializeField] float resultDelay = 1.5f;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        if (resultPanel != null) resultPanel.SetActive(false);
        if (blockerPanel != null) blockerPanel.SetActive(false);
    }

    public void ShowResultPanel(bool wasSuccessful, List<Objective> finalObjectives)
    {
        if (resultPanel == null) return;

        int completed = 0;
        if (finalObjectives != null)
        {
            for (int i = 0; i < finalObjectives.Count; i++)
                if (finalObjectives[i] != null && finalObjectives[i].IsComplete)
                    completed++;
        }
        bool success = completed >= 1;

        if (resultStatusImage != null)
        {
            resultStatusImage.sprite = success ? successSprite : failSprite;
            resultStatusImage.enabled = (resultStatusImage.sprite != null);
        }

        PopulatePopupObjectiveList(objectiveListContainer, finalObjectives);

        if (blockerPanel) blockerPanel.SetActive(true);
        if (objectivePanel) objectivePanel.SetActive(false);
        resultPanel.SetActive(true);
    }
    public void ShowResultPanelDelayed(float delay, bool wasSuccessful, List<Objective> finalObjectives)
    {
        StartCoroutine(ShowResultPanelDelayedCo(delay, wasSuccessful, finalObjectives));
    }

    private IEnumerator ShowResultPanelDelayedCo(float delay, bool wasSuccessful, List<Objective> finalObjectives)
    {
        yield return new WaitForSeconds(delay);
        ShowResultPanel(wasSuccessful, finalObjectives);
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
