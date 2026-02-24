using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StirringController : MonoBehaviour
{
    [SerializeField] GameFlowController2 gameFlowController;
    public StirableSprite stirableSprite;

    [Header("Progress")]
    public float requiredStirAmount = 720f; // Dua putaran
    public Slider progressSlider;
    public TMP_Text statusText;

    [Header("Stir Settings")]
    public float stirMultiplier = 1f;
    public float penaltyMultiplier = 0.5f;

    private bool isStirring = false;
    private Vector2 previousDirection;
    private float stirAmount = 0f;
    private bool isCompleted = false;

    private float idleTimer = 0f;
    [SerializeField] float idleThreshold = 5f;
    private bool isUserInteracting = false;
    [SerializeField] Animator idle;
    private bool idleActive = false;

    private void Awake()
    {
        progressSlider.gameObject.SetActive(true);
        idle.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCompleted) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif

        HandleIdleTimer();
        UpdateProgressUI();
        CheckCompletion();
    }

    void HandleIdleTimer()
    {
        if (isUserInteracting)
        {
            idleTimer = 0f;
            isUserInteracting = false;

            if (idleActive)
            {
                idle.SetBool("isIdle", false);
                idle.gameObject.SetActive(false);
                idleActive = false;
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold && !idleActive)
            {
                idle.gameObject.SetActive(true);
                idle.SetBool("isIdle", true);
                idleActive = true;
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isStirring = true;
            isUserInteracting = true;
        }
        else if (Input.GetMouseButton(0) && isStirring)
        {
            Vector2 currentDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Vector2.SignedAngle(previousDirection, currentDirection);

            HandleRotation(angle);
            previousDirection = currentDirection;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isStirring = false;
            isUserInteracting = true;
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            isUserInteracting = true;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    previousDirection = touchPos;
                    isStirring = true;
                    break;

                case TouchPhase.Moved:
                    if (isStirring)
                    {
                        Vector2 currentDirection = touchPos;
                        float angle = Vector2.SignedAngle(previousDirection, currentDirection);

                        HandleRotation(angle);
                        previousDirection = currentDirection;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isStirring = false;
                    break;
            }
        }
    }

    void HandleRotation(float angle)
    {
        if (angle == 0) return;

        if (angle < 0)
        {
            stirAmount += Mathf.Abs(angle) * stirMultiplier;
            Debug.Log("Searah jarum jam: +" + Mathf.Abs(angle));
        }
        else
        {
            stirAmount -= Mathf.Abs(angle) * penaltyMultiplier;
            stirAmount = Mathf.Max(0, stirAmount);
            Debug.Log("Berlawanan arah jarum jam: -" + Mathf.Abs(angle));
        }

        stirableSprite.RotateSprite(angle);
    }

    void UpdateProgressUI()
    {
        if (progressSlider != null)
        {
            progressSlider.value = Mathf.Clamp01(stirAmount / requiredStirAmount);
        }
    }

    void CheckCompletion()
    {
        if (stirAmount >= requiredStirAmount)
        {
            isCompleted = true;
            idle.SetBool("isIdle", false);
            idle.gameObject.SetActive(false);
            progressSlider.gameObject.SetActive(false);

            if (statusText != null)
            {
                statusText.text = "Aduk selesai!";
            }

            if (stirableSprite != null)
            {
                stirableSprite.SwitchToFinalState();
            }

            if (gameFlowController.currentStage == GameFlowController2.GameStage.Stirring)
            {
                gameFlowController.OnStirringFinished();
            }
        }
    }
}
