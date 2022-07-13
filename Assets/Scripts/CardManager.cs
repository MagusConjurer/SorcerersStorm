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
    private Button encounterButton;
    private Button rollButton;

    // Encounters
    private bool inEncounter;
    private List<EncounterCard> encounterDeck = new List<EncounterCard>();
    private EncounterCard currentEncounter;

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
                Invoke("PlaceCharacters", .5f);
                Invoke("DisplayBoard", .75f);
            }
        }
    }

    /// <summary>
    /// Moves the selected characters to their corresponding team positions.
    /// </summary>
    private void PlaceCharacters()
    {
        for (int i = 0; i < availableCharacterTeamPositions.Length; i++)
        {
            CharacterCard characterCard = characterSelectedDeck[i];
            if (availableCharacterTeamPositions[i] == true)
            {
                characterCard.placedIndex = i;
                characterCard.transform.position = characterTeamPositions[i].position;
                characterCard.UnselectCharacter();

                availableCharacterTeamPositions[i] = false;
                characterDeck.Remove(characterCard);
            }
        }
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
            LoadEncounterCards();
            LoadGamePanels();
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

    private void DisplayRoster()
    {
        rosterPanel.SetActive(true);
        boardPanel.SetActive(false);
    }

    private void DisplayBoard()
    {
        rosterPanel.SetActive(false);
        boardPanel.SetActive(true);
    }

    private void LoadEncounterCards()
    {
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

    private void DrawEncounter()
    {
        currentEncounter = encounterDeck[Random.Range(0, encounterDeck.Count)];
        currentEncounter.gameObject.SetActive(true);
        // TODO: Choose encounter position
        currentEncounter.transform.position = Vector3.zero;
        inEncounter = true;
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
            HandleAction(resultAction, isWin);
            inEncounter = false;
            encounterCharacterSelected = false;
            currentCharacter.UnselectCharacter();
            UpdateButtons();
        }
    }

    private void HandleAction(string actionType, bool isWin)
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
            UpdateButtons();
        }
    }
}
