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
    private Toggle nextTurnToggle;
    private bool nextTurnButtonEnabled;
    private bool reloadComplete = true;

    /// Game Scene
    private CardManager cardManager;
    /// Team & Characters
    private GameObject rosterPanel;
    private Text rosterText;
    private Button confirmTeamButton;

    private GameObject teamPanel;
    private Text keyCountText;
    private int currentKeyCount;

    /// Board
    private GameObject boardPanel;
    private Slider turnTracker;
    private Button encounterButton;
    private Button rollButton;
    private Button confirmItemButton;
    private Button nextTurnButton;
    private Text instructionText;

    /// Boss
    private GameObject bossPanel;
    private Image[] bossHealthBar;
    private Button bossEncounterButton;
    private Button bossRollButton;
    private Button mainMenuButton;
    private Text bossInstructionText;

    /// Colors
    private Color sorcererPurple;
    private Color sorcererDarkGray;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        graphicsManager = gameManager.GetComponent<GraphicsManager>();
        cardManager = gameManager.GetComponent<CardManager>();

        reloadComplete = SetupMainMenuPanels();

        sorcererPurple   = new Color(0.467f, 0.235f, 0.325f, 1.0f); // RGBA: 119,60,83,255
        sorcererDarkGray = new Color(0.275f, 0.251f, 0.247f, 1.0f); // RGBA: 70,64,63,255
    }

    // Update is called once per frame
    void Update()
    {
        if(reloadComplete == false)
        {
            reloadComplete = SetupMainMenuPanels();
        }
    }

    /// <summary>
    /// Makes only the SettingsMenu panel active.
    /// </summary>
    public void DisplaySettings()
    {
        MainMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }

    /// <summary>
    /// Makes only the MainMenu panel active
    /// </summary>
    public void DisplayMainMenu()
    {
        MainMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }

    /// <summary>
    /// Loads the GameScene scene
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// Loads the MainMenu scene
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Closes the application
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Finds and initializes the main menu and settings panels.
    /// </summary>
    /// <returns></returns>
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
            nextTurnToggle = GameObject.Find("NextTurnToggle").GetComponent<Toggle>();

            resDropdown.onValueChanged.AddListener(delegate { graphicsManager.SetResolution(resDropdown); });
            fsToggle.onValueChanged.AddListener(delegate { graphicsManager.SetFullscreen(fsToggle); });
            nextTurnToggle.onValueChanged.AddListener(delegate { SetNextTurnButtonEnabled(nextTurnToggle); });

            graphicsManager.SetResolution(resDropdown);
            SetNextTurnButtonEnabled(nextTurnToggle);
        }
    }

    /// <summary>
    /// Updates the NextTurnButtonEnabled variable based on the status of the settings menu toggle
    /// </summary>
    /// <param name="ntToggle"></param>
    private void SetNextTurnButtonEnabled(Toggle ntToggle)
    {
        nextTurnButtonEnabled = ntToggle.isOn;
    }

    /// <summary>
    /// Used by the card manager to see whether the next turn button should be used.
    /// </summary>
    /// <returns>True if enabled</returns>
    public bool NextTurnButtonIsEnabled()
    {
        return nextTurnButtonEnabled;
    }

    /// <summary>
    /// Used to make the Next Turn button visible
    /// </summary>
    public void DisplayNextTurnButton(bool shouldDisplay)
    {
        nextTurnButton.gameObject.SetActive(shouldDisplay);
        CanClickNextTurn(shouldDisplay);
    }

    /// <summary>
    /// Used to enable the Next Turn button
    /// </summary>
    /// <param name="allowed"></param>
    public void CanClickNextTurn(bool allowed)
    {
        nextTurnButton.enabled = allowed;
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
            mainMenuButton = GameObject.Find("LoadMainMenuButton").GetComponent<Button>();
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
        if(scene.name == "MainMenu")
        {
            reloadComplete = false;
        }
    }

    /// <summary>
    /// Check that all game panels have been loaded
    /// </summary>
    /// <returns>True if none of the panels are null</returns>
    public bool GamePanelsAreLoaded()
    {
        return (rosterPanel != null && boardPanel != null && teamPanel != null && bossPanel != null);
    }

    /// <summary>
    /// Finds and initializes all of the necessary game objects and variables for the game panels.
    /// </summary>
    public void LoadGamePanels()
    {
        rosterPanel = GameObject.Find("RosterPanel");
        boardPanel = GameObject.Find("BoardPanel");
        teamPanel = GameObject.Find("TeamPanel");
        bossPanel = GameObject.Find("BossPanel");

        rosterText = GameObject.Find("RosterText").GetComponent<Text>();
        confirmTeamButton = GameObject.Find("ConfirmTeamButton").GetComponent<Button>();
        confirmTeamButton.onClick.RemoveAllListeners();
        confirmTeamButton.onClick.AddListener(cardManager.ConfirmTeam);
        CanConfirmTeam(false);

        turnTracker = boardPanel.GetComponentInChildren<Slider>();

        instructionText = GameObject.Find("InstructionText").GetComponent<Text>();
        instructionText.text = "Draw an Encounter";
        keyCountText = GameObject.Find("KeyCountText").GetComponent<Text>();
        currentKeyCount = 0;

        bossInstructionText = GameObject.Find("BossInstructionText").GetComponent<Text>();
        bossHealthBar = GameObject.Find("BossHealthBar").GetComponentsInChildren<Image>();
        foreach(Image bar in bossHealthBar)
        {
            bar.color = sorcererPurple;
        }

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
            else if (button.name == "NextTurnButton")
            {
                nextTurnButton = button;
                button.onClick.AddListener(delegate { DisplayNextTurnButton(false); cardManager.EndEncounterDelayed(); });
                button.enabled = false;
                nextTurnButton.gameObject.SetActive(false);
            }
        }

        Button[] bossButtons = bossPanel.GetComponentsInChildren<Button>();
        foreach (Button button in bossButtons)
        {
            button.onClick.RemoveAllListeners();
            if (button.name == "BossEncounterButton")
            {
                bossEncounterButton = button;
                button.onClick.AddListener(cardManager.DrawBossEncounter);
                button.enabled = false;
            }
            else if (button.name == "BossDiceRollButton")
            {
                bossRollButton = button;
                button.onClick.AddListener(cardManager.HandleRoll);
                button.enabled = false;
            }
            else if (button.name == "LoadMainMenuButton")
            {
                mainMenuButton = button;
                button.onClick.AddListener(LoadMainMenu);
                button.enabled = false;
            }
        }
    }

    /// <summary>
    /// Makes only the Roster panel active
    /// </summary>
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

    /// <summary>
    /// Makes only the Board panel active
    /// </summary>
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

    /// <summary>
    /// Makes only the Boss panel active and sets the boss title text
    /// </summary>
    public void DisplayBossPanel(Boss boss)
    {
        try
        {
            rosterPanel.SetActive(false);
            boardPanel.SetActive(false);
            bossPanel.SetActive(true);

            Text bossText = GameObject.Find("BossTitleText").GetComponent<Text>();
            bossText.text = $"BOSS: {boss.GetBossName()}";

            boss.Activate();
            nextTurnButton.transform.SetParent(bossPanel.transform, true);
        }
        catch
        {
            LoadGamePanels();
            DisplayBossPanel(boss);
        }
    }

    /// <summary>
    /// Method called during the team selection phase to enable/disable the confirm button
    /// </summary>
    /// <param name="status"></param>
    public void CanConfirmTeam(bool status)
    {
        confirmTeamButton.gameObject.SetActive(status);
        confirmTeamButton.enabled = status;

        if(status)
        {
            UpdateRosterText("Confirm or Change Your Selection");
        } 
        else
        {
            UpdateRosterText("Select Your Four Characters");
        }
    }

    /// <summary>
    /// Changes the state of the regular Draw Encounter button
    /// </summary>
    /// <param name="canDraw">Whether the button should be enabled</param>
    public void EnableDrawEncounterButton(bool canDraw)
    {
        encounterButton.enabled = canDraw;
    }
    /// <summary>
    /// Changes the state of the regular Roll button
    /// </summary>
    /// <param name="canDraw">Whether the button should be enabled</param>
    public void EnableRollButton(bool canRoll)
    {
        rollButton.enabled = canRoll;
    }
    /// <summary>
    /// Changes the state of the Confirm button during item encounters
    /// </summary>
    /// <param name="canDraw">Whether the button should be enabled/visible</param>
    public void EnableItemConfirmButton(bool canConfirmItem)
    {
        confirmItemButton.enabled = canConfirmItem;
        confirmItemButton.gameObject.SetActive(canConfirmItem);
    }
    /// <summary>
    /// Changes the state of the Draw Encounter button during the Boss 
    /// </summary>
    /// <param name="canDraw">Whether the button should be enabled</param>
    public void EnableBossEncounterButton(bool canBossDraw)
    {
        bossEncounterButton.enabled = canBossDraw;
    }
    /// <summary>
    /// Changes the state of the Roll button during the Boss
    /// </summary>
    /// <param name="canDraw">Whether the button should be enabled</param>
    public void EnableBossRollButton(bool canBossRoll)
    {
        bossRollButton.enabled = canBossRoll;
    }

    /// <summary>
    /// Update whether the main menu button on the main menu is enabled and visible.
    /// </summary>
    public void EnableBossMainMenuButton(bool isEnabled)
    {
        mainMenuButton.enabled = isEnabled;
        mainMenuButton.gameObject.SetActive(isEnabled);
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

    public void UpdateRosterText(string newText)
    {
        rosterText.text = newText;
    }

    /// <summary>
    /// Used to update the instruction text during encounters
    /// </summary>
    public void UpdateInstructionText(string newText)
    {
        instructionText.text = newText;
    }

    /// <summary>
    /// Used to update the instruction text during boss encounters.
    /// Only updates if the boss is currently on the board.
    /// </summary>
    public void UpdateBossInstructionText(string newText, Boss boss)
    {
        if(boss.IsOnTheBoard())
        {
            bossInstructionText.text = newText;
        }
    }

    /// <summary>
    /// Used to update the number of keys the player has
    /// </summary>
    /// <param name="toAdd">Amount to add to the key count</param>
    public void UpdateKeyCountText(int toAdd)
    {
        currentKeyCount += toAdd;
        keyCountText.text = $"{currentKeyCount}";
    }

    /// <summary>
    /// Returns the current key count
    /// </summary>
    public int GetKeyCount()
    {
        return currentKeyCount;
    }



    /// <summary>
    /// Method to update the boss health bar.
    /// </summary>
    /// <param name="amount">Damage to apply</param>
    public void UpdateBossHealthbar(int amount, Boss boss)
    {
        boss.DecreaseBossHealth(amount);
        int currentBossHealth = boss.GetBossHealth();
        if (currentBossHealth >= 0)
        {
            int startIndex = currentBossHealth >= 0 ? currentBossHealth : 0;
            int finalIndex = startIndex + (amount);
            for (int i = startIndex; i < finalIndex; i++)
            {
                bossHealthBar[i].color = sorcererDarkGray;
            }
        }
    }

}
