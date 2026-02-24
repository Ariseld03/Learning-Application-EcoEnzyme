using UnityEngine;
using System.Collections.Generic;

public class WastePourController : MonoBehaviour
{
    [SerializeField] GameChapter2 game;

    [SerializeField] GameFlowController2 gameFlowController;
    public Transform bowlTransform;
    public GameObject wastePrefab;
    public Transform spawnPoint;
    public int maxInitialWaste = 5;
    public float maxTiltAngle = 90f;
    public float minTiltToSpawn = 10f;
    public float minTiltToDrop = 10f;
    public float rotationSpeed = 5f;
    public int maxWasteToCollect = 10;
    public float outOfBoundsY = -10f;

    private Vector2 dragStartPos;
    private bool isDragging = false;
    private bool isDraggingEnabled = true;

    private List<GameObject> allWaste = new List<GameObject>();
    private int collectedWasteCount = 0;
    private float spawnTimer = 0f;
    private float spawnCooldown = 1f;

    void Update()
    {
        HandleDragInput();

        // Clean up waste that has fallen out of bounds
        for (int i = allWaste.Count - 1; i >= 0; i--)
        {
            if (allWaste[i] == null || allWaste[i].transform.position.y < outOfBoundsY)
            {
                Destroy(allWaste[i]);
                allWaste.RemoveAt(i);
            }
        }

        // Get current tilt angle
        float currentTilt = bowlTransform.rotation.eulerAngles.z;
        if (currentTilt > 180f) currentTilt -= 360f;

        // Handle spawning and gravity activation
        if (Mathf.Abs(currentTilt) >= minTiltToSpawn)
        {
            spawnTimer += Time.deltaTime;

            int activeWasteInBowl = CountWasteInBowl();

            if (spawnTimer >= spawnCooldown &&
                allWaste.Count < maxWasteToCollect &&
                activeWasteInBowl < maxInitialWaste)
            {
                SpawnWaste();
                spawnTimer = 0f;
            }

            // Activate gravity on spawned waste
            if (Mathf.Abs(currentTilt) >= minTiltToDrop)
            {
                foreach (var waste in allWaste)
                {
                    if (waste != null)
                    {
                        Rigidbody2D wasteRb = waste.GetComponent<Rigidbody2D>();
                        if (wasteRb != null && wasteRb.gravityScale == 0f)
                        {
                            wasteRb.gravityScale = 2f;
                        }
                    }
                }
            }
        }
    }

    void HandleDragInput()
    {
        if (!isDraggingEnabled) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 dragDelta = (Vector2)Input.mousePosition - dragStartPos;
            float tilt = GetTiltFromDrag(dragDelta);
            bowlTransform.rotation = Quaternion.Lerp(bowlTransform.rotation, Quaternion.Euler(0f, 0f, tilt), Time.deltaTime * rotationSpeed);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            bowlTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                dragStartPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 dragDelta = touch.position - dragStartPos;
                float tilt = GetTiltFromDrag(dragDelta);
                bowlTransform.rotation = Quaternion.Lerp(bowlTransform.rotation, Quaternion.Euler(0f, 0f, tilt), Time.deltaTime * rotationSpeed);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
                bowlTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
#endif
    }

    float GetTiltFromDrag(Vector2 dragDelta)
    {
        if (dragDelta.x < 0 && dragDelta.y > 0)
        {
            float dragMagnitude = dragDelta.y - dragDelta.x;
            float normalized = Mathf.Clamp01(dragMagnitude / 300f);
            return normalized * maxTiltAngle;
        }
        return 0f;
    }

    void SpawnWaste()
    {
        Vector3 spawnOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0f);
        GameObject waste = Instantiate(wastePrefab, spawnPoint.position + spawnOffset, Quaternion.identity);
        allWaste.Add(waste);

        Rigidbody2D wasteRb = waste.GetComponent<Rigidbody2D>();
        if (wasteRb != null)
        {
            wasteRb.gravityScale = 0f;
        }

        Debug.Log("Spawned waste at " + spawnPoint.position);
    }

    int CountWasteInBowl()
    {
        int count = 0;
        foreach (var waste in allWaste)
        {
            if (waste != null)
            {
                Rigidbody2D rb = waste.GetComponent<Rigidbody2D>();
                if (rb != null && rb.gravityScale == 0f)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void OnWasteCollected(GameObject waste)
    {
        if (allWaste.Contains(waste))
        {
            allWaste.Remove(waste);
        }

        collectedWasteCount++;
        Debug.Log("Waste collected: " + collectedWasteCount);

        game.PlaySFX(game.putToWaterSFX);

        Destroy(waste);

        if (collectedWasteCount >= maxWasteToCollect)
        {
            WinCondition();
        }
    }

    void WinCondition()
    {
        Debug.Log("All waste collected!");
        isDraggingEnabled = false; // Disable drag input after win

        // Destroy remaining uncollected waste
        foreach (var waste in allWaste)
        {
            if (waste != null)
            {
                Destroy(waste);
            }
        }
        allWaste.Clear(); // Clear the list to free memory

        if (gameFlowController.currentStage == GameFlowController2.GameStage.PouringWaste)
        {
            gameFlowController.OnPouringWasteFinished();
        }
    }
}
