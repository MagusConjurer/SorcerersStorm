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
    private bool cardsLoaded = false;
    public bool[] availableCharacterTeamPositions;
    public bool fullTeamSelected;

    // Game Panels
    private GameObject rosterPanel;
    private GameObject boardPanel;
    private GameObject teamPanel;

    // Encounters
    private List<EncounterCard> encounterDeck = new List<EncounterCard>();

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Called when a card is clicked on
    /// </summary>
    /// <param name="characterCard"></param>
    public void SelectCharacter(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
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
                characterCard.ResetColor();

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
                button.onClick.AddListener(DrawEncounter);
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

    private void DrawEncounter()
    {
        EncounterCard currentEncounter = encounterDeck[Random.Range(0, encounterDeck.Count)];
        currentEncounter.gameObject.SetActive(true);
        currentEncounter.transform.position = Vector3.zero;
    }
}
