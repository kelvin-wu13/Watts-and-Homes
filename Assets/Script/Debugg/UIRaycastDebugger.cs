using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UIRaycastDebugger : MonoBehaviour
{
    [Header("Limit search (optional)")]
    public GraphicRaycaster[] onlyTheseRaycasters;

    [Header("Overlay (optional)")]
    public Text overlayText;

    [Header("Settings")]
    public bool activeAtStart = true;

    private bool _enabled;
    private readonly List<RaycastResult> _results = new List<RaycastResult>(32);

    private void Awake()
    {
        _enabled = activeAtStart;
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.f9Key.wasPressedThisFrame) _enabled = !_enabled;
#else
        if (Input.GetKeyDown(KeyCode.F9)) _enabled = !_enabled;
#endif

        if (!_enabled) { if (overlayText) overlayText.text = ""; return; }
        if (EventSystem.current == null) return;

        Vector2 screenPos = GetPointerPosition();
        var ped = new PointerEventData(EventSystem.current) { position = screenPos };

        _results.Clear();
        if (onlyTheseRaycasters != null && onlyTheseRaycasters.Length > 0)
        {
            foreach (var gr in onlyTheseRaycasters)
            {
                if (!gr || !gr.isActiveAndEnabled) continue;
                gr.Raycast(ped, _results);
            }
        }
        else
        {
            EventSystem.current.RaycastAll(ped, _results);
        }

        string topName = _results.Count > 0 ? FullPath(_results[0].gameObject.transform) : "<none>";
        if (overlayText)
            overlayText.text = _results.Count > 0 ? $"UI BLOCK: {_results[0].gameObject.name}\nPath: {topName}" : "UI BLOCK: <none>";

        if (WasLeftClickThisFrame())
        {
            if (_results.Count == 0)
            {
                Debug.Log("[UIRaycastDebugger] No UI under pointer.");
            }
            else
            {
                Debug.Log($"[UIRaycastDebugger] {_results.Count} UI hits (top-most first):");
                for (int i = 0; i < _results.Count; i++)
                {
                    var r = _results[i];
                    var canvas = r.module is GraphicRaycaster gr ? gr.gameObject : r.module.transform.gameObject;
                    Debug.Log($"#{i + 1}: '{r.gameObject.name}'  Canvas='{canvas.name}'  Path='{FullPath(r.gameObject.transform)}'  dist={r.distance:F3}  depth={r.depth}  sortOrder={r.sortingOrder}");
                }
            }
        }
    }

    private static string FullPath(Transform t)
    {
        var s = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            s = t.name + "/" + s;
        }
        return s;
    }

    private static bool WasLeftClickThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null) return Mouse.current.leftButton.wasPressedThisFrame;
#else
        if (Input.GetMouseButtonDown(0)) return true;
#endif
        return false;
    }

    private static Vector2 GetPointerPosition()
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null) return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
        return Vector2.zero;
    }
}
