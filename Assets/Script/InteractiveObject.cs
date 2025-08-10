using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour
{
    public GameObject itemPrefab;
    public string slotId;

    private Camera mainCamera;

    private static GameObject previewInstance;
    private static InteractiveObject activeOwner;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPress()
    {
        if (InventoryManager.Instance && !InventoryManager.Instance.CanUse(slotId)) return;

        if (previewInstance != null) Destroy(previewInstance);
        activeOwner = this;

        if (previewInstance != null) Destroy(previewInstance);
        previewInstance = Instantiate(itemPrefab);
        SetupPreviewAppearance(previewInstance);
        Debug.Log($"[InteractiveObject] OnPress slotId='{slotId}'");
    }
    private void SetupPreviewAppearance(GameObject preview)
    {
        var spriteRenderer = preview.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        var collider = preview.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        var canvasComponents = preview.GetComponentsInChildren<Canvas>();
        foreach (var canvas in canvasComponents)
        {
            var canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0.5f;
        }
    }

    private void Update()
    {
        if (activeOwner != this) return;

        if (previewInstance != null)
        {
            previewInstance.transform.position = GetMouseWorldPos();

            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceObject();
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Destroy(previewInstance);
                previewInstance = null;
            }
        }
    }

    private void PlaceObject()
    {
        // finalize visual
        var spriteRenderer = previewInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        var collider = previewInstance.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;

        foreach (var canvas in previewInstance.GetComponentsInChildren<Canvas>())
        {
            var cg = canvas.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 1f;
        }

        // konsumsi slot utk owner saat ini
        bool ok = true;
        if (InventoryManager.Instance)
            ok = InventoryManager.Instance.Consume(slotId, previewInstance);

        if (!ok)
        {
            Debug.LogWarning($"[PlaceObject] Gagal consume slot '{slotId}'.");
            Destroy(previewInstance);
        }

        // selesai
        previewInstance = null;
        activeOwner = null;
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mousePos.z = 0;
        return mousePos;
    }
}
