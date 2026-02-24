using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("Button")]
    public Button hint;
    public Button pause;

    [Header("Panels")]
    public BaseGame baseGame;
    public GameObject gameOverPanel;
    public GameObject hintPanel;
    public GameObject ratingPanel;
    [SerializeField] TextMeshProUGUI playerScore;
    public GameObject threeStar;
    public GameObject twoStar;
    public GameObject oneStar;
    public GameObject pausePanel;

    void Start()
    {
        if (hint != null && hintPanel != null)
        {
            hint.onClick.RemoveAllListeners();
            hint.onClick.AddListener(() =>
            {
                hintPanel.SetActive(!hintPanel.activeSelf);
                baseGame.PlayClickSound();
            });
        }

        pause.onClick.RemoveAllListeners();
        pause.onClick.AddListener(() =>
        {
            OpenPausePopUp();
        });
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (baseGame != null && baseGame.Countdown != null)
        {
            baseGame.Countdown.SetPaused(true);
        }
    }

    public void ShowRatingPopUp()
    {
        StartCoroutine(ShowRatingPopUpWithDelay());
    }

    private IEnumerator ShowRatingPopUpWithDelay()
    {
        DeactivateStarDisplay();

        yield return new WaitForSeconds(1.2f);

        ratingPanel.SetActive(true);

        if (baseGame != null && baseGame.Countdown != null)
        {
            baseGame.Countdown.SetPaused(true);
        }

        baseGame.PlayWinGameSound();

        playerScore.text = $"Skor: {baseGame.Score.ToString()}";

        if (baseGame.Score > 75 && baseGame.Score <= 100)
        {
            threeStar.SetActive(true);
        }
        else if (baseGame.Score > 50 && baseGame.Score <= 75)
        {
            twoStar.SetActive(true);
        }
        else if (baseGame.Score <= 50)
        {
            oneStar.SetActive(true);
        }
    }

    public void DeactivateStarDisplay()
    {
        oneStar.SetActive(false);
        twoStar.SetActive(false);
        threeStar.SetActive(false);
    }

    public void OpenPausePopUp()
    {
        pausePanel.SetActive(true);
        baseGame.PlayClickSound();
        if (baseGame != null && baseGame.Countdown != null)
        {
            baseGame.Countdown.SetPaused(true);
        }
    }

    public void ClosePausePopUp()
    {
        pausePanel.SetActive(false);
        baseGame.PlayClickSound();
        if (baseGame != null && baseGame.Countdown != null)
        {
            baseGame.Countdown.SetPaused(false);
        }
    }

    public void RestartGame()
    {
        baseGame.PlayClickSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        baseGame.PlayClickSound();
        StartCoroutine(LoadMainMenuScene());
    }
    IEnumerator LoadMainMenuScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f); 
        asyncLoad.allowSceneActivation = true;
    }
    public void BackToList()
    {
        baseGame.PlayClickSound();
        StartCoroutine(LoadListChapterScene());
    }
    IEnumerator LoadListChapterScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("List Chapter");
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSeconds(0.5f);
        asyncLoad.allowSceneActivation = true;
    }
}
