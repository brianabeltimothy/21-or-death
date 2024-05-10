using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{   
    public static UIManager Instance;

    [SerializeField] private GameObject hitButton;
    [SerializeField] private GameObject standButton;
    [SerializeField] private GameObject playerValueHandText;
    [SerializeField] private GameObject dealerValueHandText;
    [SerializeField] private GameObject hiddenCardText;
    [SerializeField] private GameObject screenDamage;
    [SerializeField] private Image screenDamageImage;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private Image blackScreenImage;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject youWinScreen;

    public TextMeshProUGUI playerLivesText;
    public TextMeshProUGUI dealerLivesText;

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        screenDamage.SetActive(false);
        blackScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        youWinScreen.SetActive(false);
    }

    public void DisableAll()
    {
        hitButton.SetActive(false);
        standButton.SetActive(false);
        playerValueHandText.SetActive(false);
        dealerValueHandText.SetActive(false);
        hiddenCardText.SetActive(false);
    }

    public void EnableAll()
    {
        hitButton.SetActive(true);
        standButton.SetActive(true);
        playerValueHandText.SetActive(true);
        dealerValueHandText.SetActive(true);
        hiddenCardText.SetActive(true);
    }

    public void DisablePlayerMove()
    {
        hitButton.SetActive(false);
        standButton.SetActive(false);
    }

    public void EnablePlayerMove()
    {
        hitButton.SetActive(true);
        standButton.SetActive(true);
    }

    public void ShowScreenDamage()
    {
        screenDamage.SetActive(true); 
    }
    
    public IEnumerator FadeOutScreenDamage()
    {
        yield return FadeToAlpha(screenDamageImage, 0f, 2f);
        screenDamage.SetActive(false); 
        yield return FadeToAlpha(screenDamageImage, 1f, 2f);
    }

    public IEnumerator ShowGameOverScreen()
    {
        blackScreen.SetActive(true);
        yield return FadeToAlpha(blackScreenImage, 1f, 2f);
        gameOverScreen.SetActive(true);
        yield return FadeToAlpha(blackScreenImage, 0f, 2f);
        blackScreen.SetActive(false);
    }

    public IEnumerator StartGameFadeOut()
    {
        Color currentColor = blackScreenImage.color;
        currentColor.a = 1f;
        blackScreenImage.color = currentColor;

        blackScreen.SetActive(true);

        yield return FadeToAlpha(blackScreenImage, 0f, 2f);

        blackScreen.SetActive(false);
        
        currentColor.a = 0f;
        blackScreenImage.color = currentColor;
    }

    public IEnumerator ShowYouWinScreen()
    {
        blackScreen.SetActive(true);
        yield return FadeToAlpha(blackScreenImage, 1f, 2f);
        youWinScreen.SetActive(true);
        yield return FadeToAlpha(blackScreenImage, 0f, 2f);
        blackScreen.SetActive(false);
    }

    IEnumerator FadeToAlpha(Image img, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColor = img.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

        while (elapsedTime < duration)
        {
            img.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final color is exactly the target color
        img.color = targetColor;
    }
}
