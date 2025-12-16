using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown resDropdown;
    public Toggle fullscreenToggle;
    Resolution[] allResolutions;
    bool isFullscreen;
    int selectedResolution;
    void Start()
    {
        isFullscreen = true;
        allResolutions = Screen.resolutions;

        List<string> resolutionsList = new List<string>();
        foreach (Resolution resolution in allResolutions)
        {
            resolutionsList.Add(resolution.ToString());
        }
        resDropdown.AddOptions(resolutionsList);
    }
}
