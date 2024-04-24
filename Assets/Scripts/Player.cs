using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static GameManager gameManager;

    public TextMeshProUGUI handValueUI;
    public TextMeshProUGUI hiddenValueUI;

    public int lives = 4;
    public int handValue = 0;
    public List<GameObject> cardsInHand = new List<GameObject>(); 

    private void Awake() {
        Instance = this;
        gameManager = FindObjectOfType<GameManager>();
    }

    public void ClearHand()
    {
        foreach (GameObject obj in cardsInHand)
        {
            Destroy(obj);
        }
        cardsInHand.Clear();
    }

    public void AddCardToHand(GameObject card)
    {
        handValue = 0; //reset value in hand
        cardsInHand.Add(card);
        handValue = CalculateHandValue(handValue, cardsInHand);
        ShowHiddenValue();

        handValueUI.text = handValue + "/21";
        if(handValue > 21)
        {
            handValueUI.color = Color.red;
            gameManager.Stand();
        }
        else if(handValue == 21)
        {
            handValueUI.color = Color.green;
        }
        else{
            handValueUI.color = Color.white;
        }
    }

    public void DisplayHandValue()
    {
        handValueUI.text = handValue + "/21";
    }

    private void ShowHiddenValue()
    {
        if(cardsInHand[0].GetComponent<Card>().value == 1)
        {
            hiddenValueUI.text = "1/11";
        }
        else{
            hiddenValueUI.text = cardsInHand[0].GetComponent<Card>().value.ToString();
        }
    }

    int CalculateHandValue(int value, List<GameObject> cardsInHand)
    {
        int numAces = 0;

        foreach (GameObject card in cardsInHand)
        {
            if (card.GetComponent<Card>().value == 1) // Ace
            {
                numAces++;
                value += 11; // Assume Ace value as 11 initially
            }
            else
            {
                value += card.GetComponent<Card>().value;
            }
        }

        // Adjust Ace values if the total value exceeds 21
        while (value > 21 && numAces > 0)
        {
            value -= 10; // Change Ace value from 11 to 1
            numAces--;
        }
        return value;
    }

}
