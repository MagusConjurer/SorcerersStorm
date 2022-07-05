using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameObject gameManager;
    private GameObject MainMenu;
    private GameObject SettingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        MainMenu = GameObject.Find("MainMenuPanel");
        SettingsMenu = GameObject.Find("SettingsMenuPanel");

        if(MainMenu != null && SettingsMenu != null)
        {
            SetupButtons(MainMenu);
            SetupButtons(SettingsMenu);

            MainMenu.SetActive(true);
            SettingsMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
