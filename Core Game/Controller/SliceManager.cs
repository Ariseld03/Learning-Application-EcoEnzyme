using System.Collections.Generic;
using UnityEngine;

public class SliceManager : MonoBehaviour
{
    [SerializeField] private List<HitCollider> allIrisan;
    private int currentIndex = 0;
    [SerializeField] GameFlowController2 gameFlowController;

    private void Start()
    {
        ActivateCurrentIrisan();
    }

    public void NextIrisan()
    {
        allIrisan[currentIndex].gameObject.SetActive(false);
        currentIndex++;

        if (currentIndex < allIrisan.Count)
        {
            ActivateCurrentIrisan();
        }
        else if(gameFlowController.currentStage == GameFlowController2.GameStage.CuttingWaste && currentIndex >= allIrisan.Count)
        {
            Debug.Log("Semua irisan sayur selesai!");
            gameFlowController.OnCuttingWasteFinished();
        }
        else if (gameFlowController.currentStage == GameFlowController2.GameStage.CuttingSugar && currentIndex >= allIrisan.Count)
        {
            Debug.Log("Semua irisan gula selesai!");
            gameFlowController.OnCuttingSugarFinished();
        }
    }

    private void ActivateCurrentIrisan()
    {
        allIrisan[currentIndex].gameObject.SetActive(true);
        allIrisan[currentIndex].SetManager(this);
    }
}