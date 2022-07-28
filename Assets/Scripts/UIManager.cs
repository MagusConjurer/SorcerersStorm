using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    /// Main Menu Scene
    private GameManager gameManager;
    private GraphicsManager graphicsManager;
    private GameObject MainMenu;
    private GameObject SettingsMenu;
    private Dropdown resDropdown;
    private Toggle fsToggle;
    private bool reloadComplete = true;

    /// Game Scene
    private CardManager cardManager;
    private GameObject rosterPanel;
    private GameObject boardPanel;
    private GameObject teamPanel;
    private GameObject bossPanel;
    private Slider turnTracker;
    private Button encounterButton;
    private Button rollButton;
    private Button confirmItemButton;
    private Text instructionText;
    private Text resultText;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        graphicsManager = gameManager.GetComponent<GraphicsManager>();
        cardManager = gameManager.GetComponent<CardManager>();

        reloadComplete = SetupMainMenuPanels();
    }

    // Update is called once per frame
    void Update()
    {
        if(reloadComplete == false)
        {
            reloadComplete = SetupMainMenuPanels();
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

    private bool SetupMainMenuPanels()
    {
        if(MainMenu == null || SettingsMenu == null)
        {
            MainMenu = GameObject.Find("MainMenuPanel");
            SettingsMenu = GameObject.Find("SettingsMenuPanel");
        }

        if (MainMenu != null && SettingsMenu != null)
        {
            SetupMainMenuScripts(MainMenu);
            SetupMainMenuScripts(SettingsMenu);

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
    private void SetupMainMenuScripts(GameObject menu)
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

            graphicsManager.SetResolution(resDropdown);
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

    public bool GamePanelsAreLoaded()
    {
        return (rosterPanel != null && boardPanel != null && teamPanel != null && bossPanel != null);
    }

    public void LoadGamePanels()
    {
        rosterPanel = GameObject.Find("RosterPanel");
        boardPanel = GameObject.Find("BoardPanel");
        teamPanel = GameObject.Find("TeamPanel");
        bossPanel = GameObject.Find("BossPanel");

        turnTracker = boardPanel.GetComponentInChildren<Slider>();
        instructionText = GameObject.Find("InstructionText").GetComponent<Text>();
        resultText = GameObject.Find("ResultText").GetComponent<Text>();

        Button[] boardButtons = boardPanel.GetComponentsInChildren<Button>();
        foreach (Button button in boardButtons)
        {
            button.onClick.RemoveAllListeners();
            if (button.name == "EncounterButton")
            {
                encounterButton = button;
                button.onClick.AddListener(cardManager.DrawEncounter);
                button.enabled = true;
            }
            else if (button.name == "DiceRollButton")
            {
                rollButton = button;
                button.onClick.AddListener(cardManager.HandleRoll);
                button.enabled = false;
            }
            else if (button.name == "ConfirmItemButton")
            {
                confirmItemButton = button;
                button.onClick.AddListener(cardManager.ConfirmItem);
                button.enabled = false;
                confirmItemButton.gameObject.SetActive(false);
            }
        }
    }

    public void DisplayRosterPanel()
    {
        try
        {
            rosterPanel.SetActive(true);
            boardPanel.SetActive(false);
            bossPanel.SetActive(false);
        }
        catch
        {
            LoadGamePanels();
            DisplayRosterPanel();
        }
    }

    public void DisplayBoardPanel()
    {
        try
        {
            rosterPanel.SetActive(false);
            boardPanel.SetActive(true);
            bossPanel.SetActive(false);
        }
        catch
        {
            LoadGamePanels();
            DisplayBoardPanel();
        }
    }

    public void DisplayBossPanel()
    {
        try
        {
            rosterPanel.SetActive(false);
            boardPanel.SetActive(false);
            bossPanel.SetActive(true);

            Text bossText = bossPanel.GetComponentInChildren<Text>();
            bossText.text = "BOSS: The Sorcerer";
        }
        catch
        {

            LoadGamePanels();
            DisplayBossPanel();
        }
    }

    public void UpdateGameButtons(bool canDrawEncounter, bool canRoll, bool needsToConfirmItem)
    {
        encounterButton.enabled = canDrawEncounter;
        rollButton.enabled = canRoll;
        confirmItemButton.enabled = needsToConfirmItem;
        confirmItemButton.gameObject.SetActive(needsToConfirmItem);
    }

    /// <summary>
    /// Increments the turn timer by the given amount.
    /// </summary>
    /// <param name="amount">The amount to increment the turn tracker by</param>
    /// <returns>True if the tracker has reached the maximum</returns>
    public bool IncrementTurnTracker(int amount)
    {
        if ((turnTracker.value + amount) < turnTracker.maxValue)
        {
            turnTracker.value += amount;
            return false;
        }
        else
        {
            turnTracker.value = turnTracker.maxValue;
            return true;
        }
    }

    public void UpdateResultText(string newText)
    {
        resultText.text = newText;
    }

    public void UpdateInstructionText(string newText)
    {
        instructionText.text = newText;
    }
}
