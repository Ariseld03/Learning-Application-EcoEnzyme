using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaterFillController : MonoBehaviour
{
    public Button waterButton;
    public Image containerImage;
    public Sprite[] containerSprites;
    public GameObject waterAnimation;

    public float targetVolume = 3000f;
    public float currentVolume = 0f;
    public float totalAccumulatedVolume = 0f;

    private float fillAmountPerInterval = 50f;
    private float fillInterval = 0.5f;
    private float fillTimer = 0f;

    private bool isPointerHeld = false;
    private bool isGameCompleted = false;

    public GameFlowController2 gameFlowController;
    [SerializeField] GameChapter2 game;

    int spriteIndex = 0;

    void Start()
    {
        waterAnimation.SetActive(false);
        containerImage.gameObject.SetActive(true);
        waterButton.gameObject.SetActive(true);

        waterButton.onClick.AddListener(() => Debug.Log("Button clicked (optional for test)"));

        EventTriggerListener.Get(waterButton.gameObject).onPointerDown += OnPointerDown;
        EventTriggerListener.Get(waterButton.gameObject).onPointerUp += OnPointerUp;
    }

    void OnEnable()
    {
        game.sfxPlayer.StopSFX();
        containerImage.gameObject.SetActive(true);
        waterButton.gameObject.SetActive(true);
        waterAnimation.SetActive(false);

        // Update sprite container sesuai volume saat ini
        UpdateContainerSprite(currentVolume);
    }

    void Update()
    {
        if (isGameCompleted || !isPointerHeld) return;

        fillTimer += Time.deltaTime;

        if (fillTimer >= fillInterval && currentVolume < targetVolume)
        {
            fillTimer = 0f;
            currentVolume += fillAmountPerInterval;
            currentVolume = Mathf.Min(currentVolume, targetVolume);

            Debug.Log($"Menambahkan 100 mL. Volume sekarang: {currentVolume} mL");
            UpdateContainerSprite(currentVolume);
        }
        if(totalAccumulatedVolume == targetVolume)
        {
            Debug.Log("Volume pas saat dilepas!");
            gameFlowController.currentStage = GameFlowController2.GameStage.FillWaterFinish;
            gameFlowController.OnCheckLastVideoAnimation();
            WinCondition();
        }
    }

    void OnPointerDown(GameObject go)
    {
        isPointerHeld = true;
        fillTimer = 0f;
        waterAnimation.SetActive(true);
        Debug.Log("Mulai mengisi...");
        game.PlaySFX(game.pourSFX);
    }

    void OnPointerUp(GameObject go)
    {
        isPointerHeld = false;
        waterAnimation.SetActive(false);
        Debug.Log("Berhenti mengisi.");
        game.sfxPlayer.StopSFX();


        if (currentVolume > 1000f)
        {
            Debug.Log("Wadah overload");
            game.ShowWrongNoPanelPopUp();
            currentVolume = 0f;
            fillTimer = 0f;
            return;
        }
        else if(currentVolume == 0f||spriteIndex==0)
        {
            return;
        }

        AddToTotalVolume();

        if (totalAccumulatedVolume == targetVolume)
        {
            isGameCompleted = true;
            Debug.Log("Volume pas saat dilepas!");
            gameFlowController.currentStage = GameFlowController2.GameStage.FillWaterFinish;
            gameFlowController.OnCheckLastVideoAnimation();
            WinCondition();
        }
        else if (totalAccumulatedVolume < targetVolume || totalAccumulatedVolume > targetVolume)
        {
            Debug.Log("Volume belum cukup, trigger animasi video pouring");
            waterButton.gameObject.SetActive(false);
            containerImage.gameObject.SetActive(false);
            gameFlowController.OnCheckLastVideoAnimation();
        }
    }

    public void AddToTotalVolume()
    {
        Debug.Log($"Menambahkan {currentVolume} mL ke total akumulasi");
        totalAccumulatedVolume += currentVolume;
    }

    public void ResetVolume()
    {
        currentVolume = 0f;
        fillTimer = 0f;
        isPointerHeld = false;
        isGameCompleted = false;
        UpdateContainerSprite(currentVolume);
        Debug.Log("Volume direset ke 0 dan sprite diperbarui.");
    }

    void UpdateContainerSprite(float volume)
    {
            spriteIndex= 0;

        if (volume <= 1000f && volume > 800f)
        {
            spriteIndex = 4;
            volume = 1000f;
            currentVolume= volume;
        }
        else if(volume > 1000f)
        {
            spriteIndex = 0;
        }
        else if (volume <= 800f && volume > 400f)
        {
            spriteIndex = 3;
            volume = 800f;
            currentVolume = volume;
        }
        else if (volume <= 400f && volume > 200f)
        {
            spriteIndex = 2;
            volume = 400f;
            currentVolume = volume;
        }
        else if (volume < 200f && volume > 100f)
        {
            spriteIndex = 1;
            volume = 200f;
            currentVolume = volume;
        }
        else if(volume <=100f)
        {
            spriteIndex = 0;
        }

        spriteIndex = Mathf.Clamp(spriteIndex, 0, containerSprites.Length - 1);

        if (containerImage.sprite != containerSprites[spriteIndex])
        {
            containerImage.sprite = containerSprites[spriteIndex];
            Debug.Log("Sprite diganti ke: " + containerImage.sprite.name);
        }
    }

    public void CheckIfOverTargetVolume()
    {
         if (totalAccumulatedVolume > targetVolume)
        {
            game.ShowWrongNoPanelPopUp();
            currentVolume = 0f;
            fillTimer = 0f;
            totalAccumulatedVolume = 0f;
            UpdateContainerSprite(currentVolume);
        }
    }

    public void WinCondition()
    {
        isPointerHeld = false;
        waterButton.onClick.RemoveAllListeners();
        waterButton.gameObject.SetActive(false);
        containerImage.gameObject.SetActive(false);
        waterAnimation.SetActive(false);
        this.gameObject.SetActive(false);
        Debug.Log("Minigame selesai! Air sudah 3000 mL.");
       
    }
}
