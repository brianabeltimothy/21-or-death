using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dealer : MonoBehaviour
{
    public static Dealer Instance;
    
    [SerializeField] private TextMeshProUGUI handValueUI;

    public int lives = 4;
    public int handValue = 0;
    public List<GameObject> cardsInHand = new List<GameObject>(); 

    private void Awake() {
        Instance = this;
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
        DisplayOpenCards();
    }

    public void DisplayHandValue()
    {
        handValueUI.text = handValue + "/21";
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

    void DisplayOpenCards()
    {
        int openCardsValue = 0;
        int numAces = 0;
        int i = 1; // Start from index 1 to skip the first card (unknown)
        while (i < cardsInHand.Count)
        {
            if (cardsInHand[i].GetComponent<Card>().value == 1) // Ace
            {
                numAces++;
                openCardsValue += 11; // Assume Ace value as 11 initially
            }
            else
            {
                openCardsValue += cardsInHand[i].GetComponent<Card>().value;
            }
            i++;
        }

        while (openCardsValue > 21 && numAces > 0)
        {
            openCardsValue -= 10; // Change Ace value from 11 to 1
            numAces--;
        }
        
        handValueUI.text = "? + " + openCardsValue + "/21";
    }

}
