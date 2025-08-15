using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    public Sprite hoverSprite;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;
    private Camera mainCamera;

    private bool isHovering = false;
    private bool isDragging = false;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
    }

    void Update()
    {
        if (GameManager.currentState != GameManager.GameState.Normal) return;

        RaycastHit2D hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
        bool mouseIsOver = (hit.collider != null && hit.collider.gameObject == this.gameObject);

        if (mouseIsOver && !isHovering)
        {
            isHovering = true;
            if (hoverSprite != null)
            {
                spriteRenderer.sprite = hoverSprite;
            }
        }
        else if (!mouseIsOver && isHovering)
        {
            isHovering = false;
            spriteRenderer.sprite = originalSprite;
        }

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (GetComponent<SolarFrame>() != null && PointerOverChildPowerCell())
                return;

            isDragging = true;
            offset = transform.position - GetMouseWorldPos();
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
        if (isHovering && Mouse.current.rightButton.wasPressedThisFrame)
        {
            var tag = GetComponent<SpawnedItemTag>();
            if (tag && !string.IsNullOrEmpty(tag.slotId))
                InventoryManager.Instance?.Refund(tag.slotId);

            Destroy(gameObject);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mousePos.z = 0;
        return mousePos;
    }
    bool PointerOverChildPowerCell()
    {
        Vector3 mp = mainCamera.ScreenToWorldPoint(
            UnityEngine.InputSystem.Pointer.current.position.ReadValue()
        );
        mp.z = 0;

        var hits = Physics2D.OverlapPointAll(mp);
        foreach (var h in hits)
        {
            if (h == null) continue;

            if (h.TryGetComponent<PowerCellManager>(out _) && h.transform.IsChildOf(transform))
                return true;
        }
        return false;
    }
}