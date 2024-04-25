using System.Collections;
using UnityEngine;
using EZCameraShake;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //scripts
    private Deck deck;
    private Player player;
    private Dealer dealer;
    private UIManager uiManager;

    [SerializeField] private GameObject playerFirstCard;
    [SerializeField] private GameObject playerSecondCard;
    [SerializeField] private GameObject dealerFirstCard;
    [SerializeField] private GameObject dealerSecondCard;

    private Transform playerNextCardPos;
    private Transform dealerNextCardPos;

    //sounds
    private AudioSource audioSource;
    [Header("Sounds")]
    [SerializeField] private AudioClip drawCardFlick; 
    [SerializeField] private AudioClip playerElectrocutedSound;
    [SerializeField] private AudioClip dealerElectrocutedSound;

    //dialogues
    [Header("Dialogues")]
    [SerializeField] private DialogueTrigger rulesDialogue;
    [SerializeField] private DialogueTrigger shuffleDialogue;
    [SerializeField] private DialogueTrigger youWinDialogue;
    [SerializeField] private DialogueTrigger youLoseDialogue;
    [SerializeField] private DialogueTrigger tieDialogue;
    [SerializeField] private DialogueTrigger dealerStandDialogue;


    public enum HandResult
    {
        PlayerWin,
        DealerWin,
        Tie
    }

    private void Awake()
    {
        deck = FindObjectOfType<Deck>();
        player = FindObjectOfType<Player>();
        dealer = FindObjectOfType<Dealer>();
        uiManager = FindObjectOfType<UIManager>();
        audioSource = GetComponent<AudioSource>();

        Instance = this;
    }

    private void Start() {
        uiManager.playerLivesText.text = player.lives.ToString();
        uiManager.dealerLivesText.text = dealer.lives.ToString();

        uiManager.DisableAll();

        rulesDialogue.StartDialogue();
        StartCoroutine(StartFirstRound());
    }


    public IEnumerator StartFirstRound()
    {
        yield return new WaitForSeconds(20f); //dont forget
        // yield return null; //temp
        yield return StartCoroutine(player.LookAtTable());
        deck.ShuffleDeck();
        StartCoroutine(DrawCardsForPlayers());
    }

    public IEnumerator StartRound()
    {
        uiManager.DisableAll();
        yield return StartCoroutine(player.LookAtTable());
        if(deck.cardDeck.Count < 27)
        {
            deck.ResetDeck();
            deck.ShuffleDeck();
            shuffleDialogue.StartDialogue();
            yield return new WaitForSeconds(2f);
        }
        StartCoroutine(DrawCardsForPlayers());
    }


    private IEnumerator DrawCardsForPlayers()
    {
        yield return DrawCardForPlayer(playerFirstCard.transform, player);
        yield return DrawCardForDealer(dealerFirstCard.transform, dealer);
        yield return new WaitForSeconds(2f);
        yield return DrawCardForPlayer(playerSecondCard.transform, player);
        yield return DrawCardForDealer(dealerSecondCard.transform, dealer);
        uiManager.EnableAll();
    }

    private IEnumerator DrawCardForPlayer(Transform cardTransform, Player player)
    {   
        audioSource.PlayOneShot(drawCardFlick);
        IEnumerator drawCoroutine = deck.DrawCard(cardTransform);
        yield return StartCoroutine(drawCoroutine);
        Card card = (Card)drawCoroutine.Current;
        player.AddCardToHand(card.gameObject);
        playerNextCardPos = card.nextCardPos;
    }

    private IEnumerator DrawCardForDealer(Transform cardTransform, Dealer dealer)
    {
        yield return new WaitForSeconds(2f);
        audioSource.PlayOneShot(drawCardFlick);
        IEnumerator drawCoroutine = deck.DrawCard(cardTransform);
        yield return StartCoroutine(drawCoroutine);
        Card card = (Card)drawCoroutine.Current;
        dealer.AddCardToHand(card.gameObject);
        dealerNextCardPos = card.nextCardPos;
    }

    public void Hit()
    {
        StartCoroutine(DrawCardForPlayer(playerNextCardPos, player));
    }

    public void Stand()
    {
        StartCoroutine(DealerTurn());
        uiManager.DisablePlayerMove();
    }

    private IEnumerator DealerTurn()
    {
        // Dealer keeps hitting until hand value is 17 or higher
        while (dealer.handValue < 17)
        {
            yield return DrawCardForDealer(dealerNextCardPos, dealer);
        }
        yield return StartCoroutine(player.LookAtDealer());
        dealerStandDialogue.StartDialogue();
        yield return new WaitForSeconds(2f); 
        yield return StartCoroutine(player.LookAtTable());
        
        //end rounds
        EndRound();
    }

    private void EndRound()
    {
        uiManager.DisablePlayerMove();
        player.hiddenValueUI.text = "";
        player.cardsInHand[0].GetComponent<Card>().FlipCard();
        dealer.cardsInHand[0].GetComponent<Card>().FlipCard();
        StartCoroutine(EndRoundCoroutine());
    }

    private IEnumerator EndRoundCoroutine()
    {
        yield return new WaitForSeconds(2f); // time for the cards to flip
        player.DisplayHandValue();
        dealer.DisplayHandValue();

        // check winners
        HandResult result = CompareHands(player.handValue, dealer.handValue);
        if(result == HandResult.PlayerWin)
        {
            dealer.lives--;
            youWinDialogue.StartDialogue();
            yield return new WaitForSeconds(2f); //time to display the winner dialogue
            yield return StartCoroutine(ElectrocuteDealer());
        }
        else if(result == HandResult.DealerWin)
        {
            player.lives--;
            youLoseDialogue.StartDialogue();
            yield return new WaitForSeconds(2f); //time to display the winner dialogue
            yield return StartCoroutine(ElectrocutePlayer(22));
        }
        else
        {
            tieDialogue.StartDialogue();
            yield return new WaitForSeconds(2f);
        }

        uiManager.playerLivesText.text = player.lives.ToString();
        uiManager.dealerLivesText.text = dealer.lives.ToString();

        if(player.lives > 0 && dealer.lives > 0)
        {
            ResetGame();
        }
        else{
            if(player.lives == 0)
                Debug.Log("Game Over");
            else
                Debug.Log("You win");
        }
    }

    HandResult CompareHands(int playerValue, int dealerValue)
    {
        
        if (playerValue > 21 && dealerValue > 21)
        {
            return HandResult.Tie;
        }
        else if (playerValue > 21)
        {
            return HandResult.DealerWin;
        }
        else if (dealerValue > 21)
        {
            return HandResult.PlayerWin;
        }
        else if (playerValue == dealerValue)
        {
            return HandResult.Tie;
        }
        else if (playerValue > dealerValue)
        {
            return HandResult.PlayerWin;
        }
        else
        {
            return HandResult.DealerWin;
        }
    }

    IEnumerator ElectrocuteDealer()
    {
        yield return StartCoroutine(player.LookAtDealer());
        audioSource.PlayOneShot(dealerElectrocutedSound);
        yield return new WaitForSeconds(14f); // time to run the electrocute
    }

    IEnumerator ElectrocutePlayer(int shakeCount)
    {
        yield return StartCoroutine(player.LookAtDealer());
        StartCoroutine(ShakeMultipleTimes(shakeCount));
        audioSource.PlayOneShot(playerElectrocutedSound);
        yield return new WaitForSeconds(14f); // time to run the electrocute
    }

    // Coroutine to shake the camera multiple times
    IEnumerator ShakeMultipleTimes(int shakeCount)
    {
        for (int i = 0; i < shakeCount; i++)
        {
            CameraShaker.Instance.ShakeOnce(.5f, 2f, .1f, 2f);
            yield return new WaitForSeconds(.5f);
        }
    }

    public void ResetGame()
    {
        Debug.Log("reset game");
        player.ClearHand();
        dealer.ClearHand();
        player.handValue = 0;
        dealer.handValue = 0;
        player.handValueUI.color = Color.white;
        StartCoroutine(StartRound());
    }
}
