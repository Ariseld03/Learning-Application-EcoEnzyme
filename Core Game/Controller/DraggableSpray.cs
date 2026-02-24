using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableSpray : MonoBehaviour
{
    [SerializeField] private GameFlowController4 gameFlowController;
    public bool IsDragging = false;

    public Animator animator;
    private Collider2D col;

    private List<float> yPositions = new List<float>();
    private HashSet<GameObject> currentTargets = new HashSet<GameObject>();

    [SerializeField] private float minSwipeYCoverage = 0.01f;

    private static int totalDestroyedTargets = 0;
    private static int targetGoal = 4;

    public static int TotalDestroyedTargets => totalDestroyedTargets;

    private bool ready = false;

    private float minX, maxX, minY, maxY;
    public Vector3 dragOffset { get; private set; }

    void Awake()
    {
        totalDestroyedTargets = 0;
        animator.applyRootMotion = false;
        animator.SetBool("isSpraying", false);
    }

    IEnumerator Start()
    {
        col = GetComponent<Collider2D>();
        yield return null;
        animator.SetBool("isSpraying", false);
        ready = true;

        Camera cam = Camera.main;
        float vertExtent = cam.orthographicSize;
        float horzExtent = vertExtent * cam.aspect;

        float halfWidth = col.bounds.extents.x;
        float halfHeight = col.bounds.extents.y;

        minX = cam.transform.position.x - horzExtent + halfWidth - 1f;
        maxX = cam.transform.position.x + horzExtent - halfWidth + 1f;
        minY = cam.transform.position.y - vertExtent + halfHeight - 2f;
        maxY = cam.transform.position.y + vertExtent - halfHeight + 1f;
    }

    public void UpdateDragging(Vector3 position)
    {
        if (!ready || totalDestroyedTargets >= targetGoal) return;

        if (!IsDragging)
        {
            animator.SetBool("isSpraying", false);
            return;
        }

        if (!animator.GetBool("isSpraying"))
            animator.SetBool("isSpraying", true);

        yPositions.Add(transform.position.y);

        Vector3 targetPos = position + dragOffset;
        targetPos = ClampToScreenBounds(targetPos);
        transform.position = targetPos;
    }

    public Vector3 ClampToScreenBounds(Vector3 targetPos)
    {
        return new Vector3(
            Mathf.Clamp(targetPos.x, minX, maxX),
            Mathf.Clamp(targetPos.y, minY, maxY),
            targetPos.z
        );
    }

    public void StartDragging(Vector3 inputWorldPos)
    {
        IsDragging = true;
        dragOffset = transform.position - inputWorldPos;
        yPositions.Clear();
        currentTargets.Clear();
    }

    public void StopDragging()
    {
        IsDragging = false;
        animator.SetBool("isSpraying", false);
        CheckCoverageAndDestroyTarget();
    }

    public void ResetTargets()
    {
        currentTargets.Clear();
    }

    public void AddTarget(GameObject target)
    {
        if (!currentTargets.Contains(target))
        {
            currentTargets.Add(target);
        }
    }

    public void CheckCoverageAndDestroyTarget()
    {
        if (currentTargets.Count == 0 || totalDestroyedTargets >= targetGoal) return;

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
            foreach (GameObject target in currentTargets)
            {
                if (target != null)
                {
                    Destroy(target);
                    totalDestroyedTargets++;
                    Debug.Log("Target destroyed! Total: " + totalDestroyedTargets);
                }
            }
            currentTargets.Clear();

            if (totalDestroyedTargets >= targetGoal)
            {
                Debug.Log("Semua target disemprot!");
                StartCoroutine(GoToNextMinigame());
            }
        }

        yPositions.Clear();
    }

    IEnumerator GoToNextMinigame()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Lanjut ke minigame berikutnya...");
        gameFlowController.DisinfectComplete();
    }
}
