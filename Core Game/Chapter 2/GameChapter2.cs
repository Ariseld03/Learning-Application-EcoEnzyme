using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameChapter2 : BaseGame
{
    public AudioClip cutSFX;
    public AudioClip pickUpSFX;
    public AudioClip dropDownSFX;
    public AudioClip pourSFX;
    public AudioClip putToWaterSFX;
    public AudioClip stirringSFX;

    [Header("Panels")]
    public GameObject wrongsmallpanel1;
    public GameObject wrongsmallpanel2;
    public GameObject wrongsmallpanel3;
    [SerializeField] public TextMeshProUGUI missionText;

    public GameFlowController2 flowController;

    [Header("Game Logic")]
    public int countWrongAnswer = 0;
    private Mission currentMission;

    [Header("List")]
    private Mission[] missions;
    private List<Mission> uncompletedMissions;
    private List<Mission> completedMissions;

    private void Awake()
    {
        score = 100;
        if (uncompletedMissions == null)
            uncompletedMissions = new List<Mission>();

        if (completedMissions == null)
            completedMissions = new List<Mission>();
    }

    private void Start()
    {
        base.DeactivateAllPopUp();
        sfxPlayer = FindObjectOfType<SFXPlayer>();
        missionText.gameObject.SetActive(true);
        AddMissions();

        if (completedMissions == null)
            completedMissions = new List<Mission>();

        if (uncompletedMissions == null || uncompletedMissions.Count == 0)
            uncompletedMissions = missions.ToList();

        LoadNextMission();
    }

    private void AddMissions()
    {
        missions = new Mission[9];
        missions[0] = new Mission { Text = "Sampah Organik 900g", Data = flowController.weightWasteObject };
        missions[1] = new Mission { Text = "Gula Merah 300g", Data = flowController.weightSugarObject };
        missions[2] = new Mission { Text = "Memotong Sampah Organik", Data = flowController.cuttingWasteObject };
        missions[3] = new Mission { Text = "Memotong Gula Merah", Data = flowController.cuttingSugarObject };
        missions[4] = new Mission { Text = "Air 3000mL", Data = flowController.fillingWaterObject };
        missions[5] = new Mission { Text = "Menuang Sampah", Data = flowController.pouringWasteObject };
        missions[6] = new Mission { Text = "Menuang Gula", Data = flowController.pouringSugarObject };
        missions[7] = new Mission { Text = "Mengaduk Bahan-bahan di Wadah", Data = flowController.stirringObject };
        missions[8] = new Mission { Text = "Menutup Wadah", Data = flowController.closingObject };
    }

    public void LoadNextMission()
    {
        DeactivateAllMissions();
        if (currentMission != null && currentMission.Data != null)
        {
            currentMission.Data?.SetActive(false);
            completedMissions.Add(currentMission);
        }

        if (uncompletedMissions.Count > 0)
        {
            currentMission = uncompletedMissions[0];
            uncompletedMissions.RemoveAt(0);

            currentMission.Data?.SetActive(true);
            missionText.text = currentMission.Text;
        }
        else
        {
            currentMission = null;
            missionText.text = "";
            base.Win();
        }
    }
    private void DeactivateAllMissions()
    {
        if (missions == null) return;

        foreach (var mission in missions)
        {
            if (mission != null && mission.Data != null)
                mission.Data.SetActive(false);
        }
    }

    public override void ShowCorrectPopUp()
    {
        StartCoroutine(base.ShowAndHideCorrectPopUp());
    }

    public override void ShowWrongPopUp()
    {
        StartCoroutine(base.ShowAndHideWrongPopUp());
        StartCoroutine(AddWrongPanel());
        score -= 10;
    }

    public void ShowWrongNoPanelPopUp()
    {
        StartCoroutine(ShowAndHideWrongPopUpOnly());
        score -= 10;
    }
    private IEnumerator ShowAndHideWrongPopUpOnly()
    {
        StartCoroutine(base.ShowAndHideWrongPopUp());
        countWrongAnswer++;

        if (countWrongAnswer > 3)
        {
            yield return new WaitForSeconds(0.2f);
            GameOver();
        }
    }

    IEnumerator AddWrongPanel()
    {
        countWrongAnswer++;

        if (countWrongAnswer == 1)
        {
            wrongsmallpanel1.SetActive(true);
        }
        else if (countWrongAnswer == 2)
        {
            wrongsmallpanel2.SetActive(true);
        }
        else if (countWrongAnswer == 3)
        {
            wrongsmallpanel3.SetActive(true);
        }

        if (countWrongAnswer > 3)
        {
            yield return new WaitForSeconds(0.2f);
            GameOver();
        }

    }
    public void DeactiveWrongPanel()
    {
        wrongsmallpanel1.SetActive(false);
        wrongsmallpanel2.SetActive(false);
        wrongsmallpanel3.SetActive(false);
    }

    public void Next()
    {
        base.Next(2);
    }

    public override void GameOver()
    {
        base.GameOver();

        if (currentMission != null)
            currentMission.Data?.SetActive(false);
    }
}
