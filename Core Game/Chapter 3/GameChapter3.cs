using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameChapter3 : BaseGame
{
    public AudioClip pickUpSFX;
    public AudioClip putDownSFX;
    public AudioClip putToWaterSFX;

    [Header("Panel Choose Wrong Answer")]
    public GameObject wrongsmallpanel1;
    public GameObject wrongsmallpanel2;
    public GameObject wrongsmallpanel3;

    public GameFlowController3 flowController;

    [Header("Game Logic")]
    public int countWrongAnswer = 0;


    private void Awake()
    {
        score = 100;
    }

    private void Start()
    {
        sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

    public void Next()
    {
        base.Next(3);
    }

    public override void GameOver()
    {
        base.GameOver();
    }
}
