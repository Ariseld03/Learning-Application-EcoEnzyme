using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DraggableWeigh : MonoBehaviour
{
    public float weight = 0f;
    public bool IsDragging { get; set; }
    public Vector3 LastPosition { get; set; }

    private DragControllerWeigh dragController;

    [Header("Prefab")]
    public GameObject draggablePrefab;
    Vector3 originalScale;

    private TMP_Text cachedScoreText;
    public bool hasScored = false;

    public GameChapter2 game;
    public GameFlowController2 gameFlowController;
    Collider2D colDispos;

    [HideInInspector]
    public bool isRespawnedFromDrop = false;

    void Start()
    {
        hasScored = false;
        LastPosition = transform.position;
        originalScale = transform.localScale;

        if (dragController == null)
            dragController = FindObjectOfType<DragControllerWeigh>();

        dragController.dragEnabled = true;

        if (dragController.scoreText != null)
            cachedScoreText = dragController.scoreText;

        CheckWeight();
        if (isRespawnedFromDrop)
        {
            SubstractScore();
        }
    }
    public void CheckWeight()
    {
        switch (gameObject.tag)
        {
            case "Weight175": weight = 175f; break;
            case "Weight40": weight = 40f; break;
            case "Weight25": weight = 25f; break;
            default: weight = 0f; break;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasScored && !isRespawnedFromDrop && other.CompareTag("ValidDrop"))
        {
            hasScored = true;
            game.PlaySFX(game.dropDownSFX);
            AddScore();

            GameObject obj = GameObject.FindWithTag("SpawnDisposable");
            if (obj != null)
            {
                colDispos = obj.GetComponent<Collider2D>();
            }
            DisposableDraggableController handler = colDispos.GetComponent<DisposableDraggableController>();
            if (handler != null)
            {
                handler.AddToDroppedList(this.draggablePrefab);
            }

            StartCoroutine(SmoothDestroyAndRespawn());
        }
    }

    void AddScore()
    {
        dragController.totalScore += weight;
        if (dragController.scoreText != null)
        {
            dragController.scoreText.text = dragController.totalScore.ToString("F0") + "g";
        }
    }

    public void SubstractScore()
    {
        dragController.totalScore -= weight;
        if (dragController.scoreText != null)
        {
            dragController.scoreText.text = dragController.totalScore.ToString("F0") + "g";
        }
    }

    public void ResetPositionSmooth()
    {
        StartCoroutine(SmoothMoveBack());
    }

    IEnumerator SmoothMoveBack()
    {
        Vector3 start = transform.position;
        float t = 0f;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, LastPosition, t / duration);
            yield return null;
        }

        transform.position = LastPosition;
    }

    public IEnumerator SmoothDestroyAndRespawn()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
        
        IsDragging = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        float duration = 0.25f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, time / duration);
            yield return null;
        }

        if (dragController.totalScore < dragController.targetScore && draggablePrefab != null)
        {
            Spawn();
        }
        else if (dragController.totalScore == dragController.targetScore)
        {
            if(gameFlowController.currentStage == GameFlowController2.GameStage.WeightWaste)
            {
                game.ShowCorrectPopUp();
                gameFlowController.OnWeighWasteFinished();
                Debug.Log("Misi Pertama Selesai");
                dragController.totalScore = 0;
                dragController.scoreText.text = dragController.totalScore.ToString("F0") + "g";
                game.countWrongAnswer = 0;
                dragController.dragEnabled = false;
            }
            else if(gameFlowController.currentStage == GameFlowController2.GameStage.WeightSugar)
            {
                game.ShowCorrectPopUp();
                dragController.scoreText.gameObject.SetActive(false);
                game.missionText.gameObject.SetActive(false);
                gameFlowController.OnWeighSugarFinished();
                Debug.Log("Misi Kedua Selesai");
                game.countWrongAnswer = 0;
                dragController.dragEnabled = false;
                OnDestroy();
            }
        }
        else if (dragController.totalScore > dragController.targetScore)
        {
            game.ShowWrongPopUp();
            dragController.totalScore = 0;
            dragController.scoreText.text = dragController.totalScore.ToString("F0") + "g";
            Spawn();
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    public void StartLerpBackThenDestroy()
    {
        StartCoroutine(LerpAndDestroy());
    }

    IEnumerator LerpAndDestroy()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.enabled = false;
 
        Collider2D col = this.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Vector3 currentScale = transform.localScale;

        IsDragging = false;

        float duration = 0.45f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(currentScale, Vector3.zero, time / duration);
            yield return null;
        }

        Destroy(gameObject);
    }

    void Spawn()
    {
        GameObject newObj = Instantiate(draggablePrefab, LastPosition, Quaternion.identity);

        Vector3 fixedPos = LastPosition;
        fixedPos.z = 0f; 
        newObj.transform.position = fixedPos;

        newObj.transform.localScale = originalScale; 

        if (!newObj.activeSelf)
            newObj.SetActive(true); 

        if (transform.parent != null)
            newObj.transform.SetParent(transform.parent, worldPositionStays: true);

        DraggableWeigh newDraggable = newObj.GetComponent<DraggableWeigh>();
        newDraggable.dragController = this.dragController;
        newDraggable.draggablePrefab = this.draggablePrefab;
        newDraggable.game = this.game;
        newDraggable.gameFlowController = this.gameFlowController;
        newDraggable.isRespawnedFromDrop = false;

        newObj.layer = gameObject.layer;
        foreach (Transform child in newObj.transform)
            child.gameObject.layer = gameObject.layer;

        Collider2D newCol = newObj.GetComponent<Collider2D>();
        if (newCol != null) newCol.enabled = true;

        if (dragController != null)
        {
            Animator anim = newObj.GetComponent<Animator>();
            dragController.AddDraggableAnimator(anim);
        }
        Debug.Log("Spawned new draggable at: " + fixedPos);
        Debug.Log("Original scale: " + originalScale);

    }

    private void OnDestroy()
    {
        if (TryGetComponent<Animator>(out var anim))
        {
            dragController?.RemoveDraggableAnimator(anim);
        }
    }

    private void OnEnable()
    {
        if (TryGetComponent<Animator>(out var anim))
        {
            FindObjectOfType<DragControllerWeigh>()?.AddDraggableAnimator(anim);
        }
    }
}
