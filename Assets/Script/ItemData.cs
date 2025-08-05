using System.ComponentModel;
using UnityEngine;

public enum ItemType { Generator, Consumer, Conductor}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public string description;
    public Sprite icon;

    public float powerGeneration;
    public float powerRequirement;
}
