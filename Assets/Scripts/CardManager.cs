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
    private bool fullTeamSelected;
    private int teamCount;

    // Encounters
    private bool reachedTheBoss;
    private bool gameIsOver;
    private bool inEnemyEncounter;
    private bool inItemEncounter;
    private bool hasConfirmedItem;
    private bool inUnlockableEncounter;
    private bool encounterItemSelected;
    private bool hasRolled;
    private List<EncounterCard> encounterDeck;
    private List<EncounterCard> encounterDiscardDeck;
    private List<EncounterCard> itemDeck;
    private EncounterCard currentEncounter;
    private Transform encounterCardPosition;
    private Transform encounterCharacterPosition;
    private EncounterCard firstItem;
    private EncounterCard secondItem;
    private Transform firstItemCardPosition;
    private Transform secondItemCardPosition;

    // Boss
    private bool inBossEncounter;
    private bool bossAlive;
    private List<BossEncounter> bossEncounterDeck;
    private List<BossEncounter> bossEncounterDiscardDeck;


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

    /// <summary>
    /// Used when loading the scene to get/refresh the game
    /// </summary>
    private void InitialGameLoad()
    {
        reachedTheBoss = false;
        gameIsOver = false;
        encounterCharacterSelected = false;
        LoadCharacterCards();
        LoadEncounters();
        LoadBossEncounters();
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

        encounterCharacterPosition = GameObject.Find("EncounterCharacterPosition").GetComponent<Transform>();

        characterDeck = new List<CharacterCard>();
        characterDeck.AddRange(GameObject.Find("CharacterCards").GetComponentsInChildren<CharacterCard>());
        for (int i = 0; i < characterDeck.Count; i++)
        {
            if(i < characterRosterPositions.Count)
            {
                characterDeck[i].gameObject.SetActive(true);
                characterDeck[i].transform.position = characterRosterPositions[i].position;
            }
            else
            {
                characterDeck[i].gameObject.SetActive(false);
            }
            
        }

        availableCharacterTeamPositions = new bool[4];
        for (int j = 0; j < availableCharacterTeamPositions.Length; j++)
        {
            availableCharacterTeamPositions[j] = true;
        }

        characterSelectedDeck = new List<CharacterCard>();
        teamCount = 0;
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
        bossAlive = false;

        encounterDeck = new List<EncounterCard>();
        encounterDiscardDeck = new List<EncounterCard>();
        encounterDeck.AddRange(GameObject.Find("Encounters").GetComponentsInChildren<EncounterCard>());
        encounterCardPosition = GameObject.Find("EncounterCardPosition").GetComponent<Transform>();

        itemDeck = new List<EncounterCard>();
        firstItemCardPosition = GameObject.Find("FirstItemCardPosition").GetComponent<Transform>();
        secondItemCardPosition = GameObject.Find("SecondItemCardPosition").GetComponent<Transform>();

        foreach (EncounterCard card in encounterDeck)
        {
            card.gameObject.SetActive(false);
            if (card.GetEncounterType() == "Item")
            {
                itemDeck.Add(card);
            }
        }
    }

    /// <summary>
    /// Set up the list of boss encounter cards
    /// </summary>
    private void LoadBossEncounters()
    {
        bossEncounterDeck = new List<BossEncounter>();
        bossEncounterDiscardDeck = new List<BossEncounter>();
        bossEncounterDeck.AddRange(GameObject.Find("BossEncounters").GetComponentsInChildren<BossEncounter>());

        foreach (BossEncounter card in bossEncounterDeck)
        {
            card.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Take all of the cards in the discard list and add them back into the main deck.
    /// </summary>
    /// <param name="atBoss">Whether to reshuffle the regular or boss encounters</param>
    private void ReshuffleEncounterCards(bool atBoss)
    {
        if(atBoss)
        {
            for(int i = bossEncounterDiscardDeck.Count - 1; i > 0; i--)
            {
                bossEncounterDeck.Add(bossEncounterDiscardDeck[i]);
                bossEncounterDiscardDeck.RemoveAt(i);
            }
        }
        else
        {
            for (int i = encounterDiscardDeck.Count - 1; i >= 0; i--)
            {
                encounterDeck.Add(encounterDiscardDeck[i]);
                encounterDiscardDeck.RemoveAt(i);
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
        bossAlive = true;
        uiManager.DisplayBossPanel();
        UpdateButtons();
    }

    /// <summary>
    /// Method used to update which buttons are active during standard encounters, the boss, and when the player dies.
    /// </summary>
    private void UpdateButtons()
    {
        if ((TeamAlive() || bossAlive) && reachedTheBoss)
        {
            UpdateBossButtons();
        }
        else
        {
            UpdateEncounterButtons(); 
        }
    }
    /// <summary>
    /// Method used to update enemy encounter specific buttons using related flags
    /// </summary>
    private void UpdateEnemyButtons()
    {
        uiManager.EnableDrawEncounterButton(!inEnemyEncounter && 
                                            !reachedTheBoss);
        uiManager.EnableRollButton(inEnemyEncounter && 
                                   encounterCharacterSelected && 
                                   !hasRolled);
    }
    /// <summary>
    /// Method used to update unlockable encounter specific buttons using related flags
    /// </summary>
    private void UpdateUnlockableButtons()
    {
        uiManager.EnableDrawEncounterButton(!inUnlockableEncounter && 
                                            !reachedTheBoss);
        uiManager.EnableRollButton(inUnlockableEncounter && 
                                   encounterCharacterSelected && 
                                   !hasRolled);
    }
    /// <summary>
    /// Method used to update item encounter specific buttons using related flags
    /// </summary>
    private void UpdateItemButtons()
    {
        uiManager.EnableDrawEncounterButton(!inItemEncounter && 
                                            !reachedTheBoss);
        uiManager.EnableItemConfirmButton(inItemEncounter && 
                                          encounterItemSelected && 
                                          encounterCharacterSelected && 
                                          !hasConfirmedItem);
    }
    /// <summary>
    /// Method used to update boss specific buttons using related flags
    /// </summary>
    private void UpdateBossButtons()
    {
        bossAlive = uiManager.GetBossHealth() > 0;
        if (reachedTheBoss)
        {
            if (bossAlive && TeamAlive())
            {
                uiManager.EnableBossEncounterButton(!inBossEncounter);
                uiManager.EnableBossRollButton(inBossEncounter && 
                                               encounterCharacterSelected && 
                                               !hasRolled);
                uiManager.EnableBossMainMenuButton(false);
            }
            else
            {
                uiManager.EnableBossEncounterButton(false);
                uiManager.EnableBossRollButton(false);

                EndGame(TeamAlive());
            }
        }
    }

    /// <summary>
    /// Calls the method specific to update buttons based on encounter type
    /// </summary>
    private void UpdateEncounterButtons()
    {
        string encounterType = currentEncounter.GetEncounterType();
        switch (encounterType)
        {
            case "Enemy":
                UpdateEnemyButtons();
                break;
            case "Item":
                UpdateItemButtons();
                break;
            case "Unlockable":
                UpdateUnlockableButtons();
                break;
        }

    }

    /// <summary>
    /// Update the outOfTurns boolean based on the incremented tracker
    /// </summary>
    /// <param name="amount"></param>
    private void IncrementTurnTracker(int amount)
    {
        reachedTheBoss = uiManager.IncrementTurnTracker(amount);
    }

    /// <summary>
    /// Used for enemy encounters
    /// </summary>
    /// <param name="rollValue">Sum of dice roll and character stat</param>
    /// <param name="description">Result of the action</param>
    private void DisplayEncounterResult(int rollValue, string description)
    {
        uiManager.UpdateInstructionText($"You rolled a {rollValue} and {description}");
    }
    /// <summary>
    /// Used for item encounters
    /// </summary>
    /// <param name="description">Result of the action</param>
    private void DisplayEncounterResult(string description)
    {
        uiManager.UpdateInstructionText(description);
    }

    /// <summary>
    /// If there are still turns available, pull a random card out of the encounter deck and display it.
    /// When an item is drawn, find and play a second one.
    /// </summary>
    public void DrawEncounter()
    {
        if(reachedTheBoss == false)
        {
            if (encounterDeck.Count == 0)
            {
                ReshuffleEncounterCards(false);
            }
            else
            {
                int randIndex = Random.Range(0, encounterDeck.Count);
                currentEncounter = encounterDeck[randIndex];
                encounterDeck.RemoveAt(randIndex);
                encounterDiscardDeck.Add(currentEncounter);
            }

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
                encounterDiscardDeck.Add(secondItem);

                inItemEncounter = true;
                encounterItemSelected = false;
                uiManager.UpdateInstructionText("Choose an Item");
            }
            else if (currentEncounter.GetEncounterType() == "Unlockable")
            {
                currentEncounter.gameObject.SetActive(true);
                currentEncounter.transform.position = encounterCardPosition.position;

                if (uiManager.GetKeyCount() > 0)
                {
                    uiManager.UpdateKeyCountText(-1);
                    uiManager.UpdateInstructionText(HandleUnlockableEncounterAction(true, true));
                    Invoke(nameof(HandleNextTurn), 1.0f);
                }
                else
                {
                    inUnlockableEncounter = true;
                    hasRolled = false;
                    uiManager.UpdateInstructionText("Choose a Character");
                }
            }
            
            UpdateButtons();
        }
    }

    /// <summary>
    /// Removes the currentEncounter card from the board and updates the text/buttons
    /// </summary>
    private void EndEncounter()
    {
        if (currentCharacter != null)
        {
            MoveToTeamPosition(currentCharacter);
            currentCharacter.UnselectCard();
            encounterCharacterSelected = false;
        }

        if (reachedTheBoss)
        {
            currentEncounter.gameObject.SetActive(false);
            inBossEncounter = false;

            if(bossAlive)
            {
                uiManager.UpdateInstructionText("Draw an Encounter");
            }
        }
        else
        {
            IncrementTurnTracker(1);

            if (currentEncounter.GetEncounterType() == "Enemy")
            {
                currentEncounter.gameObject.SetActive(false);
                inEnemyEncounter = false;
            }
            else if(currentEncounter.GetEncounterType() == "Unlockable")
            {
                currentEncounter.gameObject.SetActive(false);
                inUnlockableEncounter = false;
            }
            else if (currentEncounter.GetEncounterType() == "Item")
            {
                firstItem.gameObject.SetActive(false);
                secondItem.gameObject.SetActive(false);
                inItemEncounter = false;
                encounterItemSelected = false;
                hasConfirmedItem = false;
            }

            if (reachedTheBoss == false)
            {
                uiManager.UpdateInstructionText("Draw an Encounter");
            }
            else
            {
                Invoke(nameof(DisplayBoss), .5f);
            }
        }

        UpdateButtons();

    }
    /// <summary>
    /// Helper method to call End Encounter with a delay
    /// </summary>
    public void EndEncounterDelayed()
    {
        Invoke(nameof(EndEncounter), 1.0f);
    }

    /// <summary>
    /// Method called when the Roll button is pressed.
    /// </summary>
    public void HandleRoll()
    {
        if(encounterCharacterSelected == true && hasRolled == false)
        {
            hasRolled = true;

            if (inBossEncounter)
            {
                HandleBossEncounterAction(); 
            }
            else
            {
                // Dice roll + character stat
                int totalRollValue = Random.Range(1, 6) + GetEncounterStatValue();
                bool isWin = currentEncounter.IsResultWin(totalRollValue);
                string resultDescription = "";
                if (inEnemyEncounter)
                {
                    string resultAction = (isWin == true ? currentEncounter.GetWinAction() : currentEncounter.GetLossAction());
                    resultDescription = HandleEnemyEncounterAction(resultAction, isWin);
                }
                else if(inUnlockableEncounter)
                {
                    resultDescription = HandleUnlockableEncounterAction(isWin, false);
                }

                DisplayEncounterResult(totalRollValue, resultDescription);
            }
            
            UpdateButtons();
            Invoke(nameof(HandleNextTurn), 1.0f);
        }
    }

    /// <summary>
    /// Based on the setting, either display the next turn button or proceed to the next turn automatically
    /// </summary>
    private void HandleNextTurn()
    {
        if (gameIsOver)
        {
            EndEncounter();
            uiManager.EnableBossMainMenuButton(true);
        }
        else
        {
            if(uiManager.NextTurnButtonIsEnabled())
            {
                uiManager.DisplayNextTurnButton(true);
            }
            else
            {
                EndEncounterDelayed();
            }
        }
    }

    /// <summary>
    /// Method to retrieve whether an item is selected (used in the EncounterCard)
    /// </summary>
    public bool ItemIsSelected()
    {
        return encounterItemSelected;
    }

    /// <summary>
    /// Method called when an EncounterCard with the Item type is clicked
    /// </summary>
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

    /// <summary>
    /// Method called when the Confirm button is pressed during an Item encounter
    /// </summary>
    public void ConfirmItem()
    {
        string resultDescription = HandleItemEncounterAction(currentEncounter.GetWinAction());
        DisplayEncounterResult(resultDescription);

        hasConfirmedItem = true;

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

        UpdateButtons();
        Invoke(nameof(HandleNextTurn), 1.0f);
    }

    /// <summary>
    /// Method called during an Enemy encounter.
    /// </summary>
    /// <param name="actionType">The win/lose action from the EncounterCard</param>
    /// <param name="isWin">Was the roll + stat value in the win range</param>
    /// <returns>A description of what was lost.</returns>
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

    /// <summary>
    /// Method called during an Item encounter
    /// </summary>
    /// <param name="actionType">The action from the EncounterCard</param>
    /// <returns>A description of what was gained/lost</returns>
    private string HandleItemEncounterAction(string actionType)
    {
        int increaseAmount = currentEncounter.ResultAmount(true);
        int decreaseAmount = currentEncounter.ResultAmount(false);
        switch (actionType)
        {
            case "Key":
                uiManager.UpdateKeyCountText(1);
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
                return $"You chose the camouflage and gained {increaseAmount} stealth, but lost {decreaseAmount} accuracy.";
            default:
                return "You chose nothing.";
        }
    }

    /// <summary>
    /// Method called during an Unlockable encounter
    /// </summary>
    /// <param name="isWin">A key was available or the roll + stat value in the win range</param>
    /// <param name="withKey">A key used</param>
    /// <returns>A description of what was done and how many turns were gained/lost</returns>
    private string HandleUnlockableEncounterAction(bool isWin, bool withKey)
    {
        if (isWin && withKey)
        {
            int amount = currentEncounter.ResultAmount(true);
            IncrementTurnTracker(amount);
            return $"You use a key and are now {amount} turn(s) closer.";
        }
        else if (isWin && !withKey)
        {
            int amount = currentEncounter.ResultAmount(true);
            IncrementTurnTracker(amount);
            currentCharacter.DecreaseStealth(1);
            return $", pick the lock and are now {amount} turn(s) closer. You lose 1 stealth.";
        }
        else
        {
            int amount = currentEncounter.ResultAmount(false);
            IncrementTurnTracker(-amount);
            return $"and waste {amount} turn(s) trying to pick the lock.";
        }
    }

    /// <summary>
    /// Method to get the character's stat value for the current encounter type
    /// </summary>
    private int GetEncounterStatValue()
    {
        string statType = currentEncounter.GetWinAction();
        switch (currentEncounter.GetEncounterType())
        {
            case "Enemy":
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
            case "Unlockable":
                return currentCharacter.GetStealth();
            case "Boss":
                if (statType == "Strength")
                {
                    return currentCharacter.GetStrength() + currentCharacter.GetAccuracy();
                }
                else if (statType == "Accuracy")
                {
                    return currentCharacter.GetAccuracy() + currentCharacter.GetStealth();
                }
                else
                {
                    return currentCharacter.GetStealth() + currentCharacter.GetStrength();
                }
        }

        return 0;
    }

    /// <summary>
    /// Called when a character card is clicked on during the team selection phase
    /// </summary>
    /// <param name="characterCard"></param>
    public void AddToTeam(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
            characterCard.inTeam = true;
            characterCard.SelectCard();
            IncreaseTeamCount();
            if (teamCount == 4)
            {
                fullTeamSelected = true;
                uiManager.CanConfirmTeam(true);
            }
        }
    }

    /// <summary>
    /// Called when a character already on the team is clicked on during the team selection phase
    /// </summary>
    /// <param name="characterCard"></param>
    public void RemoveFromTeam(CharacterCard characterCard)
    {
        if(characterSelectedDeck.Contains(characterCard) == true)
        {
            characterSelectedDeck.Remove(characterCard);
            characterCard.inTeam = false;
            characterCard.UnselectCard();
            DecreaseTeamCount();
            if (teamCount < 4)
            {
                fullTeamSelected = false;
                uiManager.CanConfirmTeam(false);
            }
        }
    }

    /// <summary>
    /// Method called by the confirm button during the team selection phase
    /// </summary>
    public void ConfirmTeam()
    {
        if (teamCount == 4)
        {
            foreach(CharacterCard card in characterSelectedDeck)
            {
                card.confirmedInTeam = true;
            }
            uiManager.CanConfirmTeam(false);
            Invoke(nameof(MoveAllToTeamPositions), .5f);
            Invoke(nameof(DisplayBoard), .75f);
        }
    }

    /// <summary>
    /// Helper method for checking whether at least one team member is alive
    /// </summary>
    /// <returns>False if all team members are dead</returns>
    public bool TeamAlive()
    {
        return teamCount > 0;
    }

    /// <summary>
    /// Used to decrease the count when a character dies. Also called by the CharacterCard class .
    /// </summary>
    public void DecreaseTeamCount()
    {
        teamCount--;
    }

    /// <summary>
    /// Used to increase the count when a player joins the team. Currently only used in AddToTeam.
    /// Could be used if players could find new team members along the way.
    /// </summary>
    public void IncreaseTeamCount()
    {
        teamCount++;
    }

    /// <summary>
    /// Used after all selected characters are removed from the deck to remove the rest from the board.
    /// </summary>
    private void HideRemainingCharacters()
    {
        foreach(CharacterCard card in characterDeck)
        {
            card.gameObject.SetActive(false);
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

        HideRemainingCharacters();
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

    /// <summary>
    /// Used during an encounter after the character has been selected
    /// </summary>
    /// <param name="character">The selected character card</param>
    private void MoveToEncounterPosition(CharacterCard character)
    {
        character.transform.position = encounterCharacterPosition.position;
    }

    /// <summary>
    /// Used OnMousedown for a CharacterCard during an encounter
    /// </summary>
    /// <param name="characterCard">The card that was clicked</param>
    public void SetCurrentCharacter(CharacterCard characterCard)
    {
        if ((inEnemyEncounter || inUnlockableEncounter || inBossEncounter) && !encounterCharacterSelected)
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

    /// <summary>
    /// Used in the OnMousedown function within the CharacterCard to unselect them
    /// </summary>
    /// <param name="characterCard"></param>
    public void UnsetCurrentCharacter(CharacterCard characterCard)
    {
        if ((inEnemyEncounter || inUnlockableEncounter || inBossEncounter) && encounterCharacterSelected && !hasRolled)
        {
            currentCharacter = characterCard;
            characterCard.UnselectCard();
            encounterCharacterSelected = false;
            MoveToTeamPosition(currentCharacter);
            uiManager.UpdateInstructionText("Choose a Character");
            UpdateButtons();
        }
        else if (inItemEncounter && encounterItemSelected && encounterCharacterSelected && !hasConfirmedItem)
        {
            currentCharacter = characterCard;
            characterCard.UnselectCard();
            encounterCharacterSelected = false;
            MoveToTeamPosition(currentCharacter);
            uiManager.UpdateInstructionText("Choose a Character");
            UpdateButtons();
        }
    }

    /// <summary>
    /// If the boss still has health, pull a random card out of the boss encounter deck and display it.
    /// </summary>
    public void DrawBossEncounter()
    {
        if (reachedTheBoss && bossAlive)
        {
            if (bossEncounterDeck.Count == 0)
            {
                ReshuffleEncounterCards(true);
            }

            int randIndex = Random.Range(0, bossEncounterDeck.Count);
            currentEncounter = bossEncounterDeck[randIndex];
            bossEncounterDeck.RemoveAt(randIndex);
            bossEncounterDiscardDeck.Add((BossEncounter)currentEncounter);

            currentEncounter.gameObject.SetActive(true);
            currentEncounter.transform.position = encounterCardPosition.position;

            inBossEncounter = true;
            hasRolled = false;
            uiManager.UpdateInstructionText("Choose a Character");
        }

        UpdateButtons();
    }

    private void HandleBossEncounterAction()
    {
        BossEncounter bossEncounter = (BossEncounter)currentEncounter;
        // Dice roll + character stat
        int totalRollValue = Random.Range(1, 6) + GetEncounterStatValue();
        bool isWin = bossEncounter.IsResultWin(totalRollValue);
        bool isBigWin = bossEncounter.IsResultBigWin(totalRollValue);
        string resultDescription = "";
        
        int amount = bossEncounter.GetResultAmount(isWin, isBigWin);
        string actionType;
        if (isWin)
        {
            actionType = bossEncounter.GetWinAction();
            uiManager.DecreaseBossHealth(1);
            resultDescription += "did 1 damage to the boss,";
        }
        else if (isBigWin)
        {
            actionType = bossEncounter.GetBigWinAction();
            uiManager.DecreaseBossHealth(2);
            resultDescription += "did 2 damage to the boss,";
        }
        else
        {
            actionType = bossEncounter.GetLossAction();
        }
        
        switch (actionType)
        {
            case "Health":
                currentCharacter.DecreaseHealth(amount);
                resultDescription += $"lost {amount} health.";
                break;
            case "Strength":
                currentCharacter.DecreaseStrength(amount);
                currentCharacter.DecreaseAccuracy(amount);
                resultDescription += $" but lost {amount} strength & accuracy.";
                break;
            case "Accuracy":
                currentCharacter.DecreaseAccuracy(amount);
                currentCharacter.DecreaseStealth(amount);
                resultDescription += $" but lost {amount} accuracy & stealth.";
                break;
            case "Stealth":
                currentCharacter.DecreaseStealth(amount);
                currentCharacter.DecreaseStrength(amount);
                resultDescription += $" but lost {amount} stealth & strength.";
                break;
            default:
                break;
        }

        DisplayEncounterResult(totalRollValue, resultDescription);
    }

    private void EndGame(bool playerWon)
    {
        if(playerWon)
        {
            DisplayEncounterResult("You have defeated the boss!");
        }
        else
        {
            DisplayEncounterResult("You have been defeated!");
        }

        gameIsOver = true;
    }
}
