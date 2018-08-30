using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Publisher : MonoBehaviour
{
    Color colorStart;
    Color colorEnd;
    float duration = 1f;

    void Start()
    {
        colorStart = GetComponent<Image>().color;
        colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0f);
        StartCoroutine(Fade());
        StartCoroutine(LoadLevelAfterDelay(1, 3f));
    }

    IEnumerator LoadLevelAfterDelay(int sceneBuildIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneBuildIndex);
    }

    //set fading and lerp from faded to solid over fadeTime
    private IEnumerator Fade()
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            GetComponent<Image>().color = Color.Lerp(colorEnd, colorStart, t / duration);
            yield return null;
        }
    }
}
