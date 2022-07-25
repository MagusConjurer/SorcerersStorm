using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    UIManager uiManager;

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

    // Encounters
    private bool outOfTurns;
    private bool inEncounter;
    private bool hasRolled;
    private List<EncounterCard> encounterDeck;
    private EncounterCard currentEncounter;
    private Transform encounterCardPosition;
    private Transform encounterCharacterPosition;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.Find("GameManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "GameScene" && uiManager.GamePanelsAreLoaded() == false)
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
        uiManager.LoadGamePanels();
        uiManager.DisplayRosterPanel();
    }

    /// <summary>
    /// Set up the list of encounter cards and their positions
    /// </summary>
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

    /// <summary>
    /// Call the matching uiManager method, allowing for use with Invoke
    /// </summary>
    private void DisplayBoard()
    {
        uiManager.DisplayBoardPanel();
    }

    /// <summary>
    /// Call the matching uiManager method, allowing for use with Invoke
    /// </summary>
    private void DisplayBoss()
    {
        uiManager.DisplayBossPanel();
    }

    /// <summary>
    /// Enables/disable the card and dice buttons based on the state of the game
    /// </summary>
    private void UpdateButtons()
    {
        uiManager.UpdateGameButtons(inEncounter, outOfTurns, encounterCharacterSelected, hasRolled);
    }

    /// <summary>
    /// Update the outOfTurns bool based on the incremented tracker
    /// </summary>
    /// <param name="amount"></param>
    private void IncrementTurnTracker(int amount)
    {
        outOfTurns = uiManager.IncrementTurnTracker(amount);
    }

    private void DisplayEncounterResult(int rollValue, string description)
    {
        uiManager.UpdateResultText($"You rolled a {rollValue} and {description}");
    }

    /// <summary>
    /// If there are still turns available, pull a random card out of the encounter deck and display it.
    /// </summary>
    public void DrawEncounter()
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
            uiManager.UpdateInstructionText("Choose a Character");
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
        IncrementTurnTracker(1);
        currentEncounter.gameObject.SetActive(false);
        if (outOfTurns == false)
        {
            uiManager.UpdateInstructionText("Draw an Encounter");
            uiManager.UpdateResultText("");
            UpdateButtons();
        }
        else
        {
            Invoke(nameof(DisplayBoss), .5f);
        }
    }

    public void HandleRoll()
    {
        if(inEncounter == true && encounterCharacterSelected == true && hasRolled == false)
        {
            hasRolled = true;
            // Dice roll + character stat
            int totalRollValue = Random.Range(1, 6) + GetEncounterStatValue();
            bool isWin = currentEncounter.IsResultWin(totalRollValue);
            string resultAction = (isWin == true ? currentEncounter.GetWinAction() : currentEncounter.GetLossAction());
            string resultDescription = HandleEncounterAction(resultAction, isWin);
            DisplayEncounterResult(totalRollValue, resultDescription);
            UpdateButtons();
            Invoke(nameof(EndEncounter), 2.0f);
        }
    }

    private string HandleEncounterAction(string actionType, bool isWin)
    {
        int amount = currentEncounter.ResultAmount(isWin);
        switch (actionType)
        {
            case "Turn":
                if(isWin)
                {
                    return $"are now {amount} turn(s) closer.";
                }
                else
                {
                    return $"now it's going to take an extra {amount} turn(s).";
                }
            case "Health":
                currentCharacter.DecreaseHealth(amount);
                return $"lost {amount} health.";
            case "Strength":
                currentCharacter.DecreaseStrength(amount);
                return $"lost {amount} strength.";
            case "Accuracy":
                currentCharacter.DecreaseAccuracy(amount);
                return $"lost {amount} accuracy.";
            case "Stealth":
                currentCharacter.DecreaseStealth(amount);
                return $"lost {amount} stealth.";
            default:
                return "lost nothing.";
        }
    }

    private int GetEncounterStatValue()
    {
        switch (currentEncounter.GetEncounterType())
        {
            case "Enemy":
                string statType = currentEncounter.GetWinAction();
                if (statType == "Strength")
                {
                    return currentCharacter.GetStrength();
                }
                else if (statType == "Accuracy")
                {
                    return currentCharacter.GetAccuracy();
                }
                else
                {
                    return currentCharacter.GetStealth();
                }
            case "Trap":
                return 0;
            case "Item":
                // TODO: Implement item storage for keys
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
            uiManager.UpdateInstructionText("Roll the Dice");
            UpdateButtons();
        }
    }
}
