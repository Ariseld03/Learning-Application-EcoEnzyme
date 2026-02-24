using UnityEngine;

public class DraggableSponge : MonoBehaviour
{
    public bool IsDragging
    {
        get => isDragging;
        set
        {
            if (isDragging != value)
            {
                isDragging = value;
                if (animator != null)
                {
                    animator.SetBool("isDragging", isDragging);
                }
            }
        }
    }

    private bool isDragging = false;
    private PlateWash currentPlate = null;
    private bool wasTouchingPlate = false;

    private Collider2D spongeCollider;
    private ContactFilter2D contactFilter;
    private Collider2D[] results = new Collider2D[10];

    public Animator animator;
    public GameChapter4 game;

    private void Start()
    {
        spongeCollider = GetComponent<Collider2D>();

        if (spongeCollider == null)
        {
            Debug.LogError("[Sponge] Tidak ada Collider2D ditemukan.");
        }

        if (animator == null)
        {
            Debug.LogWarning("[Sponge] Animator tidak ditemukan, animasi tidak akan dijalankan.");
        }

        // Filter hanya layer Default (tempat piring aktif berada)
        contactFilter = new ContactFilter2D
        {
            useTriggers = false,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Default")
        };
    }

    private void Update()
    {
        if (!IsDragging || spongeCollider == null)
        {
            // Jika spons tidak di-drag, pastikan kita beri tahu plate terakhir kalau masih aktif
            if (wasTouchingPlate && currentPlate != null)
            {
                currentPlate.OnSpongeExit();
                currentPlate = null;
                wasTouchingPlate = false;
            }
            return;
        }

        int hitCount = spongeCollider.OverlapCollider(contactFilter, results);

        bool isTouchingAnyPlate = false;

        for (int i = 0; i < hitCount; i++)
        {
            if (results[i] == null) continue;

            PlateWash plate = results[i].GetComponentInParent<PlateWash>();
            if (plate != null && !plate.IsCleaned())
            {
                plate.RegisterSpongePosition(transform.position.y);
                isTouchingAnyPlate = true;
                Debug.Log($"[Sponge] Overlapping with {plate.name} at Y: {transform.position.y:F2}");

                if (currentPlate != plate)
                {
                    // Spons pindah ke plate baru
                    if (currentPlate != null)
                        currentPlate.OnSpongeExit();

                    currentPlate = plate;
                }

                break; // hanya sentuh satu piring per frame
            }
        }

        // Kalau spons sudah tidak menyentuh piring
        if (!isTouchingAnyPlate && wasTouchingPlate)
        {
            if (currentPlate != null)
            {
                currentPlate.OnSpongeExit();
                currentPlate = null;
            }
        }

        wasTouchingPlate = isTouchingAnyPlate;
    }

}
