using UnityEngine;
using UnityEngine.InputSystem;

public class HouseController : MonoBehaviour
{
    public LevelData levelData;

    public Sprite highlightSprite;
    public Sprite lockedSprite;
    public Sprite starCompleteSprite;
    public Sprite starIncompleteSprite;

    public GameObject starsDisplayContainer;

    private Sprite defaultSprite;
    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    private Camera mainCamera;
    private bool isHovering = false;

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
            MapManager.Instance.ShowLevelPopup(this);
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
        if (isUnlocked)
        {
            MapManager.Instance.ShowLevelPopup(this);
        }
    }
}