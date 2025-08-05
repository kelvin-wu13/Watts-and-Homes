using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject backgroundBlocker;

    private void Start()
    {
        if (mainPanel == null)
        {
            mainPanel.SetActive(false);
        }
        if (backgroundBlocker != null)
        {
            backgroundBlocker.SetActive(false);
        }
    }

    private void Update()
    {
        if (mainPanel != null && mainPanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
        }
    }

    public void ShowPanel()
    {
        if (mainPanel != null && backgroundBlocker != null)
        {
            backgroundBlocker.SetActive(true);
            mainPanel.SetActive(true);
        }
    }

    public void HidePanel()
    {
        if (mainPanel != null && backgroundBlocker != null)
        {
            mainPanel.SetActive(false);
            backgroundBlocker.SetActive(false);
        }
    }
}
