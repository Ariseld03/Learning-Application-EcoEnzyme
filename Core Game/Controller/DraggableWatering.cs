using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableWatering : MonoBehaviour
{
    [SerializeField] public GameFlowController4 gameFlowController;

    public bool IsDragging;
    
    [Header("Animator")]
    public Animator wateringAnimator;
    public Animator idle;

    private Collider2D col;

    private List<float> yPositions = new List<float>();
    private GameObject currentTarget = null;
    private bool targetCompleted = false;

    private float minX, maxX, minY, maxY;

    [SerializeField] private float minSwipeYCoverage = 0.6f;
    [SerializeField] private Sprite finishedSprite;

    void Start()
    {
        col = GetComponent<Collider2D>();
        wateringAnimator.applyRootMotion = false;
        wateringAnimator.SetBool("isSpraying", false);
        wateringAnimator.gameObject.SetActive(false);
        idle.SetBool("isDragging", false);

        // Hitung batas area berdasarkan ukuran kamera dan ukuran objek
        Camera cam = Camera.main;
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float halfWidth = col.bounds.extents.x;
        float halfHeight = col.bounds.extents.y;

        minX = cam.transform.position.x - horzExtent + halfWidth;
        maxX = cam.transform.position.x + horzExtent - halfWidth;
        minY = cam.transform.position.y - vertExtent + halfHeight;
        maxY = cam.transform.position.y + vertExtent - halfHeight;
    }

    void Update()
    {
        if (IsDragging)
        {
            wateringAnimator.gameObject.SetActive(true);
            wateringAnimator.SetBool("isSpraying", true);
            idle.SetBool("isDragging", true);
            if (idle.enabled)
                idle.enabled = false;

            yPositions.Add(transform.position.y);

            // Clamp posisi agar tetap di dalam layar
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            transform.position = clampedPosition;
        }
        else
        {
            wateringAnimator.gameObject.SetActive(false);
            wateringAnimator.SetBool("isSpraying", false);
            idle.SetBool("isDragging", false);
            if (!idle.enabled)
                idle.enabled = true;
        }
    }

    public void ResetTargets()
    {
        currentTarget = null;
        yPositions.Clear();
        targetCompleted = false;
    }

    public void CheckCoverageAndFinish()
    {
        if (currentTarget == null || targetCompleted) return;

        float minY = Mathf.Min(yPositions.ToArray());
        float maxY = Mathf.Max(yPositions.ToArray());
        float range = maxY - minY;

        if (range <= 0f) return;

        HashSet<int> uniqueSteps = new HashSet<int>();
        foreach (var y in yPositions)
        {
            float normalized = (y - minY) / range;
            int step = Mathf.FloorToInt(normalized * 100);
            uniqueSteps.Add(step);
        }

        float covered = uniqueSteps.Count / 100f;
        Debug.Log("Swipe Y Coverage: " + covered);

        if (covered >= minSwipeYCoverage)
        {
            targetCompleted = true;

            SpriteRenderer sr = currentTarget.GetComponent<SpriteRenderer>();
            if (sr != null && finishedSprite != null)
            {
                sr.sprite = finishedSprite;
            }

            Debug.Log("Target selesai disiram!");
            StartCoroutine(GoToNextMinigame());
        }

        yPositions.Clear();
    }
    public void SetCurrentTarget(GameObject target)
    {
        currentTarget = target;
    }

    IEnumerator GoToNextMinigame()
    {
        this.enabled = false;
        yield return new WaitForSeconds(2f);
        Debug.Log("Lanjut ke minigame berikutnya...");
        gameFlowController.WateringComplete();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget"))
        {
            currentTarget = other.gameObject;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("SprayTarget"))
        {
            currentTarget = other.gameObject;
        }
    }
}
