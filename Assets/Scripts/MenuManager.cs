using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameObject MainMenu;
    private GameObject SettingsMenu;
    // Start is called before the first frame update
    void Start()
    {
        MainMenu = GameObject.Find("MainMenuPanel");
        SettingsMenu = GameObject.Find("SettingsMenuPanel");

        if(MainMenu != null && SettingsMenu != null)
        {
            MainMenu.SetActive(true);
            SettingsMenu.SetActive(false);
        }

        // TODO: Implement setting button TaskOnClick for when the menu is loaded from another scene
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
}
