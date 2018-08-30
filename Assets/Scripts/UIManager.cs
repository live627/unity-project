using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject[] pauseObjects;
    private GameObject[] finishObjects;
    public Canvas helpCanvas;
    public Canvas menuCanvas;
    public Canvas optionsCanvas;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;

        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");            //gets all objects with tag ShowOnPause
        finishObjects = GameObject.FindGameObjectsWithTag("ShowOnFinish");          //gets all objects with tag ShowOnFinish

        HidePaused();
        HideFinished();
    }

    // Update is called once per frame
    void Update()
    {
        //uses the p button to pause and unpause the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseControl();
        }
    }

    //Reloads the Level
    public void Reload()
    {
        LoadLevel(2);
    }

    //Reloads the Level
    public void MainMenu()
    {
        menuCanvas.enabled = true;
        helpCanvas.enabled = false;
        optionsCanvas.enabled = false;
    }

    //Reloads the Level
    public void Help()
    {
        menuCanvas.enabled = false;
        helpCanvas.enabled = true;
    }

    //Reloads the Level
    public void Options()
    {
        GameObject muzzleFlashPrefab = Resources.Load("Prefabs/Options") as GameObject;
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, optionsCanvas.transform);
        Dropdown[] children = muzzleFlash.GetComponentsInChildren<Dropdown>();
        foreach (Dropdown child in children)
        {
            print("Foreach loop: " + child.name);
            switch (child.name)
            {
                case "Dropdown":
                    FindRes(child);
                    break;
                default:
                    break;
            }
        }

        menuCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }

    private static void FindRes(Dropdown dropdownMenu)
    {
        Resolution[] allRes = Screen.resolutions;
        Resolution currentRes = Screen.currentResolution;
        dropdownMenu.ClearOptions();
       // dropdownMenu.onValueChanged.AddListener(delegate { Screen.SetResolution(allRes[dropdownMenu.value].width, allRes[dropdownMenu.value].height, false); });
        for (int i = 0; i < allRes.Length; i++)
        {
            dropdownMenu.options.Add(new Dropdown.OptionData(allRes[i].ToString()));

            if (allRes[i].Equals(currentRes))
            {
                dropdownMenu.value = i;
            }
        }
    }

    private string ResToString(Resolution res)
    {
        return res.width + " x " + res.height;
    }

    /// <summary>
    /// Method to quit the game. Call methods such as auto saving before qutting here.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    //controls the pausing of the scene
    public void PauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            ShowPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            HidePaused();
        }
    }

    //shows objects with ShowOnPause tag
    public void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void HidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    //shows objects with ShowOnFinish tag
    public void ShowFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnFinish tag
    public void HideFinished()
    {
        foreach (GameObject g in finishObjects)
        {
            g.SetActive(false);
        }
    }

    //loads inputted level
    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
