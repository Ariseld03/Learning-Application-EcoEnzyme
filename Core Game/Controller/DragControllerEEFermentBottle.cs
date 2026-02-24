using System.Collections;
using UnityEngine;

public class DragControllerEEFermentBottle : MonoBehaviour
{
    public DraggableEEFermentBottle LastDragged => lastDragged;
    private bool isDragActive;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    private DraggableEEFermentBottle lastDragged;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        isDragActive = false;
        DragControllerEEFermentBottle[] controllers = FindObjectsOfType<DragControllerEEFermentBottle>();
        if (controllers.Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 10));

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        if (isDragActive && !Input.GetMouseButton(0))
        {
            Drop();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition;
        }
        else
        {
            return;
        }

        UpdateWorldPosition();

        if (isDragActive)
        {
            Drag();
        }
        else
        {
            TryInitDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            if (isDragActive)
                Drop();
            return;
        }

        Touch touch = Input.GetTouch(0);
        screenPosition = touch.position;
        UpdateWorldPosition();

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TryInitDrag();
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isDragActive)
                    Drag();
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isDragActive)
                    Drop();
                break;
        }
    }

    void UpdateWorldPosition()
    {
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = -2f;
    }

    void TryInitDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            DraggableEEFermentBottle draggable = hit.transform.GetComponent<DraggableEEFermentBottle>();
            if (draggable != null)
            {
                lastDragged = draggable;
                InitDrag();
            }
        }
    }

    void InitDrag()
    {
        UpdateDragStatus(true);
        lastDragged.game.PlaySFX(lastDragged.game.pickUpSFX);
    }

    void Drag()
    {
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(worldPosition.x, minX, maxX),
            Mathf.Clamp(worldPosition.y, minY, maxY),
            worldPosition.z
        );

        lastDragged.transform.position = clampedPosition;
    }

    void Drop()
    {
        UpdateDragStatus(false);
    }

    void UpdateDragStatus(bool isDragging)
    {
        if (lastDragged == null) return;

        isDragActive = lastDragged.IsDragging = isDragging;
        lastDragged.gameObject.layer = isDragging ? Layer.Dragging : Layer.Default;
        gameObject.layer = isDragging ? Layer.Dragging : Layer.Default;
    }
}
