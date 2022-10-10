using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Team
{
    private bool fullTeamSelected;
    private int teamCount;
    private bool[] teamPositionIndexStatus;

    private List<CharacterCard> initialDeck;
    private List<CharacterCard> teamDeck;
    private List<Transform> characterTeamTransforms;

    /// <summary>
    /// Holds all data related to the player's team of characters
    /// </summary>
    /// <param name="characterDeck">The initial deck of characters</param>
    public Team(List<CharacterCard> characterDeck)
    {
        fullTeamSelected = false;
        teamCount = 0;
        initialDeck = characterDeck;
        teamDeck = new List<CharacterCard>();
        InitializeTeamPositions();
        InitializeTeamTransforms();
    }

    /// <summary>
    /// Method for checking whether at least one team member is alive
    /// </summary>
    /// <returns>False if all team members are dead</returns>
    public bool IsAlive()
    {
        return teamCount > 0;
    }

    public int GetTeamCount()
    {
        return teamCount;
    }

    /// <summary>
    /// Called when a character card is clicked on during the team selection phase
    /// </summary>
    /// <param name="characterCard"></param>
    /// <returns>True if the confirm button should be shown</returns>
    public bool AddToTeam(CharacterCard characterCard)
    {
        if (teamDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            teamDeck.Add(characterCard);
            characterCard.inTeam = true;
            characterCard.SelectCard();
            teamCount++;
            if (teamCount == 4)
            {
                fullTeamSelected = true;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Called when a character already on the team is clicked on during the team selection phase
    /// or when a character dies.
    /// </summary>
    /// <param name="characterCard"></param>
    /// <returns>True if the confirm button should be shown</returns>
    public bool RemoveFromTeam(CharacterCard characterCard)
    {
        if (teamDeck.Contains(characterCard))
        {
            teamCount--;

            if (characterCard.confirmedInTeam == false)
            {
                teamDeck.Remove(characterCard);
                characterCard.inTeam = false;
                characterCard.UnselectCard();
                if (teamCount < 4)
                {
                    fullTeamSelected = false;
                }
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// Method called by the cardManager during the team selection phase
    /// </summary>
    public void ConfirmTeam()
    {
        if (teamCount == 4)
        {
            foreach (CharacterCard card in teamDeck)
            {
                card.confirmedInTeam = true;
            }
        }
    }

    /// <summary>
    /// Moves a character card to it's placed index position. 
    /// 
    /// Only use after MoveAllToTeamPositions has been called.
    /// </summary>
    public void MoveToTeamPosition(CharacterCard character)
    {
        character.transform.position = characterTeamTransforms[character.placedIndex].position;
    }

    /// <summary>
    /// Moves the selected characters to their corresponding team positions.
    /// </summary>
    public void MoveAllToTeamPositions()
    {
        for (int i = 0; i < teamPositionIndexStatus.Length; i++)
        {
            CharacterCard characterCard = teamDeck[i];
            if (teamPositionIndexStatus[i] == true)
            {
                characterCard.placedIndex = i;
                MoveToTeamPosition(characterCard);
                characterCard.UnselectCard();

                teamPositionIndexStatus[i] = false;
                initialDeck.Remove(characterCard);
            }
        }
    }

    private void InitializeTeamPositions()
    {
        teamPositionIndexStatus = new bool[4];
        for (int i = 0; i < teamPositionIndexStatus.Length; i++)
        {
            teamPositionIndexStatus[i] = true;
        }
    }

    private void InitializeTeamTransforms()
    {
        characterTeamTransforms = new List<Transform>();
        characterTeamTransforms.AddRange(GameObject.Find("TeamPositions").GetComponentsInChildren<Transform>());
        characterTeamTransforms.RemoveAt(0);
    }
}
