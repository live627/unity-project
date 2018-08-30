using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider progressBar;
    public Text loadingText;
    public int scene = 1;
    private int frame = 0;
            
    private IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            loadingText.text = String.Format("Loading...({0} %)", progress * 100);

            Debug.Log(String.Format("Loading... ({0} %)", operation.progress));

            if (operation.progress == 0.9f)
            {
               // operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void Start ()
    {
        loadingText.text = "Loading...";
    }

    void Update()
    {
        if (frame == 0)
        {
        StartCoroutine(LoadScene());
        }
        frame++;
        // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time/2.5f, 1));

        }
}
