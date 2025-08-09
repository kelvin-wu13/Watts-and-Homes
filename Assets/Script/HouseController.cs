using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (isHovering)
            {
                isHovering = false;
                spriteRenderer.sprite = defaultSprite;
            }
            return;
        }

        if (!isUnlocked) return;

        RaycastHit2D hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Pointer.current.position.ReadValue()));

        bool mouseIsOver = (hit.collider != null && hit.collider.gameObject == this.gameObject);

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

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (waitingForDialogue) return;

            if (levelIntroDialogue != null && levelIntroDialogue.dialogueLines.Count > 0)
            {
                waitingForDialogue = true;
                DialogueManager.Instance.OnDialogueEnd += OnHouseIntroFinished;
                levelIntroDialogue.TriggerDialogue();
            }
            else
            {
                MapManager.Instance.PrepareLevelPopup(this);
            }
        }
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
    private void OnMouseDown()
    {
        if (!isUnlocked) return;
        if (waitingForDialogue) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsRunning) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (levelIntroDialogue != null && levelIntroDialogue.dialogueLines != null && levelIntroDialogue.dialogueLines.Count > 0 && !GameProgress.HasSeenHouseIntro(levelData.levelIndex))
        {
            waitingForDialogue = true;
            DialogueManager.Instance.OnDialogueEnd += OnHouseIntroFinished;
            levelIntroDialogue.TriggerDialogue();
        }
        else
        {
            MapManager.Instance.PrepareLevelPopup(this);
        }
    }
    private void OnHouseIntroFinished()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnDialogueEnd -= OnHouseIntroFinished;
        }

        GameProgress.MarkHouseIntroSeen(levelData.levelIndex);
        waitingForDialogue = false;
        MapManager.Instance.PrepareLevelPopup(this);
    }

    private void OnDialogueFinished()
    {
        DialogueManager.Instance.OnDialogueEnd -= OnDialogueFinished;
        waitingForDialogue = false;

        MapManager.Instance.PrepareLevelPopup(this);
    }
}