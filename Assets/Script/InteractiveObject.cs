using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour
{
    public GameObject itemPrefab;
    private Camera mainCamera;

    private static GameObject previewInstance;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPress()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        previewInstance = Instantiate(itemPrefab);

        SetupPreviewAppearance(previewInstance);
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
        if (previewInstance != null)
        {
            previewInstance.transform.position = GetMouseWorldPos();

            if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
            {
                PlaceObject();
            }
        }
    }

    private void PlaceObject()
    {
        var spriteRenderer = previewInstance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        var collider = previewInstance.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        var canvasComponents = previewInstance.GetComponentsInChildren<Canvas>();
        foreach (var canvas in canvasComponents)
        {
            var canvasGroup = canvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
        previewInstance = null;
    }
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mousePos.z = 0;
        return mousePos;
    }
}
