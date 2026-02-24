using UnityEngine;

public class DragControllerSpray : MonoBehaviour
{
    public GameChapter4 game;
    private DraggableSpray lastDragged;
    private Vector3 worldPosition;

    void Update()
    {
        if (DraggableSpray.TotalDestroyedTargets >= 4) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
    }

    void HandleMouseInput()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Distance from camera
        worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            TryInitDrag();
        }

        if (Input.GetMouseButton(0))
        {
            if (lastDragged != null)
            {
                lastDragged.UpdateDragging(worldPosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (lastDragged != null)
            {
                lastDragged.StopDragging();
                game.sfxPlayer.StopSFX();
                lastDragged = null;
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = touch.position;
        touchPos.z = 10f;
        worldPosition = Camera.main.ScreenToWorldPoint(touchPos);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TryInitDrag();
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (lastDragged != null)
                    lastDragged.UpdateDragging(worldPosition);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (lastDragged != null)
                {
                    lastDragged.StopDragging();
                    game.sfxPlayer.StopSFX();
                    lastDragged = null;
                }
                break;
        }
    }

    void TryInitDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null)
        {
            DraggableSpray draggable = hit.collider.GetComponent<DraggableSpray>();
            if (draggable != null)
            {
                lastDragged = draggable;
                lastDragged.ResetTargets();
                lastDragged.StartDragging(worldPosition);
            }
            game.PlaySFX(game.spraySFX);
        }
    }
}
