using System.Collections;
using UnityEngine;
using TMPro;

public class DraggableTanah : MonoBehaviour
{
    public float weight = 0f;
    public bool IsDragging { get; set; }
    public Vector3 LastPosition { get; set; }

    private Camera mainCam;
    public DragControllerTanah dragController;

    [Header("Prefab")]
    public GameObject draggablePrefab;
    Vector3 originalScale;

    private bool hasScored = false;
    private Animator idle;

    public GameChapter4 game;
    public GameFlowController4 gameFlowController;

    public Sprite targetAchievedSprite;
    public GameObject tanamanObject;

    private SpriteRenderer tanamanSpriteRenderer;

    private bool isDestroyed=false;

    void Start()
    {
        originalScale = transform.localScale;
        hasScored = false;
        mainCam = Camera.main;
        LastPosition = transform.position;
        idle = GetComponent<Animator>();

        if (dragController == null)
            dragController = FindObjectOfType<DragControllerTanah>();

        if (tanamanObject != null)
        {
            tanamanSpriteRenderer = tanamanObject.GetComponent<SpriteRenderer>();
        }
        switch (gameObject.tag)
        {
            case "Weight5": weight = 5f; break;
            default: weight = 0f; break;
        }
    }

    void LateUpdate()
    {
        if (IsDragging && !isDestroyed)
        {
            idle.SetBool("isDragging", true);
            if (idle.enabled)
            {
                idle.enabled = false;
            }
        }
        else if (!IsDragging && !isDestroyed) 
        {
            idle.SetBool("isDragging", false);
            if (!idle.enabled)
            {
                idle.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasScored && other.CompareTag("ValidDrop"))
        {
            hasScored = true;
            game.sfxPlayer.PlaySFX(game.putDownSFX);
            AddScore(weight);
            StartCoroutine(SmoothDestroyAndRespawn());
        }
    }

    void AddScore(float amount)
    {
        dragController.totalScore += amount;
    }

    public void ResetPositionSmooth()
    {
        StartCoroutine(SmoothMoveBack());
    }

    IEnumerator SmoothMoveBack()
    {
        if(idle != null)
            idle.enabled = false;
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
        isDestroyed = true;
        IsDragging = false;
        idle = GetComponent<Animator>();
        if (idle != null) idle.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(idle);

        float duration = 0.25f;
        float time = 0f;
        // originalScale = transform.localScale; // HAPUS BARIS INI
        Debug.Log("Starting destroy animation. Initial Scale (from field): " + originalScale);

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, time / duration);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Debug.Log("Time: " + time / duration);
        Debug.Log("Lerp completed. Final Scale: " + transform.localScale);


        if (dragController.totalScore < dragController.targetScore && draggablePrefab != null)
        {
            Spawn();
            Debug.Log("Object spawned.");
        }
        else if (dragController.totalScore == dragController.targetScore)
        {
            if (tanamanSpriteRenderer != null && targetAchievedSprite != null)
            {
                tanamanSpriteRenderer.sprite = targetAchievedSprite; // Ganti sprite tanaman
            }
            gameFlowController.FertilizeComplete();
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void Spawn()
    {
        GameObject newObj = Instantiate(draggablePrefab, LastPosition, Quaternion.identity);

        Vector3 fixedPos = LastPosition;
        fixedPos.z = 0f;
        newObj.transform.position = fixedPos;

        newObj.transform.localScale = originalScale;

        if (transform.parent != null)
            newObj.transform.SetParent(transform.parent, worldPositionStays: true);

        DraggableTanah newDraggable = newObj.GetComponent<DraggableTanah>();
        newDraggable.dragController = this.dragController;
        newDraggable.draggablePrefab = this.draggablePrefab;
        newDraggable.game = this.game;
        newDraggable.gameFlowController = this.gameFlowController;
        newDraggable.tanamanSpriteRenderer = this.tanamanSpriteRenderer;
        newDraggable.targetAchievedSprite = this.targetAchievedSprite;

        newObj.layer = gameObject.layer;
        foreach (Transform child in newObj.transform)
            child.gameObject.layer = gameObject.layer;

        Collider2D newCol = newObj.GetComponent<Collider2D>();
        if (newCol != null) newCol.enabled = true;
        
    }
}
