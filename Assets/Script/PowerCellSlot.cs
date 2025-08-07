    using UnityEngine;

public class PowerCellSlot : MonoBehaviour
{
    [Header("Slot State")]
    public bool isOccupied = false;

    [Header("Visual Feedback")]
    public SpriteRenderer slotRenderer;
    public Color emptyColor = new Color(1f, 1f, 1f, 0.3f);
    public Color occupiedColor = new Color(0f, 1f, 0f, 0.5f);
    public Color hoverColor = new Color(1f, 1f, 0f, 0.6f);

    private SolarFrame parentFrame;
    private PowerCellManager currentPowerCell;
    private int row, column;
    private Collider2D slotCollider;

    private void Awake()
    {
        slotCollider = GetComponent<Collider2D>();
        if (slotCollider == null)
        {
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.5f;
            slotCollider = circleCollider;
        }
        else
        {
            slotCollider.isTrigger = true;
        }

        if (slotRenderer == null)
        {
            slotRenderer = GetComponent<SpriteRenderer>();
            if (slotRenderer == null)
            {
                slotRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }
    }

    public void Initialize(SolarFrame frame, int r, int c)
    {
        parentFrame = frame;
        row = r;
        column = c;
        SetVisualState(SlotState.Empty);
    }

    public bool PlacePowerCell(PowerCellManager powerCell)
    {
        if (isOccupied) return false;

        currentPowerCell = powerCell;
        powerCell.SetSlot(this);
        isOccupied = true;

        powerCell.transform.position = transform.position;

        powerCell.transform.SetParent(this.transform);

        SetVisualState(SlotState.Occupied);
        parentFrame.OnPowerCellPlaced();
        return true;
    }

    public void RemovePowerCell()
    {
        if (currentPowerCell != null)
        {
            currentPowerCell.SetSlot(null);
            currentPowerCell = null;
        }

        isOccupied = false;
        SetVisualState(SlotState.Empty);
        parentFrame.OnPowerCellRemoved();
    }

    public PowerCell GetPowerCell()
    {
        if (currentPowerCell != null)
        {
            return currentPowerCell.GetComponent<PowerCell>();
        }
        return null;
    }

    public void OnHoverEnter()
    {
        if (!isOccupied)
        {
            SetVisualState(SlotState.Hover);
        }
    }

    public void OnHoverExit()
    {
        if (!isOccupied)
        {
            SetVisualState(SlotState.Empty);
        }
    }

    private void SetVisualState(SlotState state)
    {
        if (slotRenderer == null) return;

        switch (state)
        {
            case SlotState.Empty:
                slotRenderer.color = emptyColor;
                break;
            case SlotState.Occupied:
                slotRenderer.color = occupiedColor;
                break;
            case SlotState.Hover:
                slotRenderer.color = hoverColor;
                break;
        }
    }

    public Vector2Int SlotCoordinates => new Vector2Int(column, row);

    public bool IsAdjacentTo(PowerCellSlot other)
    {
        if (other == null) return false;

        Vector2Int thisCoord = SlotCoordinates;
        Vector2Int otherCoord = other.SlotCoordinates;

        int deltaX = Mathf.Abs(thisCoord.x - otherCoord.x);
        int deltaY = Mathf.Abs(thisCoord.y - otherCoord.y);

        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    private enum SlotState
    {
        Empty,
        Occupied,
        Hover
    }
}