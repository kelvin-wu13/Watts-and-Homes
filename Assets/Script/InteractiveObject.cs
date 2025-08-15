using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject itemPrefab;
    public string slotId;

    private Camera mainCamera;

    // Preview & kepemilikan global (agar instance lain tidak ikut handle)
    private static GameObject previewInstance;
    private static InteractiveObject activeOwner;

    private enum DragState { Idle, WaitingForPress, Dragging }
    private static DragState state = DragState.Idle;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // PANGGIL INI dari OnPointerDown (ideal) atau OnClick (masih didukung)
    public void OnPress()
    {
        // Cek stok lebih dulu
        if (InventoryManager.Instance && !InventoryManager.Instance.CanUse(slotId))
        {
            Debug.Log($"[InteractiveObject] {slotId} stok habis.");
            return;
        }

        // Reset preview lama & set owner
        if (previewInstance) Destroy(previewInstance);
        activeOwner = this;

        // Buat preview
        previewInstance = Instantiate(itemPrefab);
        if (previewInstance && !previewInstance.activeSelf) previewInstance.SetActive(true);
        SetupPreviewAppearance(previewInstance);

        // Posisi awal
        if (previewInstance) previewInstance.transform.position = GetMouseWorldPos();

        // Jika dipanggil via OnPointerDown: tombol sudah pressed → langsung dragging
        // Jika via OnClick: tombol sudah released → tunggu press berikutnya
        state = (Mouse.current != null && Mouse.current.leftButton.isPressed)
              ? DragState.Dragging
              : DragState.WaitingForPress;

        Debug.Log($"[InteractiveObject] Start drag slotId='{slotId}' | state={state}");
    }

    private void Update()
    {
        // Hanya owner yang boleh meng-handle
        if (activeOwner != this || previewInstance == null) return;

        // Batal dengan ESC / Right Click
        if ((Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
             (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame))
        {
            CancelDrag();
            return;
        }

        // Gerakkan preview
        previewInstance.transform.position = GetMouseWorldPos();

        // State machine drag & drop
        if (Mouse.current == null) return;

        switch (state)
        {
            case DragState.WaitingForPress:
                if (Mouse.current.leftButton.wasPressedThisFrame)
                    state = DragState.Dragging;
                break;

            case DragState.Dragging:
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    // Jangan place kalau lepas di atas UI
                    if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject())
                        PlaceObject();
                    else
                        CancelDrag();
                }
                break;
        }
    }

    private void PlaceObject()
    {
        // Finalize visual (aktifkan collider & opacity)
        var sr = previewInstance.GetComponent<SpriteRenderer>();
        if (sr) sr.color = Color.white;

        var col = previewInstance.GetComponent<Collider2D>();
        if (col) col.enabled = true;

        foreach (var canvas in previewInstance.GetComponentsInChildren<Canvas>())
        {
            var cg = canvas.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 1f;
        }

        // Konsumsi slot (limit)
        bool ok = InventoryManager.Instance ?
                  InventoryManager.Instance.Consume(slotId, previewInstance) : true;

        if (!ok)
        {
            Debug.LogWarning($"[PlaceObject] Gagal consume slot '{slotId}'.");
            Destroy(previewInstance);
        }

        // Selesai
        previewInstance = null;
        activeOwner = null;
        state = DragState.Idle;
    }

    private void CancelDrag()
    {
        if (previewInstance) Destroy(previewInstance);
        previewInstance = null;
        activeOwner = null;
        state = DragState.Idle;
    }

    private void SetupPreviewAppearance(GameObject go)
    {
        if (!go) return;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr) { var c = sr.color; c.a = 0.5f; sr.color = c; }

        var col = go.GetComponent<Collider2D>();
        if (col) col.enabled = false;

        foreach (var canvas in go.GetComponentsInChildren<Canvas>())
        {
            var cg = canvas.GetComponent<CanvasGroup>();
            if (!cg) cg = canvas.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0.5f;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        var mp = mainCamera.ScreenToWorldPoint(
            Pointer.current != null ? Pointer.current.position.ReadValue() : Input.mousePosition
        );
        mp.z = 0;
        return mp;
    }
}
