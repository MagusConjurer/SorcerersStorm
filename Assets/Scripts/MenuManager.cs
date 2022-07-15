using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject MainMenu;
    private GameObject SettingsMenu;
    private bool reloadComplete = true;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

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

    private bool SetupPanels()
    {
        if(MainMenu == null || SettingsMenu == null)
        {
            MainMenu = GameObject.Find("MainMenuPanel");
            SettingsMenu = GameObject.Find("SettingsMenuPanel");
        }

        if (MainMenu != null && SettingsMenu != null)
        {
            SetupButtons(MainMenu);
            SetupButtons(SettingsMenu);

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
    private void SetupButtons(GameObject menu)
    {
        Button[] menuButtons = menu.GetComponentsInChildren<Button>();
        foreach (Button button in menuButtons)
        {
            if (button.gameObject.name == "PlayButton")
            {
                button.onClick.AddListener(LoadGame);
            }
            else if (button.gameObject.name == "SettingsButton")
            {
                button.onClick.AddListener(DisplaySettings);
            }
            else if (button.gameObject.name == "ReturnToMainButton")
            {
                button.onClick.AddListener(DisplayMainMenu);
            }
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
