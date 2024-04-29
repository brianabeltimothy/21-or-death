/*
 *  Author: ariel oliveira [o.arielg@gmail.com]
 */

using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Transform heartsParent;
    [SerializeField] private GameObject heartContainerPrefab;
    [SerializeField] private PlayerStats playerStats;

    private GameObject[] heartContainers;
    private Image[] heartFills;

    private void Start()
    {
        playerStats.onHealthChangedCallback += UpdateHeartsHUD;
        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }

    public void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

    private void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < playerStats.MaxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }

    private void SetFilledHearts()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            if (i < playerStats.Health)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }

        if (playerStats.Health % 1 != 0)
        {
            int lastPos = Mathf.FloorToInt(playerStats.Health);
            heartFills[lastPos].fillAmount = playerStats.Health % 1;
        }
    }

    private void InstantiateHeartContainers()
    {
        heartContainers = new GameObject[(int)playerStats.MaxTotalHealth];
        heartFills = new Image[(int)playerStats.MaxTotalHealth];

        for (int i = 0; i < playerStats.MaxTotalHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
}
