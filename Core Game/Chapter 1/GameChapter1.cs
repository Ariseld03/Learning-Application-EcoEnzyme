using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameChapter1 : BaseGame
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI missionText;

    [Header("Panel Choose Wrong Answer")]
    public GameObject wrongsmallpanel1;
    public GameObject wrongsmallpanel2;
    public GameObject wrongsmallpanel3;

    [Header("Minigame Correct Answer")]
    public GameObject firstMission;
    public GameObject secondMission;
    public GameObject thirdMission;
    public GameObject FourthMission;
    public GameObject fifthMission;

    [Header("Game Logic")]
    private int countWrongAnswer = 0;
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
        AddMissions();

        if (completedMissions == null)
            completedMissions = new List<Mission>();

        if (uncompletedMissions == null || uncompletedMissions.Count == 0)
            uncompletedMissions = missions.ToList();

        LoadNextMission();
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
   
    private void AddMissions() 
    {
        missions = new Mission[5];
        missions[0] = new Mission { Text = "Misi :\nPilih kulit buah yang sehat (segar dan bersih)", Data = firstMission };
        missions[1] = new Mission { Text = "Misi :\nPilih sayur yang sehat (segar dan bersih)", Data = secondMission };
        missions[2] = new Mission { Text = "Misi :\nPilih jenis gula yang alami (tidak berzat kimia)", Data = thirdMission };
        missions[3] = new Mission { Text = "Misi :\nPilih jenis buah tidak berkulit keras dan tidak berlemak", Data = FourthMission };
        missions[4] = new Mission { Text = "Misi :\nPilih jenis wadah berbahan lentur dan bermulut lebar", Data = fifthMission };
    }

    public void LoadNextMission()
    {
        DeactivateAllMissions();
        if (currentMission != null && currentMission.Data !=null)
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
            currentMission.Data?.SetActive(false);
            base.Win();
        }
    }

    public override void ShowCorrectPopUp()
    {
        StartCoroutine(base.ShowAndHideCorrectPopUp());
        LoadNextMission();
    }

    public override void ShowWrongPopUp()
    {
        StartCoroutine(base.ShowAndHideWrongPopUp());
        StartCoroutine(AddWrongAnswer());
        score -= 20;
    }

    
    public void ResetMissions()
    {
        completedMissions = new List<Mission>();
        uncompletedMissions = new List<Mission>();

        countWrongAnswer = 0;

        foreach (var mission in missions)
        {
            mission.Data.SetActive(false);
        }

        uncompletedMissions = missions.ToList();

        LoadNextMission();
    }
     IEnumerator AddWrongAnswer()
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
        base.Next(1);
    }

    public override void GameOver()
    {
        base.GameOver();

        if (currentMission != null)
            currentMission.Data?.SetActive(false);
    }
}
