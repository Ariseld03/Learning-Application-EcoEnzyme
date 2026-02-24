using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ContaminationController : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay;
    public GameObject containerTime;
    public GameFlowController3 gameFlowController;
    public Button plusHourButton;
    public Button minusHourButton;

    private int hours = 0;
    private int minutes = 0;
    private int seconds = 0;

    private const int targetHours = 3;

    void Start()
    {
        plusHourButton.gameObject.SetActive(true);
        containerTime.gameObject.SetActive(true);
        timeDisplay.gameObject.SetActive(true);
        minusHourButton.gameObject.SetActive(true);
        
        UpdateTimeDisplay();

        plusHourButton.onClick.RemoveAllListeners();
        minusHourButton.onClick.RemoveAllListeners();

        plusHourButton.onClick.AddListener(() => ChangeHours(1));
        minusHourButton.onClick.AddListener(() => ChangeHours(-1));
    }

    void ChangeHours(int amount)
    {
        gameFlowController.game.PlayClickSound();
        hours += amount;
        hours = Mathf.Clamp(hours, 0, 23);
        UpdateTimeDisplay();
        CheckCompletion();
    }

    void UpdateTimeDisplay()
    {
        timeDisplay.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    void CheckCompletion()
    {
        if (hours == targetHours && minutes == 0 && seconds == 0)
        {
            Debug.Log("Penanganan selesai! Ecoenzyme telah dikarantina 3 jam.");
            plusHourButton.enabled = false;
            minusHourButton.enabled = false;
            gameFlowController.OnContaminationSolved();
        }
    }

    public IEnumerator HandleCompletionAfterDelay()
    {
        yield return new WaitForSeconds(1f); 
        plusHourButton.gameObject.SetActive(false);
        minusHourButton.gameObject.SetActive(false);
        timeDisplay.gameObject.SetActive(false);
        containerTime.gameObject.SetActive(false);
    }


}
