using System.Collections;
using TMPro;
using UnityEngine;

public class GameFlowController4 : MonoBehaviour
{
    public GameChapter4 game;
    public GameUIManager gameUIManager;
    public GameObject fertilize;
    public GameObject washing;
    public GameObject watering;
    public GameObject biopest;
    public GameObject disinfect;

    public PlateManager plate;

    public enum GameStage
    {
        Fertilizer,
        Wash,
        Watering,
        Biopes,
        Disinfect
    }

    public GameStage currentStage = GameStage.Disinfect;

    private void Start()
    {
        disinfect.gameObject.SetActive(true);
    }

    public void DisinfectComplete()
    {
        currentStage = GameStage.Wash;
        if (currentStage == GameStage.Wash)
        {
            StartCoroutine(DelayedActivate(disinfect,washing));
        }
    }

    public void WashingComplete()
    {
        currentStage = GameStage.Watering;
        if (currentStage == GameStage.Watering)
        {
            StartCoroutine(DelayedActivate(washing,watering));
        }
    }
    public void FertilizeComplete()
    {
        currentStage = GameStage.Biopes;
        if (currentStage == GameStage.Biopes)
        {
            StartCoroutine(DelayedActivate(fertilize,biopest));
        }
    }

    public void WateringComplete()
    {
        currentStage = GameStage.Fertilizer;
        if (currentStage == GameStage.Fertilizer)
        {
            StartCoroutine(DelayedActivate(watering,fertilize));
        }
    }

    public void CompleteGame()
    {
        StartCoroutine(DelayedComplete(0.5f));
    }

    // ==== Coroutines ====
    IEnumerator DelayedActivate(GameObject objdeactive, GameObject objactive)
    {
        game.ShowCorrectPopUp();
        yield return new WaitForSeconds(1f);
        objdeactive.SetActive(false);
        if(currentStage == GameStage.Watering)
        {
            plate.DestroyPlate();
        }
        yield return new WaitForSeconds(0.5f);
        objactive.SetActive(true);
    }
    IEnumerator DelayedComplete(float delay)
    {
        yield return new WaitForSeconds(delay);
        game.Complete();
    }
}
