using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BaseGame : MonoBehaviour
{
    public static event Action OnGameOverGlobal;
    [Header("Default Set")]
    [SerializeField] GameUIManager uiManager;
    [SerializeField] Countdown countdown;

    [Header("Pop Ups")]
    public GameObject popUpCorrect;
    public GameObject popUpWrong;

    [Header("SFX")]
    public AudioClip wrongSFX;
    public AudioClip correctSFX;
    public AudioClip clickSFX;
    public AudioClip winSFX;
    public AudioClip gameoverSFX;
    public SFXPlayer sfxPlayer;

    //Scoring 
    protected int score = 100;

    public GameUIManager UIManager => uiManager;
    public Countdown Countdown => countdown;
    public int Score => score;
    private void Awake()
    {
        LoadSFX();
    }
    public void DeactivateAllPopUp()
    {
        UIManager.ratingPanel?.SetActive(false);
        UIManager.pausePanel?.SetActive(false);
        popUpWrong?.SetActive(false);
        popUpCorrect?.SetActive(false);
        UIManager.gameOverPanel?.SetActive(false);

    }
    private void LoadSFX()
    {
        wrongSFX = Resources.Load<AudioClip>("Audio/Wrong");
        correctSFX = Resources.Load<AudioClip>("Audio/Correct");
        clickSFX = Resources.Load<AudioClip>("Audio/Click");
        winSFX = Resources.Load<AudioClip>("Audio/Win");
        gameoverSFX = Resources.Load<AudioClip>("Audio/Gameover");

        if (wrongSFX == null || correctSFX == null || clickSFX == null)
        {
            Debug.LogWarning("Ada SFX yang tidak berhasil dimuat. Periksa path dan nama file.");
        }
    }
    public virtual void ShowCorrectPopUp()
    {
        StartCoroutine(ShowAndHideCorrectPopUp());
    }

    public virtual void ShowWrongPopUp()
    {
        StartCoroutine(ShowAndHideWrongPopUp());
    }

    public IEnumerator ShowAndHideCorrectPopUp()
    {
        yield return new WaitForSeconds(0.2f);
        popUpCorrect.SetActive(true);
        PlayCorrectSound();
        yield return new WaitForSeconds(1f);
        popUpCorrect.SetActive(false);
    }

    public IEnumerator ShowAndHideWrongPopUp()
    {
        yield return new WaitForSeconds(0.2f);
        popUpWrong.SetActive(true);
        PlayWrongSound();
        yield return new WaitForSeconds(1f);
        popUpWrong.SetActive(false);
    }

    public virtual void GameOver()
    {
        if (UIManager.gameOverPanel != null)
        {
            PlayGameOverSound();    
            UIManager.ShowGameOver();
            Countdown.SetPaused(true);
        }
        OnGameOverGlobal?.Invoke();
    }
    public void Win()
    {
        UIManager.ShowRatingPopUp();
    }

    public void Next(int index)
    {
        PlayClickSound();
        //int chapter = PlayerPrefs.GetInt("CurrentChapter", index);
        PlayerPrefs.SetInt("CurrentChapter", index);
        PlayerPrefs.SetInt("IsIntro", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Video Chapter");
    }

    public void PlayClickSound()
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            PlaySFX(clickSFX);
        }
    }
    public void PlayWrongSound()
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            PlaySFX(wrongSFX);
        }
    }
    public void PlayCorrectSound()
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            PlaySFX(correctSFX);
        }
    }
    public void PlayWinGameSound()
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            PlaySFX(winSFX);
        }
    }
    public void PlayGameOverSound()
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            PlaySFX(gameoverSFX);
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (sfxPlayer != null && clickSFX != null && AudioManager.Instance.sfxEnabled)
        {
            sfxPlayer.PlaySFX(clip);
        }
    }
}
