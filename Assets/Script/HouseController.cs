using UnityEngine;

public class HouseController : MonoBehaviour
{
    public LevelData levelData;

    public Sprite highlightSprite;
    public Sprite lockedSprite;
    public GameObject starsDisplayContainer;

    private Sprite defaultSprite;
    private SpriteRenderer spriteRenderer;
    private bool isUnlocked = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
        UpdateVisualState();
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
        int starCount = GameProgress.GetStars(levelData.levelIndex);
        for (int i = 0; i < starsDisplayContainer.transform.childCount; i++)
        {
            starsDisplayContainer.transform.GetChild(i).gameObject.SetActive(i < starCount);
        }
    }

    private void OnMouseEnter()
    {
        if (isUnlocked)
        {
            spriteRenderer.sprite = highlightSprite;
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
