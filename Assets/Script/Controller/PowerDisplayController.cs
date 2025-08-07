using UnityEngine;
using TMPro;

public class PowerDisplayController : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    public bool ignoreRaycasts = true;

    private IPowerDataProvider dataProvider;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        dataProvider = GetComponent<IPowerDataProvider>();

        canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (dataProvider == null)
        {
            if (displayText != null) displayText.gameObject.SetActive(false);
            this.enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (displayText == null)
        {
            this.enabled = false;
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = !ignoreRaycasts;
        }
    }

    private void LateUpdate()
    {
        if (dataProvider != null && displayText != null)
        {
            displayText.text = dataProvider.GetPowerDisplayText();
        }
    }
}