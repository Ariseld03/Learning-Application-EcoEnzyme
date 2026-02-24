using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameChapter4 : BaseGame
{
    public AudioClip spraySFX;
    public AudioClip wateringSFX;
    public AudioClip washSFX;
    public AudioClip pickUpSFX;
    public AudioClip putDownSFX;
    
    [Header("Controller")]
    public GameFlowController4 flowController;

    [Header("Game Logic")]
    private Mission currentMission;


    private void Awake()
    {
        score = 100;
    }

    private void Start()
    {
        sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

    public void Complete()
    {
        currentMission = null;
        base.Win();
    }

    public override void GameOver()
    {
        base.GameOver();

        if (currentMission != null)
            currentMission.Data?.SetActive(false);
    }
    public void Next()
    {
        base.Next(4);
    }
}
