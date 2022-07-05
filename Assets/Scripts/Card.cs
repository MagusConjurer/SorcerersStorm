using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Card : MonoBehaviour
{
    [SerializeField] protected CardManager cardManager;
    [SerializeField] protected bool selected;
    protected SpriteRenderer cardSprite;

    private void Start()
    {
        try
        {
            cardManager = FindObjectOfType<GameManager>().GetComponent<CardManager>();
            cardSprite = GetComponent<SpriteRenderer>();
        }
        catch
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
