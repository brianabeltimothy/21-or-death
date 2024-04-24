using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static Deck Instance;

    public List<GameObject> cardDeck;
    [SerializeField] private List<GameObject> originalDeck;

    private void Awake() {
        Instance = this;
    }

    void Start()
    {
        cardDeck = new List<GameObject>(originalDeck);
    }

    private void Update() {
        
    }

    public void ShuffleDeck()
    {
        int n = cardDeck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            GameObject temp = cardDeck[k];
            cardDeck[k] = cardDeck[n];
            cardDeck[n] = temp;
        }
        
        // Debug.Log("card is shuffeled");
    }

    public void ResetDeck()
    {
        cardDeck.Clear();
        cardDeck.AddRange(originalDeck);
        
        // Debug.Log("card is reset");
    }

    GameObject DealCard()
    {
        GameObject topCard = cardDeck[0];
        cardDeck.RemoveAt(0);
        // Debug.Log("card is dealed");
        return topCard;
    }

    public IEnumerator DrawCard(Transform targetPos)
    {
        GameObject drawnCard = DealCard();
        if (drawnCard != null)
        {
            GameObject instantiatedCard = Instantiate(drawnCard, this.transform.position, targetPos.rotation);
            Card cardScript = instantiatedCard.GetComponent<Card>();
            if (cardScript != null)
            {
                yield return StartCoroutine(cardScript.MoveToTarget(targetPos));
                yield return cardScript;
            }
            else
            {
                Debug.LogError("Card script not found on drawn card!");
            }
        }
        else
        {
            Debug.LogWarning("No card drawn from the deck!");
        }
        // Debug.Log("Card is drawn");
    }
}
