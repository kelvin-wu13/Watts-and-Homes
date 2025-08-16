using UnityEngine;

public class Target : MonoBehaviour,IPowerDataProvider
{
    public ItemData itemData;
    public Sprite spriteOn;
    public Sprite spriteOff;

    public Color highlightColor = new Color(1f, 1f, 0.6f, 1f);
    public Color invalidHighlightColor = new Color(1f, 0.5f, 0.5f, 1f);

    private bool externalHighlightActive = false;
    private Color originalColor;

    private SpriteRenderer spriteRenderer;
    private float powerReceived = 0f;
    public bool IsPowered { get; private set; }

    public float PowerReceived => powerReceived;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer ? spriteRenderer.color : Color.white;
        UpdateVisuals();
    }
    public string GetPowerDisplayText()
    {
        if (itemData == null) return "";
        return $"{itemData.powerRequirement:F1} W";
    }

    public void UpdatePower(float power)
    {
        powerReceived += power;
        UpdateVisuals();
    }

    public void ResetPower()
    {
        powerReceived = 0f;
        UpdateVisuals();
    }
    public void SetExternalHighlight(bool on, bool valid = true)
    {
        externalHighlightActive = on;

        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

        if (on)
        {
            spriteRenderer.color = valid ? highlightColor : invalidHighlightColor;
        }
        else
        {
            spriteRenderer.color = originalColor;
            UpdateVisuals();
        }
    }
    public void UpdateVisuals()
    {
        if (externalHighlightActive) return;
        if (powerReceived >= itemData.powerRequirement && itemData.powerRequirement > 0)
        {
            spriteRenderer.sprite = spriteOn;
            IsPowered = true;
        }
        else
        {
            spriteRenderer.sprite = spriteOff;
            IsPowered = false;
        }
    }

    public float GetPowerRequirement()
    {
        return itemData != null ? itemData.powerRequirement : 0f;
    }
}