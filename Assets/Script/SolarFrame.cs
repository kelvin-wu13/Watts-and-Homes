using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SolarFrame : MonoBehaviour
{
    [Header("Raycast Settings")]
    public LayerMask slotLayerMask = -1;
    public float snapDistance = 2f;

    private PowerCellSlot[] slots;

    private void Awake()
    {
        InitializeFrame();
    }

    private void InitializeFrame()
    {
        slots = GetComponentsInChildren<PowerCellSlot>();

        foreach (PowerCellSlot slot in slots)
        {
            slot.Initialize(this, 0, 0);
        }

        UpdatePowerDisplay();
    }

    public PowerCellSlot FindNearestEmptySlot(Vector3 worldPosition)
    {
        PowerCellSlot nearestSlot = null;
        float nearestDistance = float.MaxValue;

        foreach (PowerCellSlot slot in slots)
        {
            if (slot != null && !slot.isOccupied)
            {
                float distance = Vector3.Distance(worldPosition, slot.transform.position);
                if (distance <= snapDistance && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSlot = slot;
                }
            }
        }

        return nearestSlot;
    }

    public void OnPowerCellPlaced()
    {
        UpdatePowerDisplay();
        UpdateConnectedTargets();
    }

    public void OnPowerCellRemoved()
    {
        UpdatePowerDisplay();
        UpdateConnectedTargets();
    }

    private void UpdatePowerDisplay()
    {
        float totalGeneration = 0f;
        float totalConsumption = 0f;

        foreach (PowerCellSlot slot in slots)
        {
            PowerCell powerCell = slot.GetPowerCell();
            if (powerCell != null && powerCell.itemData != null)
            {
                switch (powerCell.itemData.itemType)
                {
                    case ItemType.Generator:
                        totalGeneration += powerCell.itemData.powerGeneration;
                        break;
                    case ItemType.Consumer:
                        totalConsumption += powerCell.itemData.powerRequirement;
                        break;
                    case ItemType.Conductor:
                        totalGeneration += powerCell.itemData.powerGeneration;
                        totalConsumption += powerCell.itemData.powerRequirement;
                        break;
                }
            }
        }

        float netPower = totalGeneration - totalConsumption;
    }

    private void UpdateConnectedTargets()
    {
        ConnectionPoint frameConnectionPoint = GetComponent<ConnectionPoint>();
        if (frameConnectionPoint == null || !frameConnectionPoint.isConnected)
            return;

        GameObject connectedCable = frameConnectionPoint.connectedCable;
        if (connectedCable == null)
            return;

        CableInteractable cableInteractable = connectedCable.GetComponent<CableInteractable>();
        if (cableInteractable == null)
            return;

        Target connectedTarget = FindConnectedTarget(frameConnectionPoint, connectedCable);
        if (connectedTarget != null)
        {
            float currentPower = GetNetPowerOutput();
            connectedTarget.UpdatePower(currentPower);
        }
    }

    private Target FindConnectedTarget(ConnectionPoint frameConnection, GameObject cable)
    {
        ConnectionPoint[] allConnectionPoints = FindObjectsOfType<ConnectionPoint>();

        foreach (ConnectionPoint cp in allConnectionPoints)
        {
            if (cp != frameConnection && cp.connectedCable == cable)
            {
                Target target = cp.GetComponent<Target>();
                if (target == null)
                {
                    target = cp.GetComponentInParent<Target>();
                }

                if (target != null)
                {
                    return target;
                }
            }
        }

        return null;
    }
    public float GetNetPowerOutput()
    {
        float totalGeneration = 0f;
        float totalConsumption = 0f;

        foreach (PowerCellSlot slot in slots)
        {
            PowerCell powerCell = slot.GetPowerCell();
            if (powerCell != null && powerCell.itemData != null)
            {
                switch (powerCell.itemData.itemType)
                {
                    case ItemType.Generator:
                        totalGeneration += powerCell.itemData.powerGeneration;
                        break;
                    case ItemType.Consumer:
                        totalConsumption += powerCell.itemData.powerRequirement;
                        break;
                    case ItemType.Conductor:
                        totalGeneration += powerCell.itemData.powerGeneration;
                        totalConsumption += powerCell.itemData.powerRequirement;
                        break;
                }
            }
        }

        return Mathf.Max(0f, totalGeneration - totalConsumption);
    }

    public bool HasEmptySlots()
    {
        foreach (PowerCellSlot slot in slots)
        {
            if (!slot.isOccupied)
                return true;
        }
        return false;
    }

    public List<PowerCell> GetAllPowerCells()
    {
        List<PowerCell> powerCells = new List<PowerCell>();
        foreach (PowerCellSlot slot in slots)
        {
            PowerCell powerCell = slot.GetPowerCell();
            if (powerCell != null)
            {
                powerCells.Add(powerCell);
            }
        }
        return powerCells;
    }
}