using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardManager : MonoBehaviour
{
    private List<Transform> characterRosterPositions = new List<Transform>();
    private List<Transform> characterTeamPositions = new List<Transform>();
    private List<Card> characterDeck = new List<Card>();
    private List<Card> characterSelectedDeck = new List<Card>();
    private bool cardsLoaded = false;
    private bool fullTeamSelected;

    public bool[] availableCharacterTeamPositions;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// Called when a card is clicked on
    /// </summary>
    /// <param name="characterCard"></param>
    public void SelectCharacter(Card characterCard)
    {
        Debug.Log(characterCard.name);
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
            if (characterSelectedDeck.Count == 4)
            {
                fullTeamSelected = true;
                Invoke("PlaceCharacters", .5f);
            }
        }
    }

    /// <summary>
    /// Moves the selected characters to their corresponding team positions.
    /// </summary>
    private void PlaceCharacters()
    {
        Debug.Log("Placing Characters in " + availableCharacterTeamPositions.Length + " positions");
        for (int i = 0; i < availableCharacterTeamPositions.Length; i++)
        {
            Card characterCard = characterSelectedDeck[i];
            if (availableCharacterTeamPositions[i] == true)
            {
                characterCard.placedIndex = i;
                characterCard.transform.position = characterTeamPositions[i].position;

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
        characterDeck.AddRange(GameObject.Find("CharacterCards").GetComponentsInChildren<Card>());
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
        }
    }
}
