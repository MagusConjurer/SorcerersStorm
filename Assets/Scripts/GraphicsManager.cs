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

    public void SetResolution()
    {
        Dropdown resDropdown = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>();
        switch (resDropdown.options[resDropdown.value].text)
        {
            case "1920x1080":
                width = 1920;
                height = 1080;
                break;
            case "1080x720":
                width = 1080;
                height = 720;
                break;
            case "720x480":
                width = 720;
                height = 480;
                break;
        }

        Screen.SetResolution(width, height, fullscreen);
    }

    public void SetFullscreen()
    {
        Toggle fsToggle = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();
        fullscreen = fsToggle.isOn;

        Screen.SetResolution(width, height, fullscreen);
    }
}
