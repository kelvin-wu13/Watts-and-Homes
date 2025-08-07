using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }

    public List<Objective> objectives = new List<Objective>();

    [SerializeField] private GameObject objectivePanel;
    [SerializeField] private Transform objectiveContainer;
    [SerializeField] private GameObject objectiveItemPrefab;

    [SerializeField] private Sprite incompleteSprite;
    [SerializeField] private Sprite completedSprite;

    private bool allObjectivesCompleted = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (objectives.Count > 0)
        {
            CreateObjectiveUI();
        }
        else if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }
    }

    private void CreateObjectiveUI()
    {
        if (objectivePanel == null || objectiveContainer == null || objectiveItemPrefab == null) return;

        foreach (Transform child in objectiveContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var objective in objectives)
        {
            GameObject itemUI_GO = Instantiate(objectiveItemPrefab, objectiveContainer);
            ObjectiveItemUI uiController = itemUI_GO.GetComponent<ObjectiveItemUI>();

            objective.uiElement = itemUI_GO;

            if (uiController != null)
            {
                uiController.objectiveText.text = objective.description;
                uiController.statusIcon.sprite = incompleteSprite;
                uiController.statusIcon.color = Color.white;
            }
        }
        objectivePanel.SetActive(true);
    }

    public void CheckAllObjectives()
    {
        if (allObjectivesCompleted) return;

        foreach (var objective in objectives)
        {
            objective.CheckObjective();
        }

        bool allComplete = true;
        foreach (var objective in objectives)
        {
            UpdateSingleObjectiveUI(objective);
            if (!objective.IsComplete)
            {
                allComplete = false;
            }
        }
        UIManager.Instance.ShowResultPanel(true, objectives);
    }

    private void UpdateSingleObjectiveUI(Objective objective)
    {
        if (objective.uiElement == null) return;

        ObjectiveItemUI uiController = objective.uiElement.GetComponent<ObjectiveItemUI>();
        if (uiController == null) return;

        if (objective.IsComplete)
        {
            uiController.statusIcon.sprite = completedSprite;
            uiController.statusIcon.color = Color.green;
            uiController.objectiveText.color = Color.green;
        }
        else
        {
            uiController.statusIcon.sprite = incompleteSprite;
            uiController.statusIcon.color = Color.white;
            uiController.objectiveText.color = Color.white;
        }
    }
}
