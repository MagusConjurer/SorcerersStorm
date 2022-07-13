using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    // Character Cards
    private List<Transform> characterRosterPositions = new List<Transform>();
    private List<Transform> characterTeamPositions = new List<Transform>();
    private List<CharacterCard> characterDeck = new List<CharacterCard>();
    private List<CharacterCard> characterSelectedDeck = new List<CharacterCard>();
    private CharacterCard currentCharacter;
    private bool encounterCharacterSelected = false;
    private bool cardsLoaded = false;
    public bool[] availableCharacterTeamPositions;
    public bool fullTeamSelected;
    

    // GUI
    private GameObject rosterPanel;
    private GameObject boardPanel;
    private GameObject teamPanel;
    private GameObject bossPanel;
    private Slider turnTracker;
    private bool outOfTurns = false;
    private Button encounterButton;
    private Button rollButton;

    // Encounters
    private bool inEncounter;
    private List<EncounterCard> encounterDeck = new List<EncounterCard>();
    private EncounterCard currentEncounter;
    private Transform encounterCardPosition;
    private Transform encounterCharacterPosition;

    // Start is called before the first frame update
    void Start()
    {

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

    /// <summary>
    /// Called when the scene loads. 
    /// </summary>
    private void LoadCharacterCards()
    {
        /// Get the lists of roster and team positions, then remove their parent transform from the list
        characterRosterPositions.AddRange(GameObject.Find("RosterPositions").GetComponentsInChildren<Transform>());
        characterRosterPositions.RemoveAt(0);
        characterTeamPositions.AddRange(GameObject.Find("TeamPositions").GetComponentsInChildren<Transform>());
        characterTeamPositions.RemoveAt(0);
        characterDeck.AddRange(GameObject.Find("CharacterCards").GetComponentsInChildren<CharacterCard>());
        fullTeamSelected = false;

        for (int i = 0; i < characterDeck.Count; i++)
        {
            characterDeck[i].gameObject.SetActive(true);
            characterDeck[i].transform.position = characterRosterPositions[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(cardsLoaded == false && SceneManager.GetActiveScene().name == "GameScene")
        {
            cardsLoaded = true;
            LoadCharacterCards();
            LoadEncounters();
            LoadGamePanels();
        }

        if(outOfTurns == true)
        {
            Invoke(nameof(LoadBossEncounter), .5f);
        }

    }

    /// <summary>
    /// Called after the GameScene is loaded.
    /// </summary>
    private void LoadGamePanels()
    {
        rosterPanel = GameObject.Find("RosterPanel");
        boardPanel = GameObject.Find("BoardPanel");
        teamPanel = GameObject.Find("TeamPanel");
        bossPanel = GameObject.Find("BossPanel");

        turnTracker = boardPanel.GetComponentInChildren<Slider>();

        Button[] boardButtons = boardPanel.GetComponentsInChildren<Button>();
        foreach(Button button in boardButtons)
        {
            if(button.name == "EncounterButton")
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

    private void LoadEncounters()
    {
        encounterCardPosition = GameObject.Find("EncounterCardPosition").GetComponent<Transform>();
        encounterCharacterPosition = GameObject.Find("EncounterCharacterPosition").GetComponent<Transform>();

        encounterDeck.AddRange(GameObject.Find("Encounters").GetComponentsInChildren<EncounterCard>());
        foreach(EncounterCard card in encounterDeck)
        {
            card.gameObject.SetActive(false);
        }
    }

    private void UpdateButtons()
    {
        encounterButton.enabled = !inEncounter;
        rollButton.enabled = (inEncounter && encounterCharacterSelected);
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
        UpdateButtons();
    }

    private void HandleRoll()
    {
        if(inEncounter == true && encounterCharacterSelected == true)
        {
            // Dice roll + character stat
            int totalValue = Random.Range(1, 6) + GetEncounterStat();
            bool isWin = currentEncounter.IsResultWin(totalValue);
            string resultAction = currentEncounter.ResultAction(isWin);
            HandleEncounterAction(resultAction, isWin);
            Invoke(nameof(EndEncounter), .25f);
        }
    }

    private void HandleEncounterAction(string actionType, bool isWin)
    {
        switch(actionType)
        {
            case "Item":
                break;
            case "Health":
                if(isWin == true)
                {
                    currentCharacter.DecreaseHealth(currentEncounter.ResultAmount(true));
                }
                else
                {
                    currentCharacter.DecreaseHealth(currentEncounter.ResultAmount(false));
                }
                break;
            case "Strength":
                break;
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

    public void SetCurrentCharacter(CharacterCard characterCard)
    {
        if (inEncounter == true && encounterCharacterSelected == false)
        {
            currentCharacter = characterCard;
            characterCard.SelectCharacter();
            encounterCharacterSelected = true;
            MoveToEncounterPosition(currentCharacter);
            UpdateButtons();
        }
    }
}
