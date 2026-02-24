using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    private List<float> yPositions = new List<float>();
    private bool isDraggingOverThis = false;
    private Collider2D col;
    private SliceManager manager;

    [SerializeField] private SlicableGulaMerah slicable;
    [SerializeField] private GameObject sliced;
    [SerializeField] private GameObject helper;
    [HideInInspector] private float minSwipeYCoverage;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        minSwipeYCoverage = 0.01f;
    }

    public void SetManager(SliceManager mgr)
    {
        manager = mgr;
        this.helper.SetActive(true);
    }

    private void Update()
    {
        HandleSliceInput();
    }

    private void HandleSliceInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (col.OverlapPoint(pos))
            {
                isDraggingOverThis = true;
                yPositions.Clear();
            }
        }

        if (isDraggingOverThis && Input.GetMouseButton(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (col.OverlapPoint(pos))
            {
                yPositions.Add(pos.y);
            }
        }

        if (isDraggingOverThis && Input.GetMouseButtonUp(0))
        {
            FinalizeDrag();
        }
#else
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));

        Debug.Log($"[TOUCH] Phase: {touch.phase}, Position: {touchPos}, Overlap: {col.OverlapPoint(touchPos)}");

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (col.OverlapPoint(touchPos))
                {
                    isDraggingOverThis = true;
                    yPositions.Clear();
                    Debug.Log("[TOUCH] Touch Began on collider");
                }
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isDraggingOverThis && col.OverlapPoint(touchPos))
                {
                    yPositions.Add(touchPos.y);
                    Debug.Log("[TOUCH] Touch dragging on collider at Y: " + touchPos.y);
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isDraggingOverThis)
                {
                    Debug.Log("[TOUCH] Touch Ended - Finalizing drag");
                    FinalizeDrag();
                }
                break;
        }
    }
#endif
    }


    private void FinalizeDrag()
    {
        isDraggingOverThis = false;
        float minY = col.bounds.min.y;
        float maxY = col.bounds.max.y;
        float range = maxY - minY;

        float covered = 0f;
        HashSet<int> uniqueSteps = new HashSet<int>();

        foreach (var y in yPositions)
        {
            float normalized = (y - minY) / range;
            int step = Mathf.FloorToInt(normalized * 100);
            uniqueSteps.Add(step);
        }

        covered = uniqueSteps.Count / 100f;
        Debug.Log("Covered: " + covered);

        if (covered >= minSwipeYCoverage)
        {
            if (slicable.getSlicedObject() == null || slicable.getSlicedObject() != null)
            {
                slicable.setSlicedObject(sliced);
                slicable.Slice();
            }

            if (manager != null)
            {
                helper.SetActive(false);
                manager.NextIrisan();
            }
        }

        yPositions.Clear();
    }
}
