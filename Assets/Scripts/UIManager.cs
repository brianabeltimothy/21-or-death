using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{   
    public static UIManager Instance;

    [SerializeField] private GameObject hitButton;
    [SerializeField] private GameObject standButton;
    [SerializeField] private GameObject playerValueHandText;
    [SerializeField] private GameObject dealerValueHandText;
    [SerializeField] private GameObject hiddenCardText;
    public TextMeshProUGUI playerLivesText;
    public TextMeshProUGUI dealerLivesText;

    private void Awake() {
        Instance = this;
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
}
