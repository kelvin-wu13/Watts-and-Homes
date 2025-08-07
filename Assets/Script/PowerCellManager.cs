using UnityEngine;
using UnityEngine.InputSystem;

public class PowerCellManager : MonoBehaviour
{
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
        if (!Mouse.current.leftButton.isPressed) return false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mouseWorldPos.z = 0;

        float distanceToMouse = Vector3.Distance(transform.position, mouseWorldPos);
        return distanceToMouse < 1f;
    }

    private void OnStartDrag()
    {
        transform.SetParent(null);

        originalPosition = transform.position;
        wasInSlot = currentSlot != null;

        PowerCell powerCell = GetComponent<PowerCell>();
        if (powerCell != null)
        {
            powerCell.SetInSlot(false);
        }

        ConnectionPoint cp = GetComponent<ConnectionPoint>();
        if (cp != null)
        {
            cp.enabled = true;
        }

        if (currentSlot != null)
        {
            currentSlot.RemovePowerCell();
            currentSlot = null;
        }
    }

    private void OnDrag()
    {
        ShowNearbySlotsFeedback();
    }

    private void OnEndDrag()
    {
        ClearSlotsFeedback();

        SolarFrame[] frames = FindObjectsByType<SolarFrame>(FindObjectsSortMode.None);
        bool placed = false;

        foreach (SolarFrame frame in frames)
        {
            PowerCellSlot nearestSlot = frame.FindNearestEmptySlot(transform.position);
            if (nearestSlot != null)
            {
                placed = nearestSlot.PlacePowerCell(this);
                if (placed) break;
            }
        }

        if (placed)
        {
            PowerCell powerCell = GetComponent<PowerCell>();
            if (powerCell != null)
            {
                powerCell.SetInSlot(true);
            }

            ConnectionPoint cp = GetComponent<ConnectionPoint>();
            if (cp != null)
            {
                cp.enabled = false;
            }
        }
        else
        {
            PowerCell powercell = GetComponent<PowerCell>();
            if (powercell != null)
            {
                powercell.SetInSlot(false);
            }
        }
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

    private void ClearSlotsFeedback()
    {
        PowerCellSlot[] allSlots = FindObjectsByType<PowerCellSlot>(FindObjectsSortMode.None);
        foreach (PowerCellSlot slot in allSlots)
        {
            if (!slot.isOccupied)
            {
                slot.OnHoverExit();
            }
        }
    }
    public void SetSlot(PowerCellSlot slot)
    {
        currentSlot = slot;

        PowerCell powerCell = GetComponent<PowerCell>();
        if (powerCell != null)
        {
            powerCell.SetInSlot(slot != null);
        }
    }

    public PowerCellSlot GetCurrentSlot()
    {
        return currentSlot;
    }
}