using System.Collections;
using TMPro;
using UnityEngine;

public class GameFlowController3 : MonoBehaviour
{ 
    [SerializeField] TextMeshProUGUI hintText;
    public GameChapter3 game;
    public GameUIManager gameUIManager;
    public FermentationController fermentationController;
    public GameObject timeAcceleratorObject;
    public ContaminationController contaminationController;
    public GameObject videoPlayerObject;

    public GameObject fermentationObject;
    public GameObject contaminationObject;
    public GameObject animationObject;
    public Animator pouringAnimator;

    public enum GameStage
    {
        Fermentation1,
        TimeAccel1,
        Fermentation2,
        Contamination,
        TimeAccel2,
        Animation,
        Fermentation3,
        TimeAccelFinal
    }

    public GameStage currentStage = GameStage.Fermentation1;

    private void Start()
    {
        fermentationController.gameObject.SetActive(true);
        fermentationController.EnableFermentationMinigame();
        hintText.text = "Letakkan di tempat yang sejuk dan tidak terpapar sinar matahari langsung";
        StartCoroutine(StartHintBeginning());
    }

    public void OnFirstMissionComplete()
    {
        game.ShowCorrectPopUp();
        currentStage = GameStage.TimeAccel1;
        if (currentStage == GameStage.TimeAccel1 && !timeAcceleratorObject.activeSelf)
        {
            StartCoroutine(DelayedActivate(timeAcceleratorObject, 1f));
            hintText.text = "Lakukan pengecekan selama 3 minggu pertama";
            StartCoroutine(StartHintBeginning());
        }
    }

    public void OnTimeAccelFinished()
    {
        game.ShowCorrectPopUp();
        if (currentStage == GameStage.TimeAccel1 && timeAcceleratorObject.activeSelf)
        {
            Debug.Log("Masuk Tahap Fermentasi lagi");
            currentStage = GameStage.Fermentation2;
            StartCoroutine(DeactiveUITimeAccel());
            StartCoroutine(DelayedFermentationResume(1f));
            hintText.text = "Ada yang cairan yang terkontaminasi, jemur di bawah sinar matahari langsung";
            StartCoroutine(StartHintBeginning());
        }
        else if (currentStage == GameStage.TimeAccel2)
        {
            currentStage = GameStage.Animation;
            StartCoroutine(DeactiveUITimeAccel());
            Debug.Log("Masuk Animasi");
            hintText.text = "Tidak ada petunjuk";
            StartCoroutine(DelayedAnimationTransition(1f));
            gameUIManager.hint.gameObject.SetActive(false);
        }
        else if (currentStage == GameStage.TimeAccelFinal)
        {
            CompleteGame();
        }
    }

    public void OnSecondMissionComplete()
    {
        game.ShowCorrectPopUp();
        currentStage = GameStage.Contamination;
        StartCoroutine(DelayedDeactivate(fermentationObject, 1f));
        hintText.text = "Jemur selama 3 jam";
        StartCoroutine(DelayedActivate(contaminationObject, 1f));
        StartCoroutine(StartHintBeginning());
    }

    public void OnContaminationSolved()
    {
        currentStage = GameStage.TimeAccel2;
        StartCoroutine(DelayedDeactivate(contaminationObject, 1f));
        StartCoroutine(contaminationController.HandleCompletionAfterDelay());
        if (currentStage == GameStage.TimeAccel2)
        {
            Debug.Log("Kembali ke akselerasi.");
            hintText.text = "Lakukan penjemuran selama 3 hari";
            StartCoroutine(DelayedActivateTimeAccel(1f));
            StartCoroutine(StartHintBeginning());
        }
    }

    public void PlaySugarPouringAnimation()
    {
        pouringAnimator.SetTrigger("Pour");
    }

    public void OnVideoAnimationFinished()
    {
        currentStage = GameStage.Fermentation3;
        videoPlayerObject.gameObject.SetActive(false);
        Debug.Log("VideoPlayer active: " + videoPlayerObject.gameObject.activeSelf + " dan akan mematikan animationObject");
        animationObject.SetActive(false);
        gameUIManager.hint.gameObject.SetActive(true);
        hintText.text = "Kembalikan ke tempat penyimpanan";
        StartCoroutine(DelayedActivateFermentationAfterVideo(1f));
        StartCoroutine(StartHintBeginning());
    }

    public void OnThirdMissionComplete()
    {
        currentStage = GameStage.TimeAccelFinal;
        StartCoroutine(DelayedActivateTimeAccel(1f));
        hintText.text = "Selesaikan pengecekan 2 minggu pengecekan lalu, fermentasikan selama 3 bulan";
        StartCoroutine(StartHintBeginning());
    }

    public void CompleteGame()
    {
        game.Countdown.SetPaused(true);
        StartCoroutine(DelayedWin(1.3f));
    }

    // ==== Coroutines ====
    IEnumerator DelayedWin(float delay)
    {
        yield return new WaitForSeconds(delay);
        game.Win();
    }

    IEnumerator DeactiveUITimeAccel()
    {
        yield return new WaitForSeconds(1f);
        timeAcceleratorObject.transform.Find("Overlay").gameObject.SetActive(false);
        timeAcceleratorObject.transform.Find("Kalender Full").gameObject.SetActive(false);
    }

    IEnumerator DelayedActivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(true);
    }

    IEnumerator DelayedDeactivate(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    IEnumerator DelayedFermentationResume(float delay)
    {
        yield return new WaitForSeconds(delay);
        fermentationObject.SetActive(true);
        fermentationController.EnableFermentationMinigame();
    }

    IEnumerator DelayedAnimationTransition(float delay)
    {
        yield return new WaitForSeconds(delay);
        videoPlayerObject.gameObject.SetActive(true);
        PlaySugarPouringAnimation();
    }

    IEnumerator DelayedActivateFermentationAfterVideo(float delay)
    {
        yield return new WaitForSeconds(delay);
        fermentationObject.SetActive(true);
        fermentationController.EnableFermentationMinigame();
    }

    IEnumerator DelayedActivateTimeAccel(float delay)
    {
        game.ShowCorrectPopUp();
        yield return new WaitForSeconds(delay);
        timeAcceleratorObject.transform.root.gameObject.SetActive(true);
        timeAcceleratorObject.transform.Find("Overlay").gameObject.SetActive(true);
        timeAcceleratorObject.transform.Find("Kalender Full").gameObject.SetActive(true);
        timeAcceleratorObject.GetComponent<TimeAccelerator>().UpdateTimeAcceleration();
    }

    IEnumerator StartHintBeginning()
    {
        yield return new WaitForSeconds(1.5f);
        gameUIManager.hintPanel.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameUIManager.hintPanel.gameObject.SetActive(false);
    }
}
