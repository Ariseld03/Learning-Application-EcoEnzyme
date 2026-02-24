using UnityEngine;
using System.Collections;

public class FermentationController : MonoBehaviour
{
    public GameFlowController3 gameFlowController;
    public GameObject firstDraggableObject;
    public Transform firstDropTarget;
    private bool firstMissionCompleted = false;

    public GameObject secondDraggableObject;
    public Transform secondDropTarget;

    public GameObject thirdDraggableObject; 
    public Transform thirdDropTarget; 

    public GameObject contaminationMinigame; 

    public int currentMission = 0;

    public void EnableFermentationMinigame()
    {
        StartCoroutine(DelayedEnableFunction());
    }

    IEnumerator DelayedEnableFunction()
    {
        UpdateCurrentMission();
        DisableFunction();
        yield return new WaitForSeconds(3.5f);
        EnableFunction();
    }

    private void UpdateCurrentMission()
    {
        if (currentMission == 0)
        {
            currentMission++;
            StartFirstMission();
        }
        else if (currentMission == 1)
        {
            currentMission++;
            StartSecondMission();
        }
        else if (currentMission == 2)
        {
            currentMission++;
            StartThirdMission();
        }
    }

    public void DisableFunction()
    {
        MonoBehaviour[] components = this.gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (var comp in components)
        {
            comp.enabled = false;
        }
    }
    public void EnableFunction()
    {
        MonoBehaviour[] components = this.gameObject.GetComponentsInChildren<MonoBehaviour>();
        foreach (var comp in components)
        {
            comp.enabled = true;
        }
    }

    public void StartFirstMission()
    {
        firstDraggableObject.SetActive(true);
        secondDraggableObject.SetActive(false);
        thirdDraggableObject.SetActive(false);
    }

    public void OnFirstMissionComplete()
    {
        DisableFunction();
        firstMissionCompleted = true;
        gameFlowController.OnFirstMissionComplete();
    }

    public bool IsFirstMissionComplete()
    {
        return firstMissionCompleted;
    }

    public void StartSecondMission()
    {
        firstDraggableObject.SetActive(false);
        firstDropTarget.gameObject.SetActive(false);
        secondDropTarget.gameObject.SetActive(true);
        secondDraggableObject.SetActive(true);
    }

    public void OnSecondMissionComplete()
    {
        DisableFunction();
        gameFlowController.OnSecondMissionComplete();
        Debug.Log("Misi kedua selesai");
    }

    public void StartThirdMission()
    {
        secondDropTarget.gameObject.SetActive(false);
        secondDraggableObject.SetActive(false);
        Debug.Log("Misi kedua selesai, lanjut ke misi ketiga.");
        thirdDraggableObject.SetActive(true); 
        thirdDropTarget.gameObject.SetActive(true);
    }

    public void OnThirdMissionComplete()
    {
        Debug.Log("Semua ketiga misi drag selesai!");
        DisableFunction();
        gameFlowController.OnThirdMissionComplete();
    }
}
