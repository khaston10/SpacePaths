using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class StartController : MonoBehaviour
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

    #region Variables - Text/ Images

    public Image[] easyStars;
    public Image[] mediumStars;
    public Image[] hardStars;

    #endregion

    #region Variables Mics

    // These values are given in minutes and will need to be tweaked for a balanced game.
    private float fiveStarTimeLimit = 1;
    private float fourStarTimeLimit = 2;
    private float threeStarTimeLimit = 3;
    private float twoStarTimeLimit = 4;
    private float oneStarTimeLimit = 5;

    public Text easyPuzzlesSolvedText;
    public Text mediumPuzzlesSolvedText;
    public Text hardPuzzlesSolvedText;

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
        SetTextAndImagesOnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
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


    public void SetTextAndImagesOnStart()
    {
        // Set all stars to off.
        for (int i = 0; i< 5; i++)
        {
            easyStars[i].gameObject.SetActive(false);
            mediumStars[i].gameObject.SetActive(false);
            hardStars[i].gameObject.SetActive(false);
        }

        // Set the 5 start rating on easy, medium and hard buttons
        if (averageTimeForEasy > 0)
        {
            if (averageTimeForEasy < fiveStarTimeLimit) SetStars(0, 5);
            else if (averageTimeForEasy < fourStarTimeLimit) SetStars(0, 4);
            else if (averageTimeForEasy < threeStarTimeLimit) SetStars(0, 3);
            else if (averageTimeForEasy < twoStarTimeLimit) SetStars(0, 2);
            else if (averageTimeForEasy < oneStarTimeLimit) SetStars(0, 1);

        }

        if(averageTimeForMed > 0)
        {
            if (averageTimeForMed < fiveStarTimeLimit) SetStars(1, 5);
            else if (averageTimeForMed < fourStarTimeLimit) SetStars(1, 4);
            else if (averageTimeForMed < threeStarTimeLimit) SetStars(1, 3);
            else if (averageTimeForMed < twoStarTimeLimit) SetStars(1, 2);
            else if (averageTimeForMed < oneStarTimeLimit) SetStars(1, 1);
        }

        if(averageTimeForHard > 0)
        {
            if (averageTimeForHard < fiveStarTimeLimit) SetStars(2, 5);
            else if (averageTimeForHard < fourStarTimeLimit) SetStars(2, 4);
            else if (averageTimeForHard < threeStarTimeLimit) SetStars(2, 3);
            else if (averageTimeForHard < twoStarTimeLimit) SetStars(2, 2);
            else if (averageTimeForHard < oneStarTimeLimit) SetStars(2, 1);
        }

        // Set the puzzles solved text.
        easyPuzzlesSolvedText.text = amountOfEasySolved.ToString();
        mediumPuzzlesSolvedText.text = amountOfMediumSolved.ToString();
        hardPuzzlesSolvedText.text = amountOfHardSolved.ToString();

    }

    public void SetStars(int difficulty, int numberOfStars)
    {
        if (difficulty == 0)
        {
            for (int i = 0; i < numberOfStars; i++)
            {
                easyStars[i].gameObject.SetActive(true);
            }
        }

        else if (difficulty == 1)
        {
            for (int i = 0; i < numberOfStars; i++)
            {
                mediumStars[i].gameObject.SetActive(true);
            }
        }

        else if (difficulty == 2)
        {
            for (int i = 0; i < numberOfStars; i++)
            {
                hardStars[i].gameObject.SetActive(true);
            }
        }
    }

    public void ClickStartPuzzle(int difficulty)
    {
        print("Start");
        currentPuzzleDifficulty = difficulty;
        SaveData();
        SceneManager.LoadScene(1);
        
    }

    #region Functions Sound

    public void ToggleSettingsPanel()
    {
        if (SettingsPanel.activeInHierarchy)
        {
            SettingsPanel.SetActive(false);
            MainPanel.SetActive(true);
        }

        else
        {
            SettingsPanel.SetActive(true);
            MainPanel.SetActive(false);
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
