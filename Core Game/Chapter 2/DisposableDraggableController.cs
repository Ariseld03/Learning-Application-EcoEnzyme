using System.Collections.Generic;
using UnityEngine;

public class DisposableDraggableController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint; // Posisi spawn ulang
    private List<GameObject> droppedObjects = new List<GameObject>(); // Menyimpan prefab terakhir
    private Camera cam;
    Vector2 worldPos;

    private void Start()
    {
        cam = Camera.main;
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
        if (Input.GetMouseButtonDown(0)) // Hanya saat mouse diklik baru
        {
            worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapPoint(worldPos);

            if (col != null && col.CompareTag("SpawnDisposable"))
            {
                if (droppedObjects.Count > 0)
                {
                    Debug.Log("Spawn");
                    SpawnLastDraggable();
                }
            }
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) // Hanya saat touch pertama dimulai
            {
                worldPos = cam.ScreenToWorldPoint(touch.position);
                Collider2D col = Physics2D.OverlapPoint(worldPos);

                if (col != null && col.CompareTag("SpawnDisposable"))
                {
                    if (droppedObjects.Count > 0)
                    {
                        SpawnLastDraggable();
                    }
                }
            }
        }
    }

    public void AddToDroppedList(GameObject draggableObj)
    {
        droppedObjects.Add(draggableObj);
    }

    private void SpawnLastDraggable()
    {
        DraggableWeigh[] existingDraggables = FindObjectsOfType<DraggableWeigh>();

        foreach (DraggableWeigh d in existingDraggables)
        {
            if (d.hasScored && d.isRespawnedFromDrop) 
            { 
                Debug.Log("Masih ada draggable aktif di layar. Tidak spawn ulang.");
                return;
            }
        }

        if (droppedObjects.Count == 0)
        {
            Debug.LogWarning("Tidak ada draggable di list untuk dispawn.");
            return;
        }

        GameObject lastObj = droppedObjects[droppedObjects.Count - 1];
        droppedObjects.RemoveAt(droppedObjects.Count - 1);

        GameObject newObj = Instantiate(lastObj, spawnPoint.position, Quaternion.identity);
        DraggableWeigh draggable = newObj.GetComponent<DraggableWeigh>();

        draggable.isRespawnedFromDrop = true;

        draggable.IsDragging = false;
        newObj.transform.localScale = Vector3.one;

        Debug.Log("Respawned draggable from drop list at: " + spawnPoint.position);
    }

}
