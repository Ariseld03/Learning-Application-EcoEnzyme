using UnityEngine;
using System.Collections;

public class PlateManager : MonoBehaviour
{
    public GameObject platePrefab;               
    public Transform spawnPoint;                 
    public Transform cleanPlateRestingPoint;     
    public GameFlowController4 gameFlowController;
    public DraggableSponge spons;

    private int plateCleanedCount = 0;
    private GameObject currentPlate;


    private void Start()
    {
        plateCleanedCount = 0;
        SpawnNextPlate();
    }

    public void OnPlateCleaned()
    {
        plateCleanedCount++;

        Debug.Log($"[PlateManager] Piring ke-{plateCleanedCount} sudah bersih.");

        if (currentPlate != null && cleanPlateRestingPoint != null && plateCleanedCount < 3)
        {
            currentPlate.transform.position = cleanPlateRestingPoint.position;
        }

        if (plateCleanedCount < 3)
        {
            SpawnNextPlate();
        }
        else
        {
            Debug.Log("[PlateManager] Semua piring sudah dibersihkan! Menang!");
            FinishThisMinigame();   
        }
    }
    public void DestroyPlate()
    {
        GameObject[] dirtyPlates = GameObject.FindGameObjectsWithTag("DirtyPlate");

        foreach (GameObject plate in dirtyPlates)
        {
            Destroy(plate);
        }
    }
    public void FinishThisMinigame()
    {
        StartCoroutine(FinishMinigameCoroutine());
    }

    private IEnumerator FinishMinigameCoroutine()
    {
        spons.animator.enabled = false;
        Debug.Log("Next!!");
        yield return new WaitForSeconds(0.5f);
        gameFlowController.WashingComplete();
    }

    private void SpawnNextPlate()
    {
        currentPlate = Instantiate(platePrefab, spawnPoint.position, Quaternion.identity);

        PlateWash plateWash = currentPlate.GetComponent<PlateWash>();
        if (plateWash != null)
        {
            plateWash.SetManager(this);
        }
        else
        {
            Debug.LogError("[PlateManager] Prefab tidak punya PlateWash.");
        }
    }
}
