using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    public void OnClick_OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnClick_CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

}
