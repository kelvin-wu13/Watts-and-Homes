using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasFollower : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private float canvasScale = 1f; // Adjustable in inspector
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private int sortingOrder = 10;

    private Canvas canvas;
    private RectTransform rectTransform;
    [SerializeField] private Transform parentTransform;
    private Vector3 localOffset;
    private Camera mainCamera;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;

        // Set canvas to World Space mode
        canvas.renderMode = RenderMode.WorldSpace;

        // Get parent transform (should be DividerBox)
        parentTransform = transform.parent;

        if (parentTransform != null)
        {
            // Store the initial local offset
            localOffset = transform.localPosition;
        }

        SetupCanvas();
    }

    void Start()
    {
        // Ensure proper positioning after all components are initialized
        if (parentTransform != null)
        {
            UpdatePosition();
        }
    }

    void LateUpdate()
    {
        if (parentTransform != null)
        {
            UpdatePosition();
        }
    }

    private void SetupCanvas()
    {
        // Adjust canvas properties for world space
        canvas.worldCamera = mainCamera;
        canvas.sortingLayerName = "Default"; // Use Default if no UI layer exists
        canvas.sortingOrder = sortingOrder;

        // Set appropriate scale for visibility
        transform.localScale = Vector3.one * canvasScale;

        // Setup RectTransform for proper sizing
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(300, 150); // Adjust as needed
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        // Add CanvasScaler for consistent scaling
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = 1f;

        // Ensure GraphicRaycaster exists for input
        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void UpdatePosition()
    {
        // Keep the canvas at the correct world position relative to parent
        Vector3 worldPosition = parentTransform.position + parentTransform.TransformDirection(localOffset) + positionOffset;
        transform.position = worldPosition;

        // Keep the canvas facing the camera if enabled
        if (faceCamera && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                           mainCamera.transform.rotation * Vector3.up);
        }
    }

    // Method to be called when parent is moved (optional, for immediate updates)
    public void OnParentMoved()
    {
        UpdatePosition();
    }

    // Method to adjust canvas scale at runtime
    public void SetCanvasScale(float newScale)
    {
        canvasScale = newScale;
        transform.localScale = Vector3.one * canvasScale;
    }

    // Method to set position offset
    public void SetPositionOffset(Vector3 offset)
    {
        positionOffset = offset;
        UpdatePosition();
    }

    // Validation in editor
    void OnValidate()
    {
        if (Application.isPlaying && transform != null)
        {
            transform.localScale = Vector3.one * canvasScale;
            UpdatePosition();
        }
    }
}