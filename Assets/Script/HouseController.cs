using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HouseController : MonoBehaviour
{
    public LevelData levelData;

    public Sprite highlightSprite;
    public Sprite lockedSprite;
    public Sprite starCompleteSprite;
    public Sprite starIncompleteSprite;
    private Sprite defaultSprite;

    public GameObject starsDisplayContainer;
    public DialogueTrigger levelIntroDialogue;
    public DialogueTrigger levelPostDialogue;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    private Camera mainCamera;
    private bool isHovering = false;

    private bool waitingForDialogue = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        mainCamera = Camera.main;
    }
    private void Start()
    {
        UpdateVisualState();
    }
    private void Update()
    {
        // Blokir input bila pointer di atas UI Graphic (bukan collider dunia)
        if (IsPointerOverPureUI())
        {
            if (isHovering)
            {
                isHovering = false;
                spriteRenderer.sprite = defaultSprite;
            }
            return;
        }

        if (!isUnlocked) return;

        // Raycast 2D sama seperti hover, supaya klik konsisten
        RaycastHit2D hit = Physics2D.GetRayIntersection(
            mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue())
        );
        bool mouseIsOver = (hit.collider != null && hit.collider.gameObject == this.gameObject);

        // Hover visual
        if (mouseIsOver && !isHovering)
        {
            isHovering = true;
            spriteRenderer.sprite = highlightSprite;
        }
        else if (!mouseIsOver && isHovering)
        {
            isHovering = false;
            spriteRenderer.sprite = defaultSprite;
        }

        // Klik kiri → dialog/popup
        if (mouseIsOver
            && Mouse.current.leftButton.wasPressedThisFrame
            && !waitingForDialogue
            && !(DialogueManager.Instance != null && DialogueManager.Instance.IsRunning))
        {
            OpenIntroOrPopup();
        }
    }
    private void OpenIntroOrPopup()
    {
        bool canIntro =
            levelIntroDialogue != null &&
            levelIntroDialogue.dialogueLines != null &&
            levelIntroDialogue.dialogueLines.Count > 0 &&
            !GameProgress.HasSeenHouseIntro(levelData.levelIndex);

        Debug.Log($"[HouseController] Klik house {levelData.levelIndex}, canIntro={canIntro}");

        if (canIntro)
        {
            Debug.Log($"[HouseController] Memulai intro dialogue untuk house {levelData.levelIndex}");

            DialogueManager.Instance.OnDialogueEnd += OnHouseIntroFinished;
            levelIntroDialogue.TriggerDialogue();

            if (DialogueManager.Instance == null || !DialogueManager.Instance.IsRunning)
            {
                Debug.Log("[HouseController] DialogueManager tidak berjalan, langsung buka popup");

                DialogueManager.Instance.OnDialogueEnd -= OnHouseIntroFinished;
                GameProgress.MarkHouseIntroSeen(levelData.levelIndex);

                waitingForDialogue = false;
                MapManager.Instance.PrepareLevelPopup(this);
                return;
            }

            waitingForDialogue = true;
        }
        else
        {
            Debug.Log($"[HouseController] Intro sudah pernah dilihat atau tidak ada, langsung buka popup untuk house {levelData.levelIndex}");
            MapManager.Instance.PrepareLevelPopup(this);
        }
    }

    private void OnHouseIntroFinished()
    {
        // Lepas listener biar tidak nyangkut
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueEnd -= OnHouseIntroFinished;

        // Tandai intro rumah sudah pernah dilihat
        GameProgress.MarkHouseIntroSeen(levelData.levelIndex);

        waitingForDialogue = false;

        // Setelah dialog intro selesai, langsung tampilkan popup level
        if (MapManager.Instance != null)
            MapManager.Instance.PrepareLevelPopup(this);
    }

    private bool IsPointerOverPureUI()
    {
        if (EventSystem.current == null) return false;

        Vector2 pos = Pointer.current != null ? Pointer.current.position.ReadValue() : Vector2.zero;
        var ped = new PointerEventData(EventSystem.current) { position = pos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        // TRUE hanya jika modulnya GraphicRaycaster (UI beneran)
        foreach (var r in results)
            if (r.module is GraphicRaycaster) return true;

        return false;
    }
    public void UpdateVisualState()
    {
        if (levelData == null) return;
        isUnlocked = GameProgress.IsLevelUnlocked(levelData.levelIndex);

        if (isUnlocked)
        {
            spriteRenderer.sprite = defaultSprite;
            UpdateStarsOnMap();
        }
        else
        {
            spriteRenderer.sprite = lockedSprite;
            if (starsDisplayContainer != null) starsDisplayContainer.SetActive(false);
        }
    }

    private void UpdateStarsOnMap()
    {
        if (starsDisplayContainer == null) return;

        starsDisplayContainer.SetActive(true);
        int completedStars = GameProgress.GetStars(levelData.levelIndex);

        for (int i = 0; i < starsDisplayContainer.transform.childCount; i++)
        {
            SpriteRenderer starRenderer = starsDisplayContainer.transform.GetChild(i).GetComponent<SpriteRenderer>();
            if (starRenderer == null) continue;

            starRenderer.gameObject.SetActive(true);

            if (i < completedStars)
            {
                starRenderer.sprite = starCompleteSprite;
            }
            else
            {
                starRenderer.sprite = starIncompleteSprite;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (isUnlocked)
        {
            if (spriteRenderer.sprite == defaultSprite)
            {
                spriteRenderer.sprite = highlightSprite;
            }
        }
    }
    private void OnMouseExit()
    {
        if (isUnlocked)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    private void OnDialogueFinished()
    {
        DialogueManager.Instance.OnDialogueEnd -= OnDialogueFinished;
        waitingForDialogue = false;

        MapManager.Instance.PrepareLevelPopup(this);
    }
}