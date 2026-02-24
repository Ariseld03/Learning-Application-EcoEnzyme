using UnityEngine;

public class PlateWash : MonoBehaviour
{
    [Header("Visual States")]
    public GameObject dirtyVisual;   // aktif di awal
    public GameObject foamyVisual;   // nonaktif di awal
    public GameObject cleanVisual;   // nonaktif di awal

    [Header("Coverage Settings")]
    public float coverageThreshold = 2.0f;

    private float minY = float.MaxValue;
    private float maxY = float.MinValue;

    private bool isCleaned = false;
    private bool hasPlayedWashSFX = false;

    private PlateManager manager;

    public void SetManager(PlateManager mgr)
    {
        manager = mgr;
    }

    private void Start()
    {
        Debug.Log($"[{name}] PlateWash initialized. Starting in Dirty state.");
        dirtyVisual.SetActive(true);
        foamyVisual.SetActive(false);
        cleanVisual.SetActive(false);
    }

    public void RegisterSpongePosition(float yPos)
    {
        if (isCleaned)
        {
            Debug.Log($"[{name}] Already cleaned. Ignoring sponge input.");
            return;
        }

        // Mainkan SFX hanya sekali saat pertama kali disentuh spons
        if (!hasPlayedWashSFX && manager != null)
        {
            hasPlayedWashSFX = true;
            manager.gameFlowController.game.PlaySFX(manager.gameFlowController.game.washSFX);
        }

        // Update min dan max posisi Y
        if (yPos < minY) minY = yPos;
        if (yPos > maxY) maxY = yPos;

        float coverage = maxY - minY;

        Debug.Log($"[{name}] Sponge Y: {yPos:F2}, MinY: {minY:F2}, MaxY: {maxY:F2}, Coverage: {coverage:F2}");

        // Cek apakah sudah cukup coverage
        if (coverage >= coverageThreshold)
        {
            Debug.Log($"[{name}] Coverage threshold met! Transitioning to FOAMY state.");
            manager.gameFlowController.game.sfxPlayer.StopSFX();
            SetFoamyState();
        }
    }
    public void OnSpongeExit()
    {
        if (!isCleaned && hasPlayedWashSFX && manager != null)
        {
            Debug.Log($"[{name}] Sponge exited. Stopping wash SFX.");
            manager.gameFlowController.game.sfxPlayer.StopSFX();
            hasPlayedWashSFX = false;
        }
    }


    private void SetFoamyState()
    {
        if (isCleaned) return;

        isCleaned = true;

        dirtyVisual.SetActive(false);
        foamyVisual.SetActive(true);
        cleanVisual.SetActive(false);

        Debug.Log($"[{name}] Visual changed to FOAMY.");

        Invoke(nameof(SetCleanState), 2f);
    }

    private void SetCleanState()
    {
        foamyVisual.SetActive(false);
        cleanVisual.SetActive(true);

        Debug.Log($"[{name}] Visual changed to CLEAN.");

        if (manager != null)
        {
            manager.OnPlateCleaned();
        }
    }

    public bool IsCleaned()
    {
        return isCleaned;
    }
}
