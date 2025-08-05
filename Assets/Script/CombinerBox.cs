using System.Collections.Generic;
using UnityEngine;

public class CombinerBox : MonoBehaviour, IPowerDataProvider
{
    [Header("Item Configuration")]
    public ItemData itemData;

    [Header("Connection Points")]
    [SerializeField] private ConnectionPoint inputConnectionPoint;
    [SerializeField] private ConnectionPoint outputConnectionPoint;

    [Header("Power Management")]
    [SerializeField] private float totalInputPower = 0f;
    [SerializeField] private float remainingPower = 0f;
    [SerializeField] private List<float> outputPowerValues = new List<float>();
    [SerializeField] private List<float> devicePowerRequirements = new List<float>();
    [SerializeField] private float powerEfficiency = 1.0f;

    public System.Action OnConnectionsUpdated;

    private int lastInputConnectionCount = -1;
    private int lastOutputConnectionCount = -1;

    private List<Target> reusableTargetList = new List<Target>();
    private List<CombinerBox> reusableCombinerList = new List<CombinerBox>();
    private List<System.Object> reusableDeviceList = new List<System.Object>();

    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 0.5f;


    private System.Text.StringBuilder debugStringBuilder = new System.Text.StringBuilder(200);

    private void Start()
    {
        ValidateItemData();
        InitializeConnectionPoints();
        UpdatePowerDistribution();
    }
    public string GetPowerDisplayText()
    {
        return $"In: {totalInputPower:F1}";
    }

    private void ValidateItemData()
    {
        if (itemData == null)
        {
            return;
        }

        if (itemData.powerRequirement > 0)
        {
            powerEfficiency = 1.0f - (itemData.powerRequirement / 100f);
            powerEfficiency = Mathf.Clamp01(powerEfficiency);
        }
    }

    private void Update()
    {
        if (Time.time - lastUpdateTime < UPDATE_INTERVAL)
            return;

        if (HasConnectionsChanged())
        {
            UpdatePowerDistribution();
        }

        lastUpdateTime = Time.time;
    }

    private void InitializeConnectionPoints()
    {
        if (inputConnectionPoint != null)
        {
            inputConnectionPoint.parentItem = this;
            inputConnectionPoint.SetConnectionLimits(true, 10);
        }

        if (outputConnectionPoint != null)
        {
            outputConnectionPoint.parentItem = this;
            outputConnectionPoint.SetConnectionLimits(true, 10);
        }
    }

    private bool HasConnectionsChanged()
    {
        int currentInputCount = GetConnectedInputCount();
        int currentOutputCount = GetConnectedOutputCount();

        if (currentInputCount != lastInputConnectionCount || currentOutputCount != lastOutputConnectionCount)
        {
            lastInputConnectionCount = currentInputCount;
            lastOutputConnectionCount = currentOutputCount;
            return true;
        }

        return false;
    }

    private void UpdatePowerDistribution()
    {
        totalInputPower = 0f;

        if (inputConnectionPoint != null && inputConnectionPoint.isConnected)
        {
            totalInputPower = GetPowerFromInputConnectionPoint(inputConnectionPoint);
        }

        float effectiveInputPower = totalInputPower * powerEfficiency;
        DistributePowerToOutput(effectiveInputPower);

        OnConnectionsUpdated?.Invoke();
    }

    private float GetPowerFromInputConnectionPoint(ConnectionPoint inputPoint)
    {
        if (inputPoint.connectedCables == null || inputPoint.connectedCables.Count == 0)
            return 0f;

        float totalPowerFromInput = 0f;

        ConnectionPoint[] allConnectionPoints = ConnectionPointManager.GetAllConnectionPoints();

        foreach (GameObject cable in inputPoint.connectedCables)
        {
            if (cable == null) continue;

            CableInteractable cableComponent = cable.GetComponentInChildren<CableInteractable>();
            if (cableComponent == null) continue;

            ConnectionPoint sourceConnectionPoint = FindOtherConnectionPoint(cable, inputPoint);
            
            if (sourceConnectionPoint != null)
            {
                if (sourceConnectionPoint.parentItem is PowerCell powerCell)
                {
                    totalPowerFromInput += powerCell.itemData.powerGeneration;
                }
                else if (sourceConnectionPoint.parentItem is SolarFrame solarFrame)
                {
                    totalPowerFromInput += solarFrame.GetNetPowerOutput();
                }
                else if (sourceConnectionPoint.parentItem is CombinerBox otherCombiner)
                {
                    totalPowerFromInput += otherCombiner.GetAvailableOutputPower();
                }
            }
        }

        return totalPowerFromInput;
    }

    private void DistributePowerToOutput(float availablePower)
    {
        outputPowerValues.Clear();
        devicePowerRequirements.Clear();
        remainingPower = availablePower;

        if (outputConnectionPoint == null || !outputConnectionPoint.isConnected)
        {
            return;
        }

        GetConnectedDevicesFromOutput(reusableDeviceList);

        if (reusableDeviceList.Count == 0)
        {
            return;
        }

        foreach (var device in reusableDeviceList)
        {
            float deviceRequirement = 0f;
            float powerToProvide = 0f;

            if (device is Target target)
            {
                deviceRequirement = target.GetPowerRequirement();
            }
            else if (device is CombinerBox combiner)
            {
                combiner.ForceUpdateDistribution();
                deviceRequirement = combiner.GetTotalConnectedDeviceRequirements();
            }

            devicePowerRequirements.Add(deviceRequirement);
            if (remainingPower >= deviceRequirement && deviceRequirement > 0)
            {
                powerToProvide = deviceRequirement;
                remainingPower -= deviceRequirement;
            }
            else
            {
                powerToProvide = remainingPower;
                remainingPower = 0;
            }

            outputPowerValues.Add(powerToProvide);

            if (device is Target targetDevice)
            {
                targetDevice.UpdatePower(powerToProvide);
            }
            else if (device is CombinerBox combinerDevice)
            {
                combinerDevice.ForceUpdateDistribution();
            }
        }
    }

    private void GetConnectedDevicesFromOutput(List<System.Object> deviceList)
    {
        deviceList.Clear();

        if (outputConnectionPoint == null || outputConnectionPoint.connectedCables == null)
            return;

        ConnectionPoint[] allConnectionPoints = ConnectionPointManager.GetAllConnectionPoints();

        foreach (GameObject cable in outputConnectionPoint.connectedCables)
        {
            if (cable == null) continue;

            ConnectionPoint otherPoint = FindOtherConnectionPoint(cable, outputConnectionPoint);
            if (otherPoint != null)
            {
                if (otherPoint.parentItem is Target target)
                {
                    deviceList.Add(target);
                }
                else if (otherPoint.parentItem is CombinerBox combiner && combiner != this)
                {
                    deviceList.Add(combiner);
                }
            }
        }
    }
    private ConnectionPoint FindOtherConnectionPoint(GameObject cable, ConnectionPoint excludePoint)
    {
        if (cable == null) return null;

        CableInteractable interactable = cable.GetComponentInChildren<CableInteractable>();
        if (interactable != null)
        {
            return interactable.GetOtherPoint(excludePoint);
        }

        return null;
    }

    public void ResetConnectedTargetsPower()
    {
        if (outputConnectionPoint == null || !outputConnectionPoint.isConnected)
            return;

        GetConnectedDevicesFromOutput(reusableDeviceList);

        foreach (var device in reusableDeviceList)
        {
            if (device is Target target)
            {
                target.ResetPower();
            }
            else if (device is CombinerBox combiner)
            {
                combiner.ResetConnectedTargetsPower();
            }
        }

        outputPowerValues.Clear();
        devicePowerRequirements.Clear();
        remainingPower = 0f;
    }

    public float GetAvailableOutputPower()
    {
        return remainingPower;
    }

    public float GetEffectiveOutputPower()
    {
        return totalInputPower * powerEfficiency;
    }

    public float GetRemainingPower()
    {
        return remainingPower;
    }

    public float GetTotalConnectedDeviceRequirements()
    {
        float total = 0f;
        for (int i = 0; i < devicePowerRequirements.Count; i++)
        {
            total += devicePowerRequirements[i];
        }
        return total;
    }

    public float GetPowerEfficiency()
    {
        return powerEfficiency;
    }

    public void SetPowerEfficiency(float efficiency)
    {
        powerEfficiency = Mathf.Clamp01(efficiency);
        UpdatePowerDistribution();
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public int GetConnectedInputCount()
    {
        return inputConnectionPoint != null ? inputConnectionPoint.GetConnectionCount() : 0;
    }

    public int GetConnectedOutputCount()
    {
        return outputConnectionPoint != null ? outputConnectionPoint.GetConnectionCount() : 0;
    }

    public ConnectionPoint GetInputConnectionPoint()
    {
        return inputConnectionPoint;
    }

    public ConnectionPoint GetOutputConnectionPoint()
    {
        return outputConnectionPoint;
    }

    public void ForceUpdateDistribution()
    {
        UpdatePowerDistribution();
    }

    public bool CanConnectNewDevice(float deviceRequirement)
    {
        return remainingPower >= deviceRequirement;
    }

    private void OnDestroy()
    {
        reusableTargetList?.Clear();
        reusableCombinerList?.Clear();
        reusableDeviceList?.Clear();
        outputPowerValues?.Clear();
        devicePowerRequirements?.Clear();
        debugStringBuilder?.Clear();
    }

    public List<ConnectionPoint> GetInputConnectionPoints()
    {
        List<ConnectionPoint> points = new List<ConnectionPoint>();
        if (inputConnectionPoint != null)
            points.Add(inputConnectionPoint);
        return points;
    }

    public List<ConnectionPoint> GetOutputConnectionPoints()
    {
        List<ConnectionPoint> points = new List<ConnectionPoint>();
        if (outputConnectionPoint != null)
            points.Add(outputConnectionPoint);
        return points;
    }

}