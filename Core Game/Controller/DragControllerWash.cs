using UnityEngine;

public class DragControllerWash : MonoBehaviour
{
    public GameChapter4 game;
    public DraggableSponge LastDragged => lastSpongeDragged;

    private bool isDragActive;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    private DraggableSponge lastSpongeDragged;

    private Camera cam;
    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        cam = Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;

        // Pastikan hanya satu DragController aktif
        DragControllerWash[] controllers = FindObjectsOfType<DragControllerWash>();
        if (controllers.Length > 1)
        {
            Destroy(gameObject);
        }
    }

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
        else return;

        worldPosition = cam.ScreenToWorldPoint(screenPosition);
        worldPosition.z = -6f;

        // Batasi posisi agar tidak keluar layar
        worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
        worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);

        if (isDragActive)
        {
            Drag();
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
            if (hit.collider != null)
            {
                DraggableSponge sponge = hit.transform.GetComponent<DraggableSponge>();
                if (sponge != null)
                {
                    lastSpongeDragged = sponge;
                    InitDrag();
                    return;
                }
            }
        }
    }

    void InitDrag()
    {
        UpdateDragStatus(true);
        game.PlaySFX(game.pickUpSFX);
    }

    void Drag()
    {
        if (lastSpongeDragged != null)
        {
            lastSpongeDragged.transform.position = worldPosition;
        }
    }

    void Drop()
    {
        UpdateDragStatus(false);
        game.PlaySFX(game.putDownSFX);
    }

    void UpdateDragStatus(bool isDragging)
    {
        isDragActive = isDragging;

        if (lastSpongeDragged != null)
        {
            lastSpongeDragged.IsDragging = isDragging;
            lastSpongeDragged.gameObject.layer = isDragging ? LayerMask.NameToLayer("Dragging") : LayerMask.NameToLayer("Default");
        }

        gameObject.layer = isDragging ? LayerMask.NameToLayer("Dragging") : LayerMask.NameToLayer("Default");
    }
}
