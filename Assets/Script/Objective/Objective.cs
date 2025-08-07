using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    public string description;
    public bool IsComplete { get; protected set; } = false;

    public GameObject uiElement;

    public abstract void CheckObjective();
}
