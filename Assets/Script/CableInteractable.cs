using UnityEngine;
using UnityEngine.InputSystem;

public class CableInteractable : MonoBehaviour
{
    public Color hoverColor = Color.white;

    [Header("Collider Settings")]
    public float colliderWidth = 0.5f;

    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;
    private Color originalColor;
    private bool isHovering = false;
    private Camera mainCamera;

    private ConnectionPoint point1;
    private ConnectionPoint point2;

    private void Start()
    {
        mainCamera = Camera.main;
        lineRenderer = GetComponentInParent<LineRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalColor = lineRenderer.startColor;
    }

    public void Initialize(ConnectionPoint p1, ConnectionPoint p2)
    {
        this.point1 = p1;
        this.point2 = p2;

        UpdateBoxCollider();
    }

    void Update()
    {
        UpdateCablePositions();

        if (lineRenderer != null && boxCollider != null)
        {
            if (GameManager.currentState != GameManager.GameState.Normal)
            {
                if (isHovering)
                {
                    isHovering = false;
                    lineRenderer.startColor = originalColor;
                    lineRenderer.endColor = originalColor;
                }
                return;
            }

            RaycastHit2D hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));
            bool mouseIsOver = (hit.collider != null && hit.collider.gameObject == this.gameObject);

            if (mouseIsOver && !isHovering)
            {
                isHovering = true;
                lineRenderer.startColor = hoverColor;
                lineRenderer.endColor = hoverColor;
            }
            else if (!mouseIsOver && isHovering)
            {
                isHovering = false;
                lineRenderer.startColor = originalColor;
                lineRenderer.endColor = originalColor;
            }

            if (isHovering && Mouse.current.rightButton.wasPressedThisFrame)
            {
                DestroyCable();
            }
        }
    }
    
    public void DestroyCable()
    {
        if (lineRenderer == null)
        {
            Destroy(this.gameObject);
            return;
        }

        GameObject cableObject = lineRenderer.gameObject;

        ResetConnectedTargetsPower();

        if (point1 != null)
        {
            point1.DisconnectCable(cableObject);
        }
        if (point2 != null)
        {
            point2.DisconnectCable(cableObject);
        }
        Destroy(cableObject);
    }

    private void UpdateCablePositions()
    {
        if (point1 == null || point2 == null || lineRenderer == null)
            return;

        lineRenderer.SetPosition(0, point1.transform.position);
        lineRenderer.SetPosition(1, point2.transform.position);

        UpdateBoxCollider();
    }
    public ConnectionPoint GetOtherPoint(ConnectionPoint knownPoint)
    {
        if (knownPoint == point1)
        {
            return point2;
        }
        if (knownPoint == point2)
        {
            return point1;
        }
        return null;
    }

    private void UpdateBoxCollider()
    {
        if (boxCollider == null || point1 == null || point2 == null)
            return;

        Vector3 pos1 = point1.transform.position;
        Vector3 pos2 = point2.transform.position;

        Vector3 center = (pos1 + pos2) / 2f;

        float distance = Vector3.Distance(pos1, pos2);

        Vector3 direction = (pos2 - pos1).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        boxCollider.size = new Vector2(distance, colliderWidth);
        boxCollider.transform.position = center;
        boxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void ResetConnectedTargetsPower()
    {

    }
}