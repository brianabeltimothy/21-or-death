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
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerStats dealerStats;

    [SerializeField] private GameObject playerFirstCard;
    [SerializeField] private GameObject playerSecondCard;
    [SerializeField] private GameObject dealerFirstCard;
    [SerializeField] private GameObject dealerSecondCard;
    [SerializeField] private GameObject playersHeart;
    [SerializeField] private GameObject dealersHeart;
    [SerializeField] private GameObject playerElectricity;
    [SerializeField] private GameObject dealerElectricity;

    private Transform playerNextCardPos;
    private Transform dealerNextCardPos;
    private bool hitButtonPressed = false;

    //sounds
    private AudioSource audioSource;
    [Header("Sounds")]
    [SerializeField] private AudioClip drawCardFlick; 
    [SerializeField] private AudioClip playerElectrocutedSound;
    [SerializeField] private AudioClip dealerElectrocutedSound;
    [SerializeField] private AudioClip retroButtonSound;

    //dialogues
    [Header("Dialogues")]
    [SerializeField] private DialogueTrigger rulesDialogue;
    [SerializeField] private DialogueTrigger shuffleDialogue;
    [SerializeField] private DialogueTrigger youWinDialogue;
    [SerializeField] private DialogueTrigger youLoseDialogue;
    [SerializeField] private DialogueTrigger tieDialogue;
    [SerializeField] private DialogueTrigger dealerStandDialogue;
    [SerializeField] private DialogueTrigger playerWinDialogue;

    [Header("Animator")]
    [SerializeField] private Animator dealerAnimator;

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
        playerElectricity.SetActive(false);
        dealerElectricity.SetActive(false);

        StartCoroutine(StartFirstRound());
    }


    public IEnumerator StartFirstRound()
    {
        yield return StartCoroutine(uiManager.StartGameFadeOut());
        rulesDialogue.StartDialogue();
        yield return new WaitForSeconds(20f); //dont forget
        // yield return null; //temp
        yield return StartCoroutine(player.LookAtTV());
        yield return new WaitForSeconds(1.5f); 
        playersHeart.SetActive(true);
        dealersHeart.SetActive(true);
        audioSource.PlayOneShot(retroButtonSound);
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(player.returnCameraPosition());
        
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
            yield return StartCoroutine(player.LookAtDealer());
            deck.ResetDeck();
            deck.ShuffleDeck();
            shuffleDialogue.StartDialogue();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(player.LookAtTable());
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
        if (hitButtonPressed)
        {
            hitButtonPressed = false;
        }
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
        if (!hitButtonPressed)
        {
            StartCoroutine(DrawCardForPlayer(playerNextCardPos, player));
            hitButtonPressed = true;
        }
    }

    public void Stand()
    {
        uiManager.DisablePlayerMove();
        StartCoroutine(DealerTurn());
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
            yield return StartCoroutine(ElectrocutePlayer(18));
        }
        else
        {
            tieDialogue.StartDialogue();
            yield return new WaitForSeconds(2f);
        }

        //tv players lives
        if(result == HandResult.DealerWin)
        {
            yield return StartCoroutine(player.LookAtTV());
            yield return new WaitForSeconds(1.5f); 
            playerStats.TakeDamage(1f);
            audioSource.PlayOneShot(retroButtonSound);
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(player.returnCameraPosition());
        }
        else if(result == HandResult.PlayerWin)
        {
            yield return StartCoroutine(player.LookAtTV());
            yield return new WaitForSeconds(1.5f); 
            dealerStats.TakeDamage(1f);
            audioSource.PlayOneShot(retroButtonSound);
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(player.returnCameraPosition());
        }

        uiManager.playerLivesText.text = player.lives.ToString();
        uiManager.dealerLivesText.text = dealer.lives.ToString();

        if(player.lives > 0 && dealer.lives > 0)
        {
            ResetGame();
        }
        else{
            if(player.lives == 0)
                yield return StartCoroutine(uiManager.ShowGameOverScreen());
            else
                yield return StartCoroutine(PlayerWins());
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

    IEnumerator PlayerWins()
    {
        yield return StartCoroutine(player.LookAtDealer());
        playerWinDialogue.StartDialogue();
        yield return new WaitForSeconds(7f);
        yield return StartCoroutine(uiManager.ShowYouWinScreen());
    }

    IEnumerator ElectrocuteDealer()
    {
        yield return StartCoroutine(player.LookAtDealer());
        yield return new WaitForSeconds(1.5f);
        dealerAnimator.ResetTrigger("Idle");
        dealerAnimator.SetTrigger("Laugh");
        audioSource.PlayOneShot(dealerElectrocutedSound);
        dealerElectricity.SetActive(true);
        yield return new WaitForSeconds(10f); // time to run the electrocute
        dealerAnimator.ResetTrigger("Laugh");
        dealerAnimator.SetTrigger("Idle");
        dealerElectricity.SetActive(false);
        yield return new WaitForSeconds(2f);
    }

    IEnumerator ElectrocutePlayer(int shakeCount)
    {
        yield return StartCoroutine(player.LookAtDealer());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ShakeMultipleTimes(shakeCount));
        playerElectricity.SetActive(true);
        uiManager.ShowScreenDamage();
        audioSource.PlayOneShot(playerElectrocutedSound);
        yield return new WaitForSeconds(9f); // time to run the electrocute
        playerElectricity.SetActive(false);
        yield return StartCoroutine(uiManager.FadeOutScreenDamage());
    }

    // Coroutine to shake the camera multiple times
    IEnumerator ShakeMultipleTimes(int shakeCount)
    {
        for (int i = 0; i < shakeCount; i++)
        {
            CameraShaker.Instance.ShakeOnce(1f, 4f, .1f, 2f);
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
