using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Team
{
    private bool fullTeamSelected;
    private int teamCount;
    private bool[] characterTeamPositionStatus;

    private List<CharacterCard> characterDeck;
    private List<CharacterCard> characterSelectedDeck;
    private List<Transform> characterRosterTransform;
    private List<Transform> characterTeamTransforms;
    
    private Transform encounterCharacterTransform;

    public Team()
    {
        fullTeamSelected = false;
        teamCount = 0;
        InitializeTeamPositions();
    }

    /// <summary>
    /// Method for checking whether at least one team member is alive
    /// </summary>
    /// <returns>False if all team members are dead</returns>
    public bool IsAlive()
    {
        return teamCount > 0;
    }

    public int TeamCount()
    {
        return teamCount;
    }

    /// <summary>
    /// Called when a character card is clicked on during the team selection phase
    /// </summary>
    /// <param name="characterCard"></param>
    public bool AddToTeam(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == false && fullTeamSelected == false)
        {
            characterSelectedDeck.Add(characterCard);
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
    /// </summary>
    /// <param name="characterCard"></param>
    public bool RemoveFromTeam(CharacterCard characterCard)
    {
        if (characterSelectedDeck.Contains(characterCard) == true)
        {
            characterSelectedDeck.Remove(characterCard);
            characterCard.inTeam = false;
            characterCard.UnselectCard();
            teamCount--;
            if (teamCount < 4)
            {
                fullTeamSelected = false;
                // uiManager.CanConfirmTeam(false);
                return false;
                uiManager.UpdateRosterText("Select Your Four Characters");
            }
        }
        return true;
    }

    /// <summary>
    /// Method called by the confirm button during the team selection phase
    /// </summary>
    public void ConfirmTeam()
    {
        if (teamCount == 4)
        {
            foreach (CharacterCard card in characterSelectedDeck)
            {
                card.confirmedInTeam = true;
            }
            uiManager.CanConfirmTeam(false);
            uiManager.UpdateRosterText("");
            Invoke(nameof(MoveAllToTeamPositions), .5f);
            Invoke(nameof(DisplayBoard), .75f);
        }
    }

    /// <summary>
    /// Used after all selected characters are removed from the deck to remove the rest from the board.
    /// </summary>
    private void HideRemainingCharacters()
    {
        foreach (CharacterCard card in characterDeck)
        {
            card.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Moves the selected characters to their corresponding team positions.
    /// </summary>
    public void MoveAllToTeamPositions()
    {
        for (int i = 0; i < characterTeamPositionStatus.Length; i++)
        {
            CharacterCard characterCard = characterSelectedDeck[i];
            if (characterTeamPositionStatus[i] == true)
            {
                characterCard.placedIndex = i;
                MoveToTeamPosition(characterCard);
                characterCard.UnselectCard();

                characterTeamPositionStatus[i] = false;
                characterDeck.Remove(characterCard);
            }
        }

        HideRemainingCharacters();
    }

    public Vector3 GetTeamPosition(int positionIndex)
    {
        return characterTeamTransforms[positionIndex].position;
    }

    private void InitializeTeamPositions()
    {
        characterTeamPositionStatus = new bool[4];
        for (int i = 0; i < characterTeamPositionStatus.Length; i++)
        {
            characterTeamPositionStatus[i] = true;
        }
    }
}
