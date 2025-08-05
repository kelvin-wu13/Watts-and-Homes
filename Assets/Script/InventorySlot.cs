using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemData itemData;
    public Image iconImage;


    private void Awake()
    {
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (itemData != null)
        {
            iconImage.sprite = itemData.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled= false;
        }
    }
}