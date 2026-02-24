using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Globalization;

public class TimeAccelerator : MonoBehaviour
{
    public GameFlowController3 gameFlowController;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI monthYearText;
    public Button skipDayButton;
    public Button skipMonthButton;
    public GameObject userInterface;

    private DateTime currentDate;
    private DateTime targetDate;
    private DateTime startDate;

    private void Awake()
    {
        ResetCurrentDate();
        BeginTimeAcceleration();
    }

    public void BeginTimeAcceleration()
    {
        Debug.Log("[TimeAccelerator] BeginTimeAcceleration dipanggil");

        // Inisialisasi ulang tanggal
        currentDate = DateTime.Today;
        startDate = currentDate;

        DateTime plusThreeMonths = currentDate.AddMonths(3);
        targetDate = plusThreeMonths.AddDays(22);
        SaveCurrentDate();

        userInterface.SetActive(true);
        dayText.gameObject.SetActive(true);
        monthYearText.gameObject.SetActive(true);
        UpdateUIDate();
        SetupButtons();
    }

    public void UpdateTimeAcceleration()
    {
        skipDayButton.enabled = true;
        skipMonthButton.enabled = true;

        LoadSavedDate();
        startDate = currentDate;
        
        if (!userInterface.activeSelf)
        {
            Debug.Log("User interface belum aktif, mengaktifkan sekarang...");
            userInterface.SetActive(true);
        }
        UpdateUIDate();
        SetupButtons();
    }

    void SetupButtons()
    {
        skipDayButton.gameObject.SetActive(true);
        skipMonthButton.gameObject.SetActive(true);

        skipDayButton.onClick.RemoveAllListeners();
        skipMonthButton.onClick.RemoveAllListeners();

        skipDayButton.onClick.AddListener(SkipOneDay);
        skipMonthButton.onClick.AddListener(SkipOneMonth);
        
        if (gameFlowController.currentStage == GameFlowController3.GameStage.TimeAccel1 || gameFlowController.currentStage == GameFlowController3.GameStage.TimeAccel2)
        {
            skipMonthButton.gameObject.SetActive(false);
        }
    }

    void SkipOneDay()
    {
        gameFlowController.game.PlayClickSound();
        currentDate = currentDate.AddDays(1);
        UpdateUIDate();
        CheckProgress();
    }

    void SkipOneMonth()
    {
        gameFlowController.game.PlayClickSound();
        currentDate = currentDate.AddMonths(1);
        UpdateUIDate();
        CheckProgress();
    }

    void UpdateUIDate()
    {
        Debug.Log($"[TimeAccelerator] UpdateUI: {currentDate.ToString("yyyy-MM-dd")}");

        CultureInfo culture = new CultureInfo("id-ID");

        dayText.text = currentDate.Day.ToString();
        monthYearText.text = currentDate.ToString("MMMM yyyy", culture); 
    }

    void CheckProgress()
    {
        int daysPassed = (currentDate - startDate).Days;
        Debug.Log($"[TimeAccelerator] Days passed: {daysPassed}");

        switch (gameFlowController.currentStage)
        {
            case GameFlowController3.GameStage.TimeAccel1:
                if (daysPassed == 5)
                {
                    Debug.Log("5 hari sudah berlalu - lanjut ke Fermentation2");
                    CompleteThisStage();
                }
                break;

            case GameFlowController3.GameStage.TimeAccel2:
                if (daysPassed == 3)
                {
                    Debug.Log("8 hari sudah berlalu - lanjut ke animasi");
                    CompleteThisStage();
                }
                break;

            case GameFlowController3.GameStage.TimeAccelFinal:
                if (currentDate >= targetDate)
                {
                    Debug.Log("Target 4 bulan tercapai - game selesai");
                    CompleteThisStage();
                }
                break;
        }
    }

    void CompleteThisStage()
    {
        Debug.Log($"[TimeAccelerator] CompleteThisStage -> Start: {startDate}, Current: {currentDate}");
        SaveCurrentDate();
        skipDayButton.enabled = false;
        skipMonthButton.enabled = false;
        StartCoroutine(DelayedCompleteStage());
    }

    // === Utilities ===

    public void SaveCurrentDate()
    {
        PlayerPrefs.SetString("CurrentDate", currentDate.ToString("yyyy-MM-dd"));
        Debug.Log($"[TimeAccelerator] CurrentDate disimpan: {currentDate.ToString("yyyy-MM-dd")}");
    }

    public void LoadSavedDate()
    {
        if (PlayerPrefs.HasKey("CurrentDate"))
        {
            currentDate = DateTime.Parse(PlayerPrefs.GetString("CurrentDate"));
            Debug.Log($"[TimeAccelerator] LoadSavedDate -> {currentDate.ToString("yyyy-MM-dd")}");
        }
        else
        {
            currentDate = DateTime.Today;
            Debug.Log("[TimeAccelerator] Tidak ada CurrentDate, pakai DateTime.Today");
        }
    }

    public DateTime GetCurrentDate()
    {
        return currentDate;
    }

    public DateTime GetFinalDate()
    {
        if (PlayerPrefs.HasKey("FinalDate"))
        {
            return DateTime.Parse(PlayerPrefs.GetString("FinalDate"));
        }
        return DateTime.Today;
    }

    public void ResetCurrentDate()
    {
        if (PlayerPrefs.HasKey("CurrentDate"))
        {
            PlayerPrefs.DeleteKey("CurrentDate");
            Debug.Log("[TimeAccelerator] CurrentDate dihapus dari PlayerPrefs");
        }
    }

    IEnumerator DelayedCompleteStage()
    {
        Debug.Log("[TimeAccelerator] Delay sebelum transisi stage...");
        yield return new WaitForSeconds(2f); 

        if (gameFlowController != null)
        {
            if (gameFlowController.currentStage == GameFlowController3.GameStage.TimeAccelFinal)
            {
                PlayerPrefs.SetString("FinalDate", currentDate.ToString("yyyy-MM-dd"));
                Debug.Log("[TimeAccelerator] FinalDate saved to PlayerPrefs");
            }
            else
            {
                StartCoroutine(DelayedDeactiveUITimeAccel());
            }
            gameFlowController.OnTimeAccelFinished();
        }
        else
        {
            Debug.LogWarning("[TimeAccelerator] GameFlowController belum di-assign!");
        }
    }
    IEnumerator DelayedDeactiveUITimeAccel()
    {
        yield return new WaitForSeconds(1f);
        userInterface.SetActive(false);
    }

}
