using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableSprayPlant : MonoBehaviour
{
    [SerializeField] public GameFlowController4 gameFlowController;
    public bool IsDragging;

    public Animator idle;
    public Animator spray;
    private Collider2D col;

    private float totalCoverage = 0f;
    private GameObject currentTarget = null;
    private bool targetSprayed = false;

    public bool isGameFinished = false;

    private float coveragePerTarget = 0.2f;
    [SerializeField] private SpriteRenderer liquidSpriteRenderer;

    private const int requiredCoverageCount = 2;

    void Start()
    {
        col = GetComponent<Collider2D>();
        
        spray.applyRootMotion = false;
        spray.gameObject.SetActive(false);

        idle.SetBool("isDragging", false);

        if (liquidSpriteRenderer != null)
        {
            liquidSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    void Update()
    {
        if (isGameFinished) return;

        if (spray != null)
        {
            spray.gameObject.SetActive(IsDragging);
            if (IsDragging)
            {
                idle.SetBool("isDragging", true);
                if (idle.enabled)
                    idle.enabled = false;
            }
            else if (IsDragging == false)
            {
                idle.SetBool("isDragging", false);
                if (!idle.enabled)
                    idle.enabled = true;
            }
        }
    }

    public void ResetTargets()
    {
        currentTarget = null;
        targetSprayed = false;
    }

    public void SetCurrentTarget(GameObject target)
    {
        if (currentTarget != target)
        {
            currentTarget = target;
            targetSprayed = false;
            Debug.Log($"[Trigger] Target baru diset: {currentTarget.name}");
        }
    }

    public void CheckCoverageAndHandleProgress()
    {
        if (currentTarget == null)
        {
            Debug.Log("[Coverage] Tidak ada target saat ini.");
            return;
        }

        if (!targetSprayed)
        {
            targetSprayed = true;
            totalCoverage += coveragePerTarget;
            Debug.Log($"[Coverage] Target {currentTarget.name} disemprot. Total coverage: {totalCoverage}");
        }

        if (totalCoverage >= requiredCoverageCount)
        {
            Debug.Log("[Coverage] Semua target sudah disiram!");
            StartCoroutine(GoToNextMinigame());
        }
        else
        {
            float opacity = Mathf.Clamp01(totalCoverage / requiredCoverageCount);
            if (liquidSpriteRenderer != null)
            {
                liquidSpriteRenderer.color = new Color(1f, 1f, 1f, opacity);
                Debug.Log($"[Visual] Opacity sprite liquid: {opacity}");
            }
        }
    }

    IEnumerator GoToNextMinigame()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Lanjut ke minigame berikutnya...");
        isGameFinished = true;

        if (spray != null)
        {
            spray.enabled = false;
        }

        gameFlowController.CompleteGame();
    }
}
