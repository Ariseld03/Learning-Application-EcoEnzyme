using UnityEngine;

public class DragControllerSprayPlant : MonoBehaviour
{
    private DraggableSprayPlant lastDragged;
    private Vector3 offset;
    private Vector3 worldPosition;
    private Vector3 screenPosition;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = 10f;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (Input.GetMouseButtonDown(0))
        {
            TryInitDrag();
        }

        if (Input.GetMouseButton(0))
        {
            Drag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            if (lastDragged != null && lastDragged.IsDragging)
                StopDrag();
            return;
        }

        Touch touch = Input.GetTouch(0);
        screenPosition = touch.position;
        screenPosition.z = 10f;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TryInitDrag();
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                Drag();
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                StopDrag();
                break;
        }
    }

    void TryInitDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            DraggableSprayPlant draggable = hit.collider.GetComponent<DraggableSprayPlant>();
            if (draggable != null && !draggable.isGameFinished)
            {
                lastDragged = draggable;
                lastDragged.ResetTargets();
                InitDrag();
            }
        }
    }

    void InitDrag()
    {
        if (lastDragged != null && !lastDragged.isGameFinished)
        {
            lastDragged.IsDragging = true;
            offset = lastDragged.transform.position - worldPosition;
            lastDragged.gameFlowController.game.PlaySFX(lastDragged.gameFlowController.game.spraySFX);
        }
        else
        {
            lastDragged = null;
        }
    }

    void Drag()
    {
        if (lastDragged != null && lastDragged.IsDragging && !lastDragged.isGameFinished)
        {
            Vector3 targetPos = worldPosition + offset;
            lastDragged.transform.position = targetPos;
        }
    }

    void StopDrag()
    {
        if (lastDragged != null)
        {
            lastDragged.IsDragging = false;
            lastDragged.gameFlowController.game.sfxPlayer.StopSFX();

            if (!lastDragged.isGameFinished)
            {
                lastDragged.CheckCoverageAndHandleProgress();
            }

            lastDragged = null;
        }
    }
}
