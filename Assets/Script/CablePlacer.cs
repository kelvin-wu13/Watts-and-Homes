using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CablePlacer : MonoBehaviour
{
    private enum PlacementState { Idle, WaitingForFirstPoint, WaitingForSecondPoint }

    [Header("Line Settings")]
    public Material lineMaterial;
    public Color lineColor = Color.orange;
    public float lineWidth = 0.5f;
    public int cableSortingOrder = -1;

    [Header("Interaction")]
    public LayerMask connectableLayer;
    public Color previewColor = new Color(1f, 1f, 1f, 0.5f);
    public Color cableHoverColor = Color.cyan;

    [Header("Collider Settings")]
    public float cableColliderWidth = 0.5f;

    public static CablePlacer Instance;

    private PlacementState currentState = PlacementState.Idle;
    private LineRenderer currentLine;
    private ConnectionPoint firstConnectionPoint;
    private Camera mainCamera;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        mainCamera = Camera.main;
    }

    public void StartPlacingCable()
    {
        if (currentState != PlacementState.Idle) return;

        GameObject lineObject = new GameObject("Cable_Runtime");
        lineObject.transform.position = Vector3.zero;
        currentLine = lineObject.AddComponent<LineRenderer>();

        currentLine.material = lineMaterial;
        currentLine.startColor = previewColor;
        currentLine.endColor = previewColor;
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 2;

        currentLine.sortingOrder = cableSortingOrder;

        currentState = PlacementState.WaitingForFirstPoint;
    }

    private void Update()
    {
        if (GameManager.currentState != GameManager.GameState.PlacingCable)
        {
            if (currentLine != null) CancelPlacement();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
            return;
        }

        Vector3 mousePos = GetMouseWorldPos();

        if (currentState == PlacementState.WaitingForFirstPoint || currentState == PlacementState.WaitingForSecondPoint)
        {
            Vector3 startPos = (currentState == PlacementState.WaitingForFirstPoint) ? mousePos : firstConnectionPoint.transform.position;
            currentLine.SetPosition(0, startPos);
            currentLine.SetPosition(1, mousePos);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray cameraRay = mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.GetRayIntersection(cameraRay, Mathf.Infinity, connectableLayer);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("IConnectable"))
                {
                    if (hit.collider.TryGetComponent(out ConnectionPoint clickedPoint))
                    {
                        HandleConnectionPointClick(clickedPoint);
                    }
                }
            }
        }
    }

    private void HandleConnectionPointClick(ConnectionPoint point)
    {
        if (!point.CanAcceptMoreConnections()) return;

        if (currentState == PlacementState.WaitingForFirstPoint)
        {
            firstConnectionPoint = point;
            firstConnectionPoint.ConnectCable(currentLine.gameObject);

            currentLine.startColor = lineColor;
            currentLine.endColor = lineColor;

            currentState = PlacementState.WaitingForSecondPoint;
        }
        else if (currentState == PlacementState.WaitingForSecondPoint)
        {
            if (point == firstConnectionPoint) return;

            point.ConnectCable(currentLine.gameObject);

            currentLine.SetPosition(0, firstConnectionPoint.transform.position);
            currentLine.SetPosition(1, point.transform.position);

            Vector3 pos1 = firstConnectionPoint.transform.position;
            Vector3 pos2 = point.transform.position;
            Vector3 center = (pos1 + pos2) / 2f;
            float distance = Vector3.Distance(pos1, pos2);
            float angle = Mathf.Atan2(pos2.y - pos1.y, pos2.x - pos1.x) * Mathf.Rad2Deg;

            GameObject colliderObj = new GameObject("CableCollider");
            colliderObj.transform.SetParent(currentLine.transform);

            colliderObj.transform.position = center;
            colliderObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            BoxCollider2D lineCollider = colliderObj.AddComponent<BoxCollider2D>();
            lineCollider.size = new Vector2(distance, cableColliderWidth);

            CableInteractable interactable = colliderObj.AddComponent<CableInteractable>();
            interactable.hoverColor = this.cableHoverColor;
            interactable.colliderWidth = this.cableColliderWidth;
            interactable.Initialize(firstConnectionPoint, point);


            currentLine.sortingOrder = cableSortingOrder;

            SetConnections(firstConnectionPoint, point);

            ResetPlacement();
        }
    }

    private void SetConnections(ConnectionPoint point1, ConnectionPoint point2)
    {
        MonoBehaviour p1Parent = point1.parentItem;
        MonoBehaviour p2Parent = point2.parentItem;
        Target consumer = null;
        float powerToSend = 0f;

        if (p1Parent is PowerCell cell1 && p2Parent is Target target1)
        {
            powerToSend = cell1.itemData.powerGeneration;
            consumer = target1;
        }
        else if (p2Parent is PowerCell cell2 && p1Parent is Target target2)
        {
            powerToSend = cell2.itemData.powerGeneration;
            consumer = target2;
        }
        else if (p1Parent is SolarFrame frame1 && p2Parent is Target target3)
        {
            powerToSend = frame1.GetNetPowerOutput();
            consumer = target3;
        }
        else if (p2Parent is SolarFrame frame2 && p1Parent is Target target4)
        {
            powerToSend = frame2.GetNetPowerOutput();
            consumer = target4;
        }
        else if (p1Parent is CombinerBox combiner1)
        {
            combiner1.ForceUpdateDistribution();
            if (p2Parent is CombinerBox combiner2)
            {
                combiner2.ForceUpdateDistribution();
            }
        }
        else if (p2Parent is CombinerBox combiner3)
        {
            combiner3.ForceUpdateDistribution();
        }
        else if (p1Parent is PowerCell cell3 && p2Parent is CombinerBox combiner4)
        {
            combiner4.ForceUpdateDistribution();
        }
        else if (p2Parent is PowerCell cell4 && p1Parent is CombinerBox combiner5)
        {
            combiner5.ForceUpdateDistribution();
        }
        else if (p1Parent is SolarFrame frame3 && p2Parent is CombinerBox combiner6)
        {
            combiner6.ForceUpdateDistribution();
        }
        else if (p2Parent is SolarFrame frame4 && p1Parent is CombinerBox combiner7)
        {
            combiner7.ForceUpdateDistribution();
        }
        else if (p1Parent is DividerBox divider1)
        {
            divider1.ForceUpdateDistribution();
            if (p2Parent is CombinerBox combiner2)
            {
                combiner2.ForceUpdateDistribution();
            }
            else if (p2Parent is DividerBox divider2)
            {
                divider2.ForceUpdateDistribution();
            }
        }
        else if (p2Parent is DividerBox divider3)
        {
            divider3.ForceUpdateDistribution();
        }

        if (consumer != null)
        {
            consumer.UpdatePower(powerToSend);
        }
    }

    private void CancelPlacement()
    {
        if (firstConnectionPoint != null && currentLine != null)
        {
            firstConnectionPoint.DisconnectCable(currentLine.gameObject);
        }
        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
        }
        ResetPlacement();
    }

    private void ResetPlacement()
    {
        firstConnectionPoint = null;
        currentLine = null;
        currentState = PlacementState.Idle;
        GameManager.SetState(GameManager.GameState.Normal);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        mousePos.z = 0;
        return mousePos;
    }
}