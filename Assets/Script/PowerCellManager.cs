using UnityEngine;
using UnityEngine.InputSystem;

public class PowerCellManager : MonoBehaviour
{
    public bool IsDragging { get; private set; } = false;

    [Header("Power Cell Settings")]
    public float snapDistance = 2f;

    private PowerCellSlot currentSlot;
    private Vector3 originalPosition;
    private bool wasInSlot = false;
    private Interactable interactable;
    private bool wasDragging = false;

    private void Start()
    {
        Interactable genericInteractable = GetComponent<Interactable>();
        originalPosition = transform.position;
    }

    private void Update()
    {
        bool isDragging = IsCurrentlyBeingDragged();

        if (isDragging && !wasDragging)
        {
            OnStartDrag();
        }
        else if (!isDragging && wasDragging)
        {
            OnEndDrag();
        }
        else if (isDragging)
        {
            OnDrag();
        }

        wasDragging = isDragging;
    }

    private bool IsCurrentlyBeingDragged()
    {
        if (GameManager.currentState != GameManager.GameState.Normal) return false;
        if (Mouse.current == null) return false;

        // Saat sudah dragging, cukup tahan tombol kiri
        if (wasDragging) return Mouse.current.leftButton.isPressed;

        // Mulai drag HANYA jika klik langsung di cell ini
        var ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        var hit = Physics2D.GetRayIntersection(ray);
        return hit.collider != null
            && hit.collider.gameObject == gameObject
            && Mouse.current.leftButton.isPressed;
    }

    private void OnStartDrag()
    {
        IsDragging = true;

        // kalau sedang di slot, lepas parent tapi TETAPKAN world transform
        if (currentSlot != null)
        {
            var old = currentSlot;
            currentSlot = null;
            old.RemovePowerCell();                 // fungsi lamamu
            transform.SetParent(null, true);       // true = keep world pos/rot/scale
        }

        var cp = GetComponent<ConnectionPoint>();
        if (cp) cp.enabled = true;
    }

    private void OnDrag()
    {
        // Pindahkan cell mengikuti kursor
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mouseWorldPos.z = 0;
        transform.position = mouseWorldPos;

        // Feedback slot terdekat (punyamu sudah ada)
        ShowNearbySlotsFeedback();
    }

    private void OnEndDrag()
    {
        IsDragging = false;

        // Coba snap ke slot kosong terdekat jika cukup dekat
        float bestDist = float.MaxValue;
        PowerCellSlot best = null;
        foreach (var frame in FindObjectsByType<SolarFrame>(FindObjectsSortMode.None))
        {
            var slot = frame.FindNearestEmptySlot(transform.position);
            if (slot == null) continue;
            float d = Vector3.Distance(transform.position, slot.transform.position);
            if (d < bestDist) { bestDist = d; best = slot; }
        }
        if (best != null && bestDist <= snapDistance)
            best.PlacePowerCell(this); // parent & snap (pakai logika kamu)

        var cp = GetComponent<ConnectionPoint>();
        if (cp) cp.enabled = false;
    }

    private void ShowNearbySlotsFeedback()
    {
        SolarFrame[] frames = FindObjectsByType<SolarFrame>(FindObjectsSortMode.None);

        foreach (SolarFrame frame in frames)
        {
            PowerCellSlot nearestSlot = frame.FindNearestEmptySlot(transform.position);
            if (nearestSlot != null)
            {
                nearestSlot.OnHoverEnter();
            }
        }
    }

    //private void ClearSlotsFeedback()
    //{
    //    PowerCellSlot[] allSlots = FindObjectsByType<PowerCellSlot>(FindObjectsSortMode.None);
    //    foreach (PowerCellSlot slot in allSlots)
    //    {
    //        if (!slot.isOccupied)
    //        {
    //            slot.OnHoverExit();
    //        }
    //    }
    //}
    public void SetSlot(PowerCellSlot slot)
    {
        currentSlot = slot;

        var powerCell = GetComponent<PowerCell>();
        if (powerCell) powerCell.SetInSlot(slot != null);

        if (slot != null)
        {
            // simpan world-scale sebelum diparent
            Vector3 worldScale = transform.lossyScale;

            transform.SetParent(slot.transform, true);
            transform.position = slot.transform.position;
            transform.rotation = slot.transform.rotation;
        }
        else
        {
            // keluar dari slot → lepas parent, ukuran tetap
            transform.SetParent(null, true);
        }
    }

    public PowerCellSlot GetCurrentSlot()
    {
        return currentSlot;
    }
}