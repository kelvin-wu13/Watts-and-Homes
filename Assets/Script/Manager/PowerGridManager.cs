using UnityEngine;

public class PowerGridManager : MonoBehaviour
{
    public static PowerGridManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    public void ActivateGridAndCheckObjectives()
    {
        RunPowerDistribution();

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.CheckAllObjectives();
        }
        else
        {
            Debug.LogWarning("ObjectiveManager.Instance tidak ditemukan! Pengecekan tujuan dilewati.");
        }
    }
    private void RunPowerDistribution()
    {
        DeactivateAllTargets();

        PowerCell[] allPowerCells = FindObjectsByType<PowerCell>(FindObjectsSortMode.None);
        SolarFrame[] allSolarFrames = FindObjectsByType<SolarFrame>(FindObjectsSortMode.None);

        foreach (PowerCell cell in allPowerCells)
        {
            ConnectionPoint cp = cell.GetComponent<ConnectionPoint>();
            if (cp != null && cp.isConnected)
            {
                TriggerUpdateOnConnectedDevice(cp);
            }
        }

        foreach (SolarFrame frame in allSolarFrames)
        {
            ConnectionPoint cp = frame.GetComponent<ConnectionPoint>();
            if (cp != null && cp.isConnected)
            {
                TriggerUpdateOnConnectedDevice(cp);
            }
        }
    }
    private void TriggerUpdateOnConnectedDevice(ConnectionPoint sourcePoint)
    {
        foreach (GameObject cable in sourcePoint.connectedCables)
        {
            if (cable == null) continue;
            CableInteractable cableInfo = cable.GetComponentInChildren<CableInteractable>();
            if (cableInfo == null) continue;

            ConnectionPoint destinationPoint = cableInfo.GetOtherPoint(sourcePoint);
            if (destinationPoint == null || destinationPoint.parentItem == null) continue;

            if (destinationPoint.parentItem is CombinerBox combiner)
            {
                combiner.ForceUpdateDistribution();
            }
            else if (destinationPoint.parentItem is DividerBox divider)
            {
                divider.ForceUpdateDistribution();
            }
            else if (destinationPoint.parentItem is Target target)
            {
                float powerFromSource = 0f;
                if (sourcePoint.parentItem is PowerCell sourceCell)
                {
                    powerFromSource = sourceCell.itemData.powerGeneration;
                }
                else if (sourcePoint.parentItem is SolarFrame sourceFrame)
                {
                    powerFromSource = sourceFrame.GetNetPowerOutput();
                }
                target.UpdatePower(powerFromSource);
            }
        }
    }
    private void DeactivateAllTargets()
    {
        Target[] allTargets = FindObjectsByType<Target>(FindObjectsSortMode.None);
        foreach (var target in allTargets)
        {
            target.ResetPower();
        }
    }
}
