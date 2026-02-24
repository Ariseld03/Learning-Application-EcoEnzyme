using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowController2 : MonoBehaviour
{
    public WaterFillController waterFillController;

    public GameUIManager gameUIManager;
    public GameObject weightWasteObject;
    public GameObject weightSugarObject;
    public GameObject cuttingWasteObject;
    public GameObject cuttingSugarObject;
    public GameObject fillingWaterObject;
    public GameObject videoPlayerPouringWaterObject;
    public GameObject pouringWasteObject;
    public GameObject pouringSugarObject;
    public GameObject stirringObject;
    public GameObject closingObject;

    public GameObject animationPouringWaterObject;
    public Animator pouringAnimator;

    public GameChapter2 game;

    public GameStage currentStage = GameStage.WeightWaste;
    public enum GameStage
    {
        PouringSugar,
        PouringWaste,
        FillWater,
        FillWaterFinish,
        Stirring,
        WeightWaste,
        WeightSugar,
        Closing,
        CuttingSugar,
        CuttingWaste
    }

    private IEnumerator DelayNextMission(GameStage nextStage, float delay = 1f)
    {
        yield return new WaitForSeconds(delay);
        currentStage = nextStage;
        if(currentStage == GameStage.CuttingWaste || currentStage == GameStage.PouringWaste|| currentStage == GameStage.WeightSugar)
        {
            game.DeactiveWrongPanel();
        }
        game.LoadNextMission();
    }

    public void OnWeighWasteFinished()
    {
        if (currentStage == GameStage.WeightWaste && weightWasteObject.activeSelf)
        {
            StartCoroutine(DelayNextMission(GameStage.WeightSugar));
        }
    }

    public void OnWeighSugarFinished()
    {
        if (currentStage == GameStage.WeightSugar && weightSugarObject.activeSelf)
        {
            Debug.Log("Masuk Nimbang Gula");
            StartCoroutine(DelayNextMission(GameStage.CuttingWaste));
        }
    }

    public void OnCuttingWasteFinished()
    {
        game.ShowCorrectPopUp();
        StartCoroutine(DelayNextMission(GameStage.CuttingSugar));
    }

    public void OnCuttingSugarFinished()
    {
        game.ShowCorrectPopUp();
        StartCoroutine(DelayNextMission(GameStage.FillWater));
    }

    public void OnCheckLastVideoAnimation()
    {
        StartCoroutine(DelayCheckLastVideoAnimation());
    }

    private IEnumerator DelayCheckLastVideoAnimation()
    {
        yield return new WaitForSeconds(1f);

        if (currentStage == GameStage.FillWaterFinish)
        {
            game.missionText.gameObject.SetActive(false);
        }

        fillingWaterObject.SetActive(false);
        videoPlayerPouringWaterObject.SetActive(true);
    }

    public void OnVideoAnimationFinished()
    {
        StartCoroutine(DelayVideoFinished());
    }

    private IEnumerator DelayVideoFinished()
    {
        yield return new WaitForSeconds(1f);
        game.sfxPlayer.StopSFX();
        if (currentStage == GameStage.FillWaterFinish)
        {
            game.ShowCorrectPopUp();
            videoPlayerPouringWaterObject.SetActive(false);
            StartCoroutine(DelayNextMission(GameStage.PouringWaste)); // bisa diubah ke PouringSugar kalau mau langsung lanjut
        }
        else
        {
            ActivateFillingWater();
            videoPlayerPouringWaterObject.SetActive(false);
        }
    }

    public void ActivateFillingWater()
    {
        game.missionText.gameObject.SetActive(true);
        fillingWaterObject.SetActive(true);

        // Hidupkan ulang button dan image di WaterFillController
        if (waterFillController != null)
        {
            waterFillController.waterButton.gameObject.SetActive(true);
            waterFillController.containerImage.gameObject.SetActive(true);
            waterFillController.waterAnimation.SetActive(false);
        }
    }

    public void OnPouringWasteFinished()
    {
        game.ShowCorrectPopUp();
        StartCoroutine(DelayNextMission(GameStage.PouringSugar));
    }

    public void OnPouringSugarFinished()
    {
        game.ShowCorrectPopUp();
        StartCoroutine(DelayNextMission(GameStage.Stirring));
    }

    public void OnStirringFinished()
    {
        game.ShowCorrectPopUp();
        StartCoroutine(DelayNextMission(GameStage.Closing));
    }

    public void GameFinished()
    {
        StartCoroutine(DelayNextMission(currentStage,0.2f));
        game.Countdown.SetPaused(true);
    }
}
