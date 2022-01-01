using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public GameObject PuzzleSolvedPanel;
    public Slider musicSlider;
    public Slider SFXSlider;

    #endregion

    #region Variables Vertices

    public bool pathStarted = false;
    private bool puzzleIsSolvable = false;
    public Button[] vertices;
    public Text[] degreeText;
    int amountOfVertices = 0;
    int amountOfColors = 0;
    List<int> verticiesAvailable = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    List<string> colorsAvailable = new List<string> { "cyan", "red", "blue", "magenta" };
    List<int> verticiesInUse = new List<int>();
    List<string> colorsInUse = new List<string>();
    List<string> colorsToBeUsed = new List<string>();
    List<int> verticesStartingSize = new List<int>();
    private float pathContinuetimer = .5f;

    // This will hold the current path.
    private List<int> currentPath = new List<int>();

    // THe line drawer is used to draw lines between two points.
    private List<Transform> pathPoints = new List<Transform>();
    public GameObject LineDrawer;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        SetVolumeSlidersAndLevelsOnStart();

        while (!puzzleIsSolvable)
        {


            // Reset Lists FOr New Puzzle.
            verticiesInUse.Clear();
            verticiesInUse  = new List<int>();
            colorsInUse.Clear();
            colorsInUse = new List<string>();
            colorsToBeUsed.Clear();
            colorsToBeUsed = new List<string>();
            verticesStartingSize.Clear();
            verticesStartingSize = new List<int>();
            colorsAvailable.Clear();
            colorsAvailable = new List<string> { "cyan", "red", "blue", "magenta" };
            verticiesAvailable.Clear();
            verticiesAvailable = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            SetNewPuzzle();
            puzzleIsSolvable = CheckPuzzleForSolvability();

            if (!puzzleIsSolvable) print("Puzzle Rejected");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs(); 
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        mins = Mathf.Floor(timer / 60);
        secs = timer % 60;
        timeText.text = mins.ToString() + " : " + Mathf.RoundToInt(secs).ToString();

        // When the path has been started we need to limit how quickly the user can add to the path.
        if (pathStarted && pathContinuetimer > 0) pathContinuetimer -= Time.deltaTime;
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
        PlayAudioClip(1);

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


    public void PlayAudioClip(int clip)
    {
        /*
        0 - Mouse click Good 1
        1 - Mouse click Good 2
        2 - Mouse click Bad 1
        3 - Mouse click Electric 1
        4 - Mouse click Electric 2
        5 - Celebrate
        */

        GameObject.Find("Sfx Source").GetComponent<SoundEffectsController>().PlayClip(clip);

    }

    #endregion


    #region Functions Vertices

    public void SetNewPuzzle()
    {
        // For easy puzzles we will have a min of 3 vertices, and max of 6.
        // All colors will be used.

        // For med puzzles we will have a min of 5 vertices, and max of 8.
        // 3 colors will be used.

        // For hard puzzles we will have a min of 7 vertices, and max of 12.
        // 2 colors will be used.

        if (currentPuzzleDifficulty == 0)
        {
            amountOfVertices = Random.Range(3, 5);
            amountOfColors = 4;
            

        }
            
        else if (currentPuzzleDifficulty == 1)
        {
            amountOfVertices = Random.Range(5, 8);
            amountOfColors = 3;

        }
            
        else if (currentPuzzleDifficulty == 2)
        {
            amountOfVertices = Random.Range(7, 12);
            amountOfColors = 2;
        } 
        
        for (int i= 0; i < amountOfVertices; i++)
        {
            int indexOfNextVertex = Random.Range(0, verticiesAvailable.Count);
            verticiesInUse.Add(verticiesAvailable[indexOfNextVertex]);
            verticiesAvailable.RemoveAt(indexOfNextVertex);
        }

        for (int i = 0; i < amountOfColors; i++)
        {
            int indexOfNextColor = Random.Range(0, colorsAvailable.Count);
            colorsToBeUsed.Add(colorsAvailable[indexOfNextColor]);
            colorsAvailable.RemoveAt(indexOfNextColor);
        }

        // Hide all the vertices that will not be used.
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            vertices[verticiesInUse[i]].gameObject.SetActive(true);
        }

        // Set Size and Color.
        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            var indexOfColor = Random.Range(0, colorsToBeUsed.Count);
            colorsInUse.Add(colorsToBeUsed[indexOfColor]); // Save this information so that the vertex color can be restored if the puzzle needs to be reset.
            var size = Random.Range(1, verticiesInUse.Count);
            ChangeVertexColor(verticiesInUse[i], colorsToBeUsed[indexOfColor]);
            ChangeVertexSize(verticiesInUse[i], size);
            verticesStartingSize.Add(size);
        }

    }

    public bool CheckPuzzleForSolvability()
    {
        // Returns true if the puzzle can be solved.

        // There can only be at most two vertices with an odd degree.
        int numberOfOddVerticies = 0;
        print("There are " + verticiesInUse.Count + " vertices in use.");
        for (int indexOfVertex = 0; indexOfVertex < verticiesInUse.Count; indexOfVertex++)
        {
            
            var temp = GetVertexSize(verticiesInUse[indexOfVertex]);
            print("The size of vertex: " + indexOfVertex + " is: " + temp.ToString());
            if (temp==1 || temp == 3 || temp == 5)
            {
                
                numberOfOddVerticies += 1;
            }
        }

        if (numberOfOddVerticies > 2) return false;

        return true;
    }

    public void StartPath(int vertexNum)
    {
        PlayAudioClip(4);

        if (!pathStarted && vertices[vertexNum].GetComponent<RectTransform>().rect.height > 30)
        {
            pathStarted = true;

            // Add point the the current path.
            currentPath.Add(vertexNum);
            pathPoints.Add(vertices[vertexNum].transform);
        }
    }

    public void EndPath()
    {
        PlayAudioClip(2);

        // Check to see if the path has been competed correctly.
        if (IsPuzzleComplete())
        {
            // Save data.
            if (currentPuzzleDifficulty == 0)
            {
                amountOfEasySolved += 1;
                // Need to cal average.
            }

            else if (currentPuzzleDifficulty == 1)
            {
                amountOfMediumSolved += 1;
                // Need to cal average.
            }

            else if (currentPuzzleDifficulty == 2)
            {
                amountOfHardSolved += 1;
                // Need to cal average.
            }

            SaveData();

            PlayAudioClip(5);

            // Give Player Feedback.
            TogglePuzzleSolvedPanel();
        }

        // If it has not we need to reset the puzzle.
        for (int i = 0; i < verticiesInUse.Count; i++)
        {
            ChangeVertexSize(verticiesInUse[i], verticesStartingSize[i]);
            ChangeVertexColor(verticiesInUse[i], colorsInUse[i]);
        }

        pathStarted = false;

        // Clear the current Path and Path Points.
        currentPath.Clear();
        pathPoints.Clear();

        //Draw a line between these points.
        UpdateThePathRenderer();
    }

    public bool IsPuzzleComplete()
    {
        // Returns true if the puzzle is complete.
        // Check to see if all vertices are now size zero.
        for (int i = 0; i <  verticiesInUse.Count; i++)
        {
            if (GetVertexSize(verticiesInUse[i]) > 0)
            {
                return false;
            }
        }

        return true;
    }

    public void ContinuePath(int vertexNum)
    {
        if (pathStarted && vertices[vertexNum].GetComponent<RectTransform>().rect.height > 30 && pathContinuetimer <= 0)
        {
            // For the time being we will not allow a path from a vertex to the same vertex.
            if (currentPath[currentPath.Count - 1] != vertexNum)
            {

                // We also do not want to add an edge if it already exists.
                if (!CheckIfEdgeExists(currentPath[currentPath.Count-1], vertexNum))
                {
                    // Change the size of the next vertex and the one the edge is leaving..
                    ChangeVertexSize(vertexNum, Mathf.RoundToInt(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 40) / 10);
                    ChangeVertexSize(currentPath[currentPath.Count-1], Mathf.RoundToInt(vertices[currentPath[currentPath.Count-1]].GetComponent<RectTransform>().rect.height - 40) / 10);

                    // Change the color to grey if the vertex is size 0.
                    if (vertices[vertexNum].GetComponent<RectTransform>().rect.height == 30)
                    {
                        ChangeVertexColor(vertexNum, "grey");
                    }

                    // Reset path continue timer so the path can not grow too fast.
                    pathContinuetimer = .5f;

                    // Add point the the current path.
                    currentPath.Add(vertexNum);
                    pathPoints.Add(vertices[vertexNum].transform);

                    //Draw a line between these points.
                    UpdateThePathRenderer();

                    PlayAudioClip(0);
                }
                
            }
        }

    }

    public void ChangeVertexColor(int vertexNum, string color)
    {
        // Valid colors - 'green', 'red', 'blue', 'white', 'grey', 'magenta', 'cyan' 

        if (color == "green") vertices[vertexNum].GetComponent<Image>().color = Color.green;
        else if (color == "red") vertices[vertexNum].GetComponent<Image>().color = Color.red;
        else if (color == "blue") vertices[vertexNum].GetComponent<Image>().color = Color.blue;
        else if (color == "cyan") vertices[vertexNum].GetComponent<Image>().color = Color.cyan;
        else if (color == "magenta") vertices[vertexNum].GetComponent<Image>().color = Color.magenta;
        else if (color == "white") vertices[vertexNum].GetComponent<Image>().color = Color.white;
        else if (color == "grey") vertices[vertexNum].GetComponent<Image>().color = Color.grey;
        else print("Invalid Color Selected");        

    }

    public void ChangeVertexSize(int vertexNum, int size)
    {
        // A vertex with size 0 should set to (30, 30).
        // A vertex with size 5, which is considered the max, should be set at (80, 80).

        vertices[vertexNum].GetComponent<RectTransform>().sizeDelta = new Vector2((10*size) + 30, (10 * size) + 30);

        // Update the degree text.
        UpdateDegreeText(vertexNum, size);
    }

    public void UpdateDegreeText(int vertexNum, int size)
    {
        degreeText[vertexNum].text = size.ToString();
    }

    public void UpdateThePathRenderer()
    {
        LineDrawer.GetComponent<LineDrawer>().SetUpLine(pathPoints);
    }

    public bool CheckIfEdgeExists(int vertexStart, int VertextEnd)
    {
        // Returns true if the edge already exists.
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            if (currentPath[i] == vertexStart && currentPath[i + 1] == VertextEnd) return true;
            if (currentPath[i] == VertextEnd && currentPath[i + 1] == vertexStart) return true;
        }

        return false;
    }

    public void TogglePuzzleSolvedPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        PuzzleSolvedPanel.SetActive(true);
    }

    public void ClickNextPuzzle()
    {
        PlayAudioClip(1);

        SceneManager.LoadScene(1);
    }

    public void ClickMainMenu()
    {
        PlayAudioClip(2);

        SceneManager.LoadScene(0);
    }

    public int GetVertexSize(int vertexNum)
    {
        //print((float)(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 30f) / (float)10);
        return Mathf.RoundToInt((float)(vertices[vertexNum].GetComponent<RectTransform>().rect.height - 30f) / (float)10);
        
    }
        

    #endregion


    #region Functions Control Detection

    public void GetInputs()
    {
        if (Input.GetMouseButtonUp(0) && pathStarted)
        {
            EndPath();
        }
    }

    #endregion

    #endregion
}
