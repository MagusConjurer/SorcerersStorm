using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Card : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    [SerializeField] private bool selected;
    public int placedIndex;

    private void Start()
    {
        try
        {
            cardManager = FindObjectOfType<GameManager>().GetComponent<CardManager>();
        }
        catch
        {
            SceneManager.LoadScene("MainMenu");
        }
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
