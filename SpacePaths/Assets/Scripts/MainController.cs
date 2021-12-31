using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainController : MonoBehaviour
{

    #region Global Variables
    public int currentPuzzleDifficulty;

    public int amountOfEasySolved;
    public int amountOfMediumSolved;
    public int amountOfHardSolved;

    public float averageTimeForEasy;
    public float averageTimeForMed;
    public float averageTimeForHard;

    public float musicVolume;
    public float sfxVolume;

    #endregion

    #region Variables - Text and Images

    public Text timeText;
    private float timer;
    private float mins;
    private float secs;

    #endregion

    #region Variables Sounds

    public AudioMixer masterMixer;
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public Slider musicSlider;
    public Slider SFXSlider;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        SetVolumeSlidersAndLevelsOnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        mins = Mathf.Floor(timer / 60);
        secs = timer % 60;
        timeText.text = mins.ToString() + " : " + Mathf.RoundToInt(secs).ToString();
    }

    #region Functions

    #region Save/Load

    public void SaveData()
    {
        GlobalController.Instance.amountOfEasySolved = amountOfEasySolved;
        GlobalController.Instance.amountOfMediumSolved = amountOfMediumSolved;
        GlobalController.Instance.amountOfHardSolved = amountOfHardSolved;
        GlobalController.Instance.averageTimeForEasy = averageTimeForEasy;
        GlobalController.Instance.averageTimeForMed = averageTimeForMed;
        GlobalController.Instance.averageTimeForHard = averageTimeForHard;
        GlobalController.Instance.musicVolume = musicVolume;
        GlobalController.Instance.sfxVolume = sfxVolume;
        GlobalController.Instance.currentPuzzleDifficulty = currentPuzzleDifficulty;
    }

    public void LoadData()
    {
        amountOfEasySolved = GlobalController.Instance.amountOfEasySolved;
        amountOfMediumSolved = GlobalController.Instance.amountOfMediumSolved;
        amountOfHardSolved = GlobalController.Instance.amountOfHardSolved;
        averageTimeForEasy = GlobalController.Instance.averageTimeForEasy;
        averageTimeForMed = GlobalController.Instance.averageTimeForMed;
        averageTimeForHard = GlobalController.Instance.averageTimeForHard;
        musicVolume = GlobalController.Instance.musicVolume;
        sfxVolume = GlobalController.Instance.sfxVolume;
        currentPuzzleDifficulty = GlobalController.Instance.currentPuzzleDifficulty;
    }

    #endregion

    #region Functions Sound

    public void ToggleSettingsPanel()
    {
        if (SettingsPanel.activeInHierarchy)
        {
            SettingsPanel.SetActive(false);
        }

        else
        {
            SettingsPanel.SetActive(true);
        }
    }

    public void SetVolumeSlidersAndLevelsOnStart()
    {
        masterMixer.SetFloat("MusicVol", musicVolume);
        masterMixer.SetFloat("SFXVol", sfxVolume);
        musicSlider.value = musicVolume;
        SFXSlider.value = sfxVolume;

    }

    public void AdjustMusicVol(float vol)
    {
        masterMixer.SetFloat("MusicVol", vol);
        musicVolume = vol;
    }

    public void AdjustSFXVol(float vol)
    {
        masterMixer.SetFloat("SFXVol", vol);
        sfxVolume = vol;
    }

    #endregion

    #endregion
}
