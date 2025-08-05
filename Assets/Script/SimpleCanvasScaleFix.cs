using UnityEngine;

// Attach this to Canvas instead of CanvasFollower if you want simpler solution
public class SimpleCanvasScaleFix : MonoBehaviour
{
    [Header("Scale Settings")]
    public float worldSpaceScale = 0.1f; // Adjust this for size

    private Canvas canvas;
    private Transform parentTransform;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        parentTransform = transform.parent;

        // Setup world space canvas
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        canvas.sortingOrder = 10;

        // Set the scale
        transform.localScale = Vector3.one * worldSpaceScale;

        // Setup RectTransform
        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(400, 200); // Width x Height in pixels
        }
    }

    void LateUpdate()
    {
        // Follow parent position
        if (parentTransform != null)
        {
            transform.position = parentTransform.position;
        }
    }
}