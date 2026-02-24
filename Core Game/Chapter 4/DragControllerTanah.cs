using UnityEngine;
using System.Collections;
using TMPro;

public class DragControllerTanah : MonoBehaviour
{
    public DraggableTanah LastDragged => lastDragged;
    [SerializeField] GameFlowController4 gameFlowController;

    private bool isDragActive;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    public DraggableTanah lastDragged;

    public float totalScore = 0f;
    [SerializeField] public float targetScore;

    [SerializeField] private LayerMask draggableLayerMask;

    private void Update()
    {
        if (isDragActive)
        {
            if (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                Drop();
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
        {
            screenPosition = Input.GetTouch(0).position;
        }
        else
        {
            return;
        }

        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        if (isDragActive && lastDragged != null)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, draggableLayerMask);
            if (hit.collider != null)
            {
                Debug.Log("Raycast hit: " + hit.collider.name + ", Layer: " + hit.collider.gameObject.layer);
                DraggableTanah draggable = hit.transform.GetComponent<DraggableTanah>();
                if (draggable != null)
                {
                    lastDragged = draggable;
                    InitDrag();
                }
            }
        }
    }

    void InitDrag()
    {
        if (lastDragged == null) return;
        lastDragged.LastPosition = lastDragged.transform.position;
        UpdateDragStatus(true);
        gameFlowController.game.PlaySFX(gameFlowController.game.pickUpSFX);
    }

    void Drag()
    {
        if (lastDragged != null)
            lastDragged.transform.position = worldPosition;
    }

    void Drop()
    {
        if (lastDragged != null && lastDragged.IsDragging)
        {
            Collider2D col = lastDragged.GetComponent<Collider2D>();
            if (!col.IsTouchingLayers(LayerMask.GetMask("ValidDrop")))
            {
                lastDragged.ResetPositionSmooth();
            }
        }
        gameFlowController.game.PlaySFX(gameFlowController.game.putDownSFX);
        UpdateDragStatus(false);
    }

    void UpdateDragStatus(bool isDragging)
    {
        if (lastDragged == null) return;
        isDragActive = lastDragged.IsDragging = isDragging;
    }

    public void FinishThisMinigame()
    {
        StartCoroutine(FinishMinigameCoroutine());
    }

    private IEnumerator FinishMinigameCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Next!!");
        gameFlowController.FertilizeComplete();
    }

}
