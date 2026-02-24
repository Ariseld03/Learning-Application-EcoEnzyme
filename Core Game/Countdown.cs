using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public BaseGame gameChapter;
    [SerializeField] TextMeshProUGUI countdownText;

    public float remainingTime = 70f; // Atur ke -1 untuk unlimited
    private bool isPaused;

    private void Start()
    {
        if (remainingTime == 0)
        {
            SetPaused(true);
        }
        isPaused = false;
    }

    void Update()
    {
        if (isPaused || remainingTime == -1f) return;

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            countdownText.color = Color.red;
            gameChapter.GameOver();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        countdownText.text = $"{minutes:00}:{seconds:00}";
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
}
