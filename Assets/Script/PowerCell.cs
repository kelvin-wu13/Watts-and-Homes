using UnityEngine;

public class PowerCell : MonoBehaviour, IPowerDataProvider
{
    public ItemData itemData;
    public string GetPowerDisplayText()
    {
        if (itemData == null) return "";

        switch (itemData.itemType)
        {
            case ItemType.Generator:
                return $"{itemData.powerGeneration:F1} W";
            case ItemType.Consumer:
                return $"{itemData.powerRequirement:F1} W";
            default:
                return "";
        }
    }

    private void OnDestroy()
    {
        ConnectionPoint cp = GetComponent<ConnectionPoint>();
        if (cp != null && cp.connectedCable != null)
        {
            Destroy(cp.connectedCable);
        }
    }
}
