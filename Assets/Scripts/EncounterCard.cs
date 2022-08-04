using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCard : Card
{
    [SerializeField] private Type EncounterType;
    [SerializeField] private List<int> winCriteria;    

    [SerializeField] private Action WinResult;
    [SerializeField, Range(1, 2)] private int WinResultAmount;
    [SerializeField] private Action LossResult;
    [SerializeField, Range(1, 2)] private int LossResultAmount;

    public string GetEncounterType()
    {
        return EncounterType.ToString();
    }

    public bool IsResultWin(int rollValue)
    {
        return winCriteria.Contains(rollValue);
    }

    public string GetWinAction()
    {
        return WinResult.ToString();
    }
    public string GetLossAction()
    {
        return LossResult.ToString();
    }

    public int ResultAmount(bool isWin)
    {
        if (isWin)
        {
            return WinResultAmount;
        }
        else
        {
            return LossResultAmount;
        }
    }

    private void OnMouseDown()
    {
        if(EncounterType.ToString() == "Item")
        {
            if (selected == false && cardManager.ItemIsSelected() == false)
            {
                SelectCard();
                cardManager.UpdateSelectedItem(this);
            }
            else if (selected == true && cardManager.ItemIsSelected() == true)
            {
                UnselectCard();
                cardManager.UpdateSelectedItem(this);
            }
        }
    }

    private enum Action
    {
        Turn,
        Key,
        Health,
        Strength,
        Accuracy,
        Stealth
    }

    private enum Type 
    {
        Item,
        Enemy, 
        Trap,
        Unlockable
    }
}
