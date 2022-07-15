using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    // Character Cards
    private List<Transform> characterRosterPositions;
    private List<Transform> characterTeamPositions;
    private List<CharacterCard> characterDeck;
    private List<CharacterCard> characterSelectedDeck;
    private CharacterCard currentCharacter;
    private bool encounterCharacterSelected;
    private bool gameLoaded;
    private bool[] availableCharacterTeamPositions;
    public bool fullTeamSelected;
    

    // GUI
    private GameObject rosterPanel;
    private GameObject boardPanel;
    private GameObject teamPanel;
    private GameObject bossPanel;
    private Slider turnTracker;
    private bool outOfTurns;
    private Button encounterButton;
    private Button rollButton;
    private Text instructionText;
    private Text resultText;

    // Encounters
    private bool inEncounter;
    private bool hasRolled;
    private List<EncounterCard> encounterDeck;
    private EncounterCard currentEncounter;
    private Transform encounterCardPosition;
    private Transform encounterCharacterPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "GameScene" && PanelsAreLoaded() == false)
        {
            InitialGameLoad();
        }
    }

    private void InitialGameLoad()
    {
        outOfTurns = false;
        encounterCharacterSelected = false;
        LoadCharacterCards();
        LoadEncounters();
        LoadGamePanels();
    }

    private bool PanelsAreLoaded()
    {
        return (rosterPanel != null && boardPanel != null && teamPanel != null && bossPanel != null);
    }

    /// <summary>
    /// Set up the list of character cards and their positions
    /// </summary>
    private void LoadCharacterCards()
    {
        /// Get the lists of roster and team positions, then remove their parent transform from the list
        characterRosterPositions = new List<Transform>();
        characterRosterPositions.AddRange(GameObject.Find("RosterPositions").GetComponentsInChildren<Transform>());
        characterRosterPositions.RemoveAt(0);
        
        characterTeamPositions = new List<Transform>();
        characterTeamPositions.AddRange(GameObject.Find("TeamPositions").GetComponentsInChildren<Transform>());
        characterTeamPositions.RemoveAt(0);
        
        characterDeck = new List<CharacterCard>();
        characterDeck.AddRange(GameObject.Find("CharacterCards").GetComponentsInChildren<CharacterCard>());
        for (int i = 0; i < characterDeck.Count; i++)
        {
            characterDeck[i].gameObject.SetActive(true);
            characterDeck[i].transform.position = characterRosterPositions[i].position;
        }

        availableCharacterTeamPositions = new bool[4];
        for (int j = 0; j < availableCharacterTeamPositions.Length; j++)
        {
            availableCharacterTeamPositions[j] = true;
        }

        characterSelectedDeck = new List<CharacterCard>();
        fullTeamSelected = false;
    }

    /// <summary>
    /// Find all the panels and setup their components
    /// </summary>
    private void LoadGamePanels()
    {
        rosterPanel = GameObject.Find("RosterPanel");
        boardPanel = GameObject.Find("BoardPanel");
        teamPanel = GameObject.Find("TeamPanel");
        bossPanel = GameObject.Find("BossPanel");

        turnTracker = boardPanel.GetComponentInChildren<Slider>();
        instructionText = GameObject.Find("InstructionText").GetComponent<Text>();
        resultText = GameObject.Find("ResultText").GetComponent<Text>();

        Button[] boardButtons = boardPanel.GetComponentsInChildren<Button>();
        foreach(Button button in boardButtons)
        {
            button.onClick.RemoveAllListeners();
            if (button.name == "EncounterButton")
            {
                encounterButton = button;
                button.onClick.AddListener(DrawEncounter);
                button.enabled = true;
            }
            else if(button.name == "DiceRollButton")
            {
                rollButton = button;
                button.onClick.AddListener(HandleRoll);
                button.enabled = false;
            }
        }

        DisplayRoster();
    }

    private void LoadEncounters()
    {
        encounterDeck = new List<EncounterCard>();
        encounterCardPosition = GameObject.Find("EncounterCardPosition").GetComponent<Transform>();
        encounterCharacterPosition = GameObject.Find("EncounterCharacterPosition").GetComponent<Transform>();

        encounterDeck.AddRange(GameObject.Find("Encounters").GetComponentsInChildren<EncounterCard>());
        foreach (EncounterCard card in encounterDeck)
        {
            card.gameObject.SetActive(false);
        }
    }

    private void LoadBossEncounter()
    {
        rosterPanel.SetActive(false);
        boardPanel.SetActive(false);
        bossPanel.SetActive(true);

        Text bossText = bossPanel.GetComponentInChildren<Text>();
        bossText.text = "BOSS: The Sorcerer";
    }

    private void DisplayRoster()
    {
        rosterPanel.SetActive(true);
        boardPanel.SetActive(false);
        bossPanel.SetActive(false);
    }

    private void DisplayBoard()
    {
        rosterPanel.SetActive(false);
        boardPanel.SetActive(true);
        bossPanel.SetActive(false);
    }

    private void UpdateButtons()
    {
        encounterButton.enabled = (!inEncounter && !outOfTurns);
        rollButton.enabled = (inEncounter && encounterCharacterSelected && !hasRolled);
    }

    /// <summary>
    /// Increments the turn timer by the given amount.
    /// </summary>
    /// <param name="amount">The amount to increment the turn tracker by</param>
    /// <returns>True if the tracker has reached the maximum</returns>
    private bool IncrementTurnTracker(int amount)
    {
        if((turnTracker.value + amount) < turnTracker.maxValue)
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

    private void DisplayEncounterResult(int rollValue, string description)
    {
        resultText.text = $"You rolled a {rollValue} and {description}";
    }

    /// <summary>
    /// If there are still turns available, pull a random card out of the encounter deck and display it.
    /// </summary>
    private void DrawEncounter()
    {
        if(outOfTurns == false)
        {
            int randIndex = Random.Range(0, encounterDeck.Count);
            currentEncounter = encounterDeck[randIndex];
            encounterDeck.RemoveAt(randIndex);
            currentEncounter.gameObject.SetActive(true);
            currentEncounter.transform.position = encounterCardPosition.position;
            inEncounter = true;
            hasRolled = false;
            instructionText.text = "Choose a Character";
            UpdateButtons();
        }
    }

    /// <summary>
    /// Removes the currentEncounter card from the board
    /// </summary>
    private void EndEncounter()
    {
        inEncounter = false;
        encounterCharacterSelected = false;
        MoveToTeamPosition(currentCharacter);
        currentCharacter.UnselectCharacter();
        outOfTurns = IncrementTurnTracker(1);
        currentEncounter.gameObject.SetActive(false);
        if (outOfTurns == false)
        {
            instructionText.text = "Draw an Encounter";
            resultText.text = "";
            UpdateButtons();
        }
        else
        {
            Invoke(nameof(LoadBossEncounter), .5f);
        }
    }

    private void HandleRoll()
    {
        if(inEncounter == true && encounterCharacterSelected == true && hasRolled == false)
        {
            hasRolled = true;
            // Dice roll + character stat
            int totalRollValue = Random.Range(1, 6) + GetEncounterStat();
            bool isWin = currentEncounter.IsResultWin(totalRollValue);
            string resultAction = currentEncounter.ResultAction(isWin);
            string resultDescription = HandleEncounterAction(resultAction, isWin);
            DisplayEncounterResult(totalRollValue, resultDescription);
            UpdateButtons();
            Invoke(nameof(EndEncounter), 1.0f);
        }
    }

    private string HandleEncounterAction(string actionType, bool isWin)
    {
        int amount = currentEncounter.ResultAmount(isWin);
        switch (actionType)
        {
            case "Item":
                return $"gained {amount} item.";
            case "Health":
                currentCharacter.DecreaseHealth(amount);
                return $"lost {amount} health.";
            case "Strength":
                currentCharacter.DecreaseStrength(amount);
                return $"lost {amount} strength.";
            default:
                return "lost nothing.";
        }
    }

    private int GetEncounterStat()
    {
        switch (currentEncounter.GetEncounterType())
        {
            case "Enemy":
                return currentCharacter.GetStrength();
            case "Trap":
                return 0;
            case "SkillCheck":
                return 0;
            case "Unlockable":
                return 0;
        }

        return 0;
    }

    /// <summary>
    /// Called when a card is clicked on
    /// </summary>
    /// <param name="characterCard"></param>
    public void AddToTeam(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
            characterCard.inTeam = true;
            characterCard.SelectCharacter();
            if (characterSelectedDeck.Count == 4)
            {
                fullTeamSelected = true;
                Invoke(nameof(MoveAllToTeamPositions), .5f);
                Invoke(nameof(DisplayBoard), .75f);
            }
        }
    }

    /// <summary>
    /// Moves the selected characters to their corresponding team positions.
    /// </summary>
    private void MoveAllToTeamPositions()
    {
        for (int i = 0; i < availableCharacterTeamPositions.Length; i++)
        {
            CharacterCard characterCard = characterSelectedDeck[i];
            if (availableCharacterTeamPositions[i] == true)
            {
                characterCard.placedIndex = i;
                MoveToTeamPosition(characterCard);
                characterCard.UnselectCharacter();

                availableCharacterTeamPositions[i] = false;
                characterDeck.Remove(characterCard);
            }
        }
    }

    /// <summary>
    /// Moves a character card to it's placed index position. 
    /// 
    /// Only use after MoveAllToTeamPositions has been called.
    /// </summary>
    private void MoveToTeamPosition(CharacterCard character)
    {
        character.transform.position = characterTeamPositions[character.placedIndex].position;
    }

    private void MoveToEncounterPosition(CharacterCard character)
    {
        character.transform.position = encounterCharacterPosition.position;
    }

    public void SetCurrentCharacter(CharacterCard characterCard)
    {
        if (inEncounter == true && encounterCharacterSelected == false)
        {
            currentCharacter = characterCard;
            characterCard.SelectCharacter();
            encounterCharacterSelected = true;
            MoveToEncounterPosition(currentCharacter);
            instructionText.text = "Roll the Dice";
            UpdateButtons();
        }
    }
}
