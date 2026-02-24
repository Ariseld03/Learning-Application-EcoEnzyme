using UnityEngine;

public class DragControllerWatering : MonoBehaviour
{
    private DraggableWatering lastDragged;
    private Vector3 offset;
    private Vector3 worldPosition;

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag(worldPosition);
        }
        else if (Input.GetMouseButton(0))
        {
            Drag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;
            touchPosition.z = 10f;
            worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);

            if (touch.phase == TouchPhase.Began)
            {
                TryStartDrag(worldPosition);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                Drag();
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndDrag();
            }
        }
    }

    void TryStartDrag(Vector3 inputWorldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(inputWorldPos, Vector2.zero);
        if (hit.collider != null)
        {
            DraggableWatering draggable = hit.collider.GetComponent<DraggableWatering>();
            if (draggable != null)
            {
                lastDragged = draggable;
                lastDragged.ResetTargets();
                InitDrag(inputWorldPos);
            }
        }
    }

    void InitDrag(Vector3 inputWorldPos)
    {
        if (lastDragged != null)
        {
            lastDragged.IsDragging = true;
            offset = lastDragged.transform.position - inputWorldPos;
            lastDragged.gameFlowController.game.PlaySFX(lastDragged.gameFlowController.game.wateringSFX);
        }
    }

    void Drag()
    {
        if (lastDragged != null)
        {
            Vector3 targetPos = worldPosition + offset;
            lastDragged.transform.position = targetPos;
        }
    }

    void EndDrag()
    {
        if (lastDragged != null)
        {
            lastDragged.IsDragging = false;
            lastDragged.gameFlowController.game.sfxPlayer.StopSFX();
            lastDragged.CheckCoverageAndFinish();
            lastDragged = null;
        }
    }
}
