using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card
{
    public int placedIndex;

    private void OnMouseDown()
    {
        if (selected == false)
        {
            cardManager.SelectCharacter(this);
            selected = true;
            cardSprite.color = cardSprite.color / 2;
        }
    }

    public void ResetColor()
    {
        cardSprite.color = cardSprite.color * 2;
    }
}
