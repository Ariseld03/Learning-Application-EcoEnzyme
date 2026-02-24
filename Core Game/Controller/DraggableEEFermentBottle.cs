using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableEEFermentBottle : MonoBehaviour
{
    public GameChapter3 game;

    public bool IsDragging;
    public Vector3 LastPosition;

    public FermentationController fermentasi;
    private Collider2D col;
    private DragControllerEEFermentBottle dragController;

    private Animator idle;

    // --- Tambahan Variabel untuk Lerp ---
    [Header("Lerp Settings")]
    public float lerpDuration = 0.3f; 
    private bool isLerping = false; 
    private Vector3 startLerpPos;
    private Vector3 targetLerpPos;
    private Vector3 startLerpScale;
    private Vector3 targetLerpScale;
    private float lerpStartTime;
    // --- Akhir Tambahan Variabel ---

    void Start()
    {
        col = GetComponent<Collider2D>();
        dragController = FindObjectOfType<DragControllerEEFermentBottle>();
        idle = this.GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (IsDragging)
        {
            idle.SetBool("isIdle", false);
            if (idle.enabled)
                idle.enabled = false;
            // Jika objek sedang di-drag, hentikan lerping jika masih berjalan
            if (isLerping)
            {
                StopCoroutine("AnimateToTarget");
                isLerping = false;
            }
            return;
        }
        else if (!IsDragging && !isLerping) // Hanya ubah ke idle jika tidak di-drag dan tidak sedang lerping
        {
            // Debug.Log("idle"); // Nonaktifkan jika terlalu banyak log
            idle.SetBool("isIdle", true);
            if (!idle.enabled)
                idle.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Logika untuk mengatur posisi agar tidak tumpang tindih (jika masih diperlukan)
        DraggableEEFermentBottle colliderDraggable = other.GetComponent<DraggableEEFermentBottle>();
        if (colliderDraggable != null && dragController.LastDragged != null && dragController.LastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = other.Distance(col);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            // Debug.Log("tran bf = " + transform.position);
            // Debug.Log($"Distance: {colliderDistance2D.distance}, Normal: {colliderDistance2D.normal}");
            transform.position -= diff;
            // Debug.Log("tran = " + transform.position);
        }

        // --- Logika untuk ValidDrop dengan Lerp ---
        if (other.CompareTag("ValidDrop") && !IsDragging && !isLerping) // Pastikan tidak sedang di-drag atau lerping
        {
            // Dapatkan collider dari objek ValidDrop
            Collider2D validDropCollider = other.GetComponent<Collider2D>();

            if (validDropCollider != null)
            {
                // Set posisi dan skala target untuk lerp
                startLerpPos = transform.position;
                targetLerpPos = validDropCollider.transform.position; // Posisi ValidDrop

                startLerpScale = transform.localScale;
                // Hitung skala target agar ukuran objek ini sama dengan collider ValidDrop
                // Asumsi collider ValidDrop adalah BoxCollider2D atau memiliki bounds yang valid
                Vector2 dropBoundsSize = validDropCollider.bounds.size;
                Vector2 thisBoundsSize = col.bounds.size; // Ukuran collider objek ini

                // Menghitung faktor skala yang dibutuhkan
                float scaleX = dropBoundsSize.x / thisBoundsSize.x * transform.localScale.x;
                float scaleY = dropBoundsSize.y / thisBoundsSize.y * transform.localScale.y;
                targetLerpScale = new Vector3(scaleX, scaleY, transform.localScale.z); // Pertahankan z

                // Mulai Coroutine lerp
                StartCoroutine(AnimateToTarget(targetLerpPos, targetLerpScale));

                // Logika gameplay setelah drop berhasil
                game.PlaySFX(game.putDownSFX);
                game.ShowAndHideCorrectPopUp();

                // Matikan script ini dan animator idle setelah proses selesai
                idle.enabled = false;
                this.enabled = false;

                // Memicu misi (jika masih diperlukan setelah lerp)
                // Sebaiknya ini dipanggil di akhir Coroutine AnimateToTarget
                // atau setelah dipastikan objek sudah berada di tempatnya
                if (fermentasi.currentMission == 1)
                {
                    StartCoroutine(DelayedMissionComplete(() => fermentasi.OnFirstMissionComplete(), lerpDuration + 0.1f)); // Tambah delay lerp
                }
                else if (fermentasi.currentMission == 2)
                {
                    StartCoroutine(DelayedMissionComplete(() => fermentasi.OnSecondMissionComplete(), lerpDuration + 0.1f));
                }
                else if (fermentasi.currentMission == 3)
                {
                    StartCoroutine(DelayedMissionComplete(() => fermentasi.OnThirdMissionComplete(), lerpDuration + 0.1f));
                }
            }
        }
        // --- Akhir Logika ValidDrop dengan Lerp ---
    }

    // Coroutine untuk animasi Lerp posisi dan skala
    IEnumerator AnimateToTarget(Vector3 targetPosition, Vector3 targetScale)
    {
        isLerping = true;
        lerpStartTime = Time.time;
        float elapsed = 0f;

        // Simpan posisi dan skala awal saat lerp dimulai
        startLerpPos = transform.position;
        startLerpScale = transform.localScale;

        while (elapsed < lerpDuration)
        {
            elapsed = Time.time - lerpStartTime;
            float t = elapsed / lerpDuration;

            // Lerp posisi
            transform.position = Vector3.Lerp(startLerpPos, targetPosition, t);
            // Lerp skala
            transform.localScale = Vector3.Lerp(startLerpScale, targetScale, t);

            yield return null; // Tunggu satu frame
        }

        // Pastikan posisi dan skala tepat di akhir animasi
        transform.position = targetPosition;
        transform.localScale = targetScale;
        isLerping = false; // Lerping selesai
    }


    IEnumerator DelayedMissionComplete(System.Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}