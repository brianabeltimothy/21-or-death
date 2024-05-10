using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider loadingSlider;

    [SerializeField] private GameObject playersHeart;
    [SerializeField] private GameObject dealersHeart;

    bool isBlinking = false;

    private void Update() {
        if(!isBlinking)
        {
            StartCoroutine(BlinkingTVScreen());
        }
    }

    IEnumerator BlinkingTVScreen()
    {
        isBlinking = true;
        yield return new WaitForSeconds(1f);
        playersHeart.SetActive(false);
        dealersHeart.SetActive(false);
        yield return new WaitForSeconds(1f);
        playersHeart.SetActive(true);
        dealersHeart.SetActive(true);
        isBlinking = false;
    }

    public void LoadLevelButton(string levelToLoad)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
