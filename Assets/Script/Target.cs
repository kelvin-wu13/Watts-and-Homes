using UnityEngine;

public class Target : MonoBehaviour,IPowerDataProvider
{
    public ItemData itemData;
    public Sprite spriteOn;
    public Sprite spriteOff;

    private SpriteRenderer spriteRenderer;
    private float powerReceived = 0f;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }
    public string GetPowerDisplayText()
    {
        if (itemData == null) return "";
        return $"{itemData.powerRequirement:F1} W";
    }

    public void UpdatePower(float power)
    {
        powerReceived = power;
        UpdateVisuals();
    }

    public void ResetPower()
    {
        powerReceived = 0f;
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (powerReceived >= itemData.powerRequirement && itemData.powerRequirement > 0)
        {
            spriteRenderer.sprite = spriteOn;
        }
        else
        {
            spriteRenderer.sprite = spriteOff;
        }
    }

    public float GetPowerRequirement()
    {
        return itemData != null ? itemData.powerRequirement : 0f;
    }
}