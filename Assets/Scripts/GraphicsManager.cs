using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    // Default values 
    private int width = 1920;
    private int height = 1080;
    private bool fullscreen = false;

    public void SetResolution(Dropdown resolutionDropdown)
    {
        switch (resolutionDropdown.options[resolutionDropdown.value].text)
        {
            case "1920x1080":
                width = 1920;
                height = 1080;
                break;
        }

        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetFullscreen(Toggle fullscreenToggle)
    {
        fullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(width, height, fullscreen);
    }
}
