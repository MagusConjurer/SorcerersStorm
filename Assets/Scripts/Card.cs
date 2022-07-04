using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    [SerializeField] private bool selected;
    public int placedIndex;

    private void Start()
    {
        cardManager = FindObjectOfType<GameManager>().GetComponent<CardManager>();
    }

    private void OnMouseDown()
    {
        if (selected == false)
        {
            cardManager.SelectCharacter(this);
            selected = true;
        }
    }
}
