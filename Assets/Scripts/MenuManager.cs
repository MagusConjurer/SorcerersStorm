using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    private GraphicsManager graphicsManager;
    private GameObject MainMenu;
    private GameObject SettingsMenu;
    private Dropdown resDropdown;
    private Toggle fsToggle;
    private bool reloadComplete = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        graphicsManager = gameManager.GetComponent<GraphicsManager>();

        reloadComplete = SetupPanels();
    }

    // Update is called once per frame
    void Update()
    {
        if(reloadComplete == false)
        {
            reloadComplete = SetupPanels();
        }
    }

    public void DisplaySettings()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    public void DisplayMainMenu()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private bool SetupPanels()
    {
        if(MainMenu == null || SettingsMenu == null)
        {
            MainMenu = GameObject.Find("MainMenuPanel");
            SettingsMenu = GameObject.Find("SettingsMenuPanel");
        }

        if (MainMenu != null && SettingsMenu != null)
        {
            SetupMenuScripts(MainMenu);
            SetupMenuScripts(SettingsMenu);

            MainMenu.SetActive(true);
            SettingsMenu.SetActive(false);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Assign the OnClick functions to the buttons. Primarily for when the MainMenu scene loads from another scene.
    /// </summary>
    /// <param name="menu">A UI panel game object with button children</param>
    private void SetupMenuScripts(GameObject menu)
    {
        Button[] menuButtons = menu.GetComponentsInChildren<Button>();
        foreach (Button button in menuButtons)
        {
            switch(button.gameObject.name)
            {
                case ("PlayButton"):
                    button.onClick.AddListener(LoadGame);
                    break;
                case ("SettingsButton"):
                    button.onClick.AddListener(DisplaySettings);
                    break;
                case ("ReturnToMainButton"):
                    button.onClick.AddListener(DisplayMainMenu);
                    break;
                case ("ExitButton"):
                    button.onClick.AddListener(ExitGame);
                    break;
            }
        }

        if(menu.name == "SettingsMenuPanel")
        {
            resDropdown = GameObject.Find("ResolutionDropdown").GetComponent<Dropdown>();
            fsToggle = GameObject.Find("FullScreenToggle").GetComponent<Toggle>();

            resDropdown.onValueChanged.AddListener(delegate { graphicsManager.SetResolution(resDropdown); });
            fsToggle.onValueChanged.AddListener(delegate { graphicsManager.SetFullscreen(fsToggle); });
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    /// <summary>
    /// Used to call methods after the scene is loaded
    /// 
    /// Following the example from: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "GameScene")
        {
            Button mainMenuButton = GameObject.Find("LoadMainMenuButton").GetComponent<Button>();
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
        if(scene.name == "MainMenu")
        {
            reloadComplete = false;
        }
    }
}
