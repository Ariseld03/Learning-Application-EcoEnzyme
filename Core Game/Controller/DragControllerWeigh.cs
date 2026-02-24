using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DragControllerWeigh : MonoBehaviour
{
    public DraggableWeigh LastDragged => lastDragged;
    public TMP_Text scoreText;

    public bool dragEnabled = true;

    private bool isDragActive;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    public DraggableWeigh lastDragged;

    public float totalScore = 0f;
    [SerializeField] public float targetScore;

    [SerializeField] private LayerMask draggableLayerMask;

    private float idleTimer = 0f;
    private bool isUserInteracting = false;

    [SerializeField] private float idleThreshold = 2f;
    public List<Animator> draggableAnimators = new List<Animator>();
    private int currentIdleIndex = 0;

    private void Start()
    {
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!dragEnabled) return; 

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#endif
        HandleIdleTimer();
    }

    private Coroutine idleCoroutine;

    IEnumerator ResetIdleFlag(Animator anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.SetBool("isIdle", false);
    }

    void HandleIdleTimer()
    {
        if (isUserInteracting)
        {
            idleTimer = 0f;
            isUserInteracting = false;
            if (idleCoroutine != null)
            {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold)
            {
                if (idleCoroutine == null)
                    idleCoroutine = StartCoroutine(PlayIdleAnimationsSequentially());

                idleTimer = 0f;
            }
        }
    }

    IEnumerator PlayIdleAnimationsSequentially()
    {
        if (draggableAnimators == null || draggableAnimators.Count == 0)
        {
            idleCoroutine = null;
            yield break;
        }

        float delayBetweenAnimations = 1f;

        for (int i = 0; i < draggableAnimators.Count; i++)
        {
            if (draggableAnimators[currentIdleIndex] != null)
            {
                draggableAnimators[currentIdleIndex].SetBool("isIdle", false);
            }

            currentIdleIndex = (currentIdleIndex + 1) % draggableAnimators.Count;

            Animator currentAnim = draggableAnimators[currentIdleIndex];
            currentAnim.SetBool("isIdle", true);

            StartCoroutine(ResetIdleFlag(currentAnim, delayBetweenAnimations));
            yield return new WaitForSeconds(delayBetweenAnimations);
        }

        idleCoroutine = null;
    }

    public void AddDraggableAnimator(Animator anim)
    {
        if (anim != null && !draggableAnimators.Contains(anim))
        {
            draggableAnimators.Add(anim);
        }
    }

    public void RemoveDraggableAnimator(Animator anim)
    {
        if (anim != null && draggableAnimators.Contains(anim))
        {
            draggableAnimators.Remove(anim);
        }
    }

    void HandleMouseInput()
    {
        if (!dragEnabled) return;

        if (isDragActive)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Drop();
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            screenPosition = Input.mousePosition;
            isUserInteracting = true;
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
            TryInitDrag();
        }
    }

    void HandleTouchInput()
    {
        if (!dragEnabled) return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        screenPosition = touch.position;
        isUserInteracting = true;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                TryInitDrag();
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isDragActive && lastDragged != null)
                    Drag();
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                Drop();
                break;
        }
    }

    void TryInitDrag()
    {
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, draggableLayerMask);
        if (hit.collider != null)
        {
            Debug.Log("Raycast hit: " + hit.collider.name + ", Layer: " + hit.collider.gameObject.layer);
            DraggableWeigh draggable = hit.transform.GetComponent<DraggableWeigh>();
            if (draggable != null)
            {
                lastDragged = draggable;
                InitDrag();
            }
        }
    }

    void InitDrag()
    {
        if (lastDragged == null) return;

        lastDragged.LastPosition = lastDragged.transform.position;
        UpdateDragStatus(true);

        if (lastDragged.game != null && lastDragged.game.sfxPlayer != null)
        {
            lastDragged.game.PlaySFX(lastDragged.game.pickUpSFX);
        }
    }

    void Drag()
    {
        if (lastDragged != null)
            lastDragged.transform.position = worldPosition;
        
        if (lastDragged.isRespawnedFromDrop)
        {
            // Trigger animasi lerp lalu hancurkan
            lastDragged.StartLerpBackThenDestroy();

            // Reset agar tidak bisa lanjut didrag
            lastDragged = null;
            //isDragActive = false;
            return;
        }
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
        UpdateDragStatus(false);
    }

    void UpdateDragStatus(bool isDragging)
    {
        if (lastDragged == null) return;
        isDragActive = lastDragged.IsDragging = isDragging;
    }
}
