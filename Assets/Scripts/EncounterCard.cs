using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCard : Card
{
    [SerializeField] private Type encounterType;
    [SerializeField] private List<int> winCriteria;    

    [SerializeField] private Action winResult;
    [SerializeField, Range(1, 2)] private int winResultAmount;
    [SerializeField] private Action lossResult;
    [SerializeField, Range(1, 2)] private int lossResultAmount;

    private bool canClickItem;

    public string GetEncounterType()
    {
        return encounterType.ToString();
    }

    public bool IsResultWin(int rollValue)
    {
        return winCriteria.Contains(rollValue);
    }

    public string GetWinAction()
    {
        return winResult.ToString();
    }
    public string GetLossAction()
    {
        return lossResult.ToString();
    }

    public int ResultAmount(bool isWin)
    {
        if (isWin)
        {
            return winResultAmount;
        }
        else
        {
            return lossResultAmount;
        }
    }

    public void SetItemClickable(bool status)
    {
        if(encounterType.ToString() ==  "Item")
        {
            canClickItem = status;
        }
    }

    private void OnMouseDown()
    {
        if(encounterType.ToString() == "Item")
        {
            if (selected == false && cardManager.ItemIsSelected() == false && canClickItem)
            {
                SelectCard();
                cardManager.UpdateSelectedItem(this);
            }
            else if (selected == true && cardManager.ItemIsSelected() == true && canClickItem)
            {
                UnselectCard();
                cardManager.UpdateSelectedItem(this);
            }
        }
    }

    protected enum Action
    {
        Turn,
        Key,
        Health,
        Strength,
        Accuracy,
        Stealth
    }

    protected enum Type 
    {
        Item,
        Enemy, 
        Trap,
        Unlockable,
        Boss
    }
}
