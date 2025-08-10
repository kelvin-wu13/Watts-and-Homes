using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class Slot
    {
        [Tooltip("Kunci unik. Contoh: Cable, SolarFrame, CombinerBox, dsb.")]
        public string slotId;

        [Header("Optional (boleh kosong)")]
        public ItemData item;    
        public GameObject prefab;  

        [Header("Limit")]
        public int maxUses = 0;
        [HideInInspector] public int used = 0;

        [Header("UI (boleh kosong)")]
        public TextMeshProUGUI counterText;
        public Button button;
    }

    public List<Slot> slots = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        RefreshAllUI();
    }

    public bool CanUse(string slotId) { var s = GetSlot(slotId); return s != null && s.used < s.maxUses; }
    public bool CanUse(ItemData item) { var s = GetSlot(item); return s != null && s.used < s.maxUses; }

    public bool Consume(string slotId, GameObject spawned = null)
    {
        var s = GetSlot(slotId);
        if (s == null)
        {
            Debug.LogError($"[Inventory] Slot '{slotId}' TIDAK DITEMUKAN. " +
                           $"Cek InventoryManager.slots di scene ini.");
            return false;
        }
        if (s.used >= s.maxUses)
        {
            Debug.LogWarning($"[Inventory] Slot '{slotId}' HABIS ({s.used}/{s.maxUses}).");
            return false;
        }

        s.used++;
        TagSpawned(spawned, slotId);
        RefreshSlotUI(s);
        Debug.Log($"[Inventory] {slotId}: used={s.used}/{s.maxUses}");
        return true;
    }
    public bool Consume(ItemData item, GameObject spawned = null)
        => Consume(GetSlot(item)?.slotId, spawned);

    public void Refund(string slotId)
    {
        var s = GetSlot(slotId);
        if (s == null) return;
        s.used = Mathf.Max(0, s.used - 1);
        RefreshSlotUI(s);
    }

    Slot GetSlot(string slotId) => slots.Find(x => x != null && x.slotId == slotId);
    Slot GetSlot(ItemData item) => slots.Find(x => x != null && x.item == item);

    void TagSpawned(GameObject go, string slotId)
    {
        if (!go) return;
        var tag = go.GetComponent<SpawnedItemTag>();
        if (!tag) tag = go.AddComponent<SpawnedItemTag>();
        tag.slotId = slotId;
    }

    void RefreshAllUI() { foreach (var s in slots) RefreshSlotUI(s); }
    void RefreshSlotUI(Slot s)
    {
        if (s == null) return;
        int remain = Mathf.Max(0, s.maxUses - s.used);
        if (s.counterText) s.counterText.text = remain.ToString();
        if (s.button) s.button.interactable = remain > 0;
    }
}