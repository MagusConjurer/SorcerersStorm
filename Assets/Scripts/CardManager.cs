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
    private bool inEnemyEncounter;
    private bool inItemEncounter;
    private bool encounterItemSelected;
    private bool hasRolled;
    private List<EncounterCard> encounterDeck;
    private List<EncounterCard> itemDeck;
    private EncounterCard currentEncounter;
    private Transform encounterCardPosition;
    private Transform encounterCharacterPosition;
    private EncounterCard firstItem;
    private EncounterCard secondItem;
    private Transform firstItemCardPosition;
    private Transform secondItemCardPosition;

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

        itemDeck = new List<EncounterCard>();
        firstItemCardPosition = GameObject.Find("FirstItemCardPosition").GetComponent<Transform>();
        secondItemCardPosition = GameObject.Find("SecondItemCardPosition").GetComponent<Transform>();

        encounterDeck.AddRange(GameObject.Find("Encounters").GetComponentsInChildren<EncounterCard>());
        foreach (EncounterCard card in encounterDeck)
        {
            card.gameObject.SetActive(false);
            if(card.GetEncounterType() == "Item")
            {
                itemDeck.Add(card);
            }
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
        bool isEnemy = (currentEncounter.GetEncounterType() == "Enemy");

        bool canDrawEncounter = (!inEnemyEncounter && !inItemEncounter && !outOfTurns);
        bool canRoll = (inEnemyEncounter && encounterCharacterSelected && !hasRolled && isEnemy);
        bool needsToConfirmItem = (inItemEncounter && encounterItemSelected && encounterCharacterSelected);
        uiManager.UpdateGameButtons(canDrawEncounter, canRoll, needsToConfirmItem);
    }

    /// <summary>
    /// Update the outOfTurns bool based on the incremented tracker
    /// </summary>
    /// <param name="amount"></param>
    private void IncrementTurnTracker(int amount)
    {
        outOfTurns = uiManager.IncrementTurnTracker(amount);
    }

    /// <summary>
    /// Used for enemy encounters
    /// </summary>
    /// <param name="rollValue">Sum of dice roll and character stat</param>
    /// <param name="description">Result of the action</param>
    private void DisplayEncounterResult(int rollValue, string description)
    {
        uiManager.UpdateResultText($"You rolled a {rollValue} and {description}");
    }
    /// <summary>
    /// Used for item encounters
    /// </summary>
    /// <param name="description">Result of the action</param>
    private void DisplayEncounterResult(string description)
    {
        uiManager.UpdateResultText(description);
    }

    /// <summary>
    /// If there are still turns available, pull a random card out of the encounter deck and display it.
    /// When an item is drawn, find and play a second one.
    /// </summary>
    public void DrawEncounter()
    {
        if(outOfTurns == false)
        {
            int randIndex = Random.Range(0, encounterDeck.Count);
            currentEncounter = encounterDeck[randIndex];
            encounterDeck.RemoveAt(randIndex);

            if(currentEncounter.GetEncounterType() == "Enemy")
            {
                currentEncounter.gameObject.SetActive(true);
                currentEncounter.transform.position = encounterCardPosition.position;

                inEnemyEncounter = true;
                hasRolled = false;
                uiManager.UpdateInstructionText("Choose a Character");
            }
            else if(currentEncounter.GetEncounterType() == "Item")
            {
                // Use the drawn card as the first item
                firstItem = currentEncounter;
                firstItem.transform.position = firstItemCardPosition.position;
                firstItem.gameObject.SetActive(true);
                itemDeck.Remove(firstItem);

                // Find a second item card in the deck
                int randItemIndex = Random.Range(0, itemDeck.Count);
                secondItem = itemDeck[randItemIndex];
                secondItem.transform.position = secondItemCardPosition.position;
                secondItem.gameObject.SetActive(true);
                itemDeck.RemoveAt(randItemIndex);
                encounterDeck.Remove(secondItem);

                inItemEncounter = true;
                encounterItemSelected = false;
                uiManager.UpdateInstructionText("Choose an Item");   
            }
            
            UpdateButtons();
        }
    }

    /// <summary>
    /// Removes the currentEncounter card from the board
    /// </summary>
    private void EndEncounter()
    {
        inEnemyEncounter = false;
        encounterCharacterSelected = false;
        MoveToTeamPosition(currentCharacter);
        currentCharacter.UnselectCard();
        IncrementTurnTracker(1);

        if(currentEncounter.GetEncounterType() == "Enemy")
        {
            currentEncounter.gameObject.SetActive(false);
        }
        else if (currentEncounter.GetEncounterType()=="Item")
        {
            firstItem.gameObject.SetActive(false);
            secondItem.gameObject.SetActive(false);
        }
        
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
        if(inEnemyEncounter == true && encounterCharacterSelected == true && hasRolled == false)
        {
            hasRolled = true;
            // Dice roll + character stat
            int totalRollValue = Random.Range(1, 6) + GetEncounterStatValue();
            bool isWin = currentEncounter.IsResultWin(totalRollValue);
            string resultAction = (isWin == true ? currentEncounter.GetWinAction() : currentEncounter.GetLossAction());
            string resultDescription = HandleEnemyEncounterAction(resultAction, isWin);
            DisplayEncounterResult(totalRollValue, resultDescription);
            UpdateButtons();
            Invoke(nameof(EndEncounter), 2.0f);
        }
    }

    public bool ItemIsSelected()
    {
        return encounterItemSelected;
    }

    public void UpdateSelectedItem(EncounterCard itemCard)
    {
        if(inItemEncounter == true && encounterItemSelected == false)
        {
            currentEncounter = itemCard;
            encounterItemSelected = true;
            uiManager.UpdateInstructionText("Select a Character");
        }
        else if(inItemEncounter == true && encounterItemSelected == true)
        {
            encounterItemSelected = false;
            uiManager.UpdateInstructionText("Select an Item");
        }
    }

    public void ConfirmItem()
    {
        string resultDescription = HandleItemEncounterAction(currentEncounter.GetWinAction());
        DisplayEncounterResult(resultDescription);

        if(currentEncounter == firstItem)
        {
            firstItem.transform.position = encounterCardPosition.position;
            secondItem.gameObject.SetActive(false);
        }
        else
        {
            secondItem.transform.position = encounterCardPosition.position;
            firstItem.gameObject.SetActive(false);
        }

        inItemEncounter = false;
        encounterItemSelected = false;
        UpdateButtons();
        Invoke(nameof(EndEncounter), 2.0f);
    }

    private string HandleEnemyEncounterAction(string actionType, bool isWin)
    {
        int amount = currentEncounter.ResultAmount(isWin);
        switch (actionType)
        {
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

    private string HandleItemEncounterAction(string actionType)
    {
        int increaseAmount = currentEncounter.ResultAmount(true);
        int decreaseAmount = currentEncounter.ResultAmount(false);
        switch (actionType)
        {
            case "Key":
                currentCharacter.HasKey = true;
                return $"You gained a key.";
            case "Health":
                currentCharacter.IncreaseHealth(increaseAmount);
                currentCharacter.DecreaseStealth(decreaseAmount);
                return $"You chose armour and gained {increaseAmount} health, but lost {decreaseAmount} stealth.";
            case "Strength":
                currentCharacter.IncreaseStrength(increaseAmount);
                currentCharacter.DecreaseHealth(decreaseAmount);
                return $"You chose the weapon and gained {increaseAmount} strength, but lost {decreaseAmount} health.";
            case "Accuracy":
                currentCharacter.IncreaseAccuracy(increaseAmount);
                currentCharacter.DecreaseStrength(decreaseAmount);
                return $"You chose the seeing glass and gained {increaseAmount} accuracy, but lost {decreaseAmount} strength.";
            case "Stealth":
                currentCharacter.IncreaseStealth(increaseAmount);
                currentCharacter.DecreaseAccuracy(decreaseAmount);
                return $"You chose the camoflauge and gained {increaseAmount} stealth, but lost {decreaseAmount} accuracy.";
            default:
                return "You chose nothing.";
        }
    }

    private string HandleUnlockableEncounterAction(bool isWin)
    {
        // TODO: add stealth piece   
        if (isWin)
        {
            int amount = currentEncounter.ResultAmount(true);
            IncrementTurnTracker(amount);
            return $"are now {amount} turn(s) closer.";
        }
        else
        {
            int amount = currentEncounter.ResultAmount(false);
            IncrementTurnTracker(-amount);
            return $"now it's going to take an extra {amount} turn(s).";
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
    /// Called when a character card is clicked on
    /// </summary>
    /// <param name="characterCard"></param>
    public void AddToTeam(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
            characterCard.inTeam = true;
            characterCard.SelectCard();
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
                characterCard.UnselectCard();

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
        if (inEnemyEncounter && !encounterCharacterSelected)
        {
            currentCharacter = characterCard;
            characterCard.SelectCard();
            encounterCharacterSelected = true;
            MoveToEncounterPosition(currentCharacter);
            uiManager.UpdateInstructionText("Roll the Dice");
            UpdateButtons();
        }
        else if(inItemEncounter && encounterItemSelected && !encounterCharacterSelected)
        {
            currentCharacter = characterCard;
            characterCard.SelectCard();
            encounterCharacterSelected = true;
            MoveToEncounterPosition(currentCharacter);
            uiManager.UpdateInstructionText("Confirm Selection");
            UpdateButtons();
        }
    }
}
