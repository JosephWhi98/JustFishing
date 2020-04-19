using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.gameObject.SetActive(true);
    }

    public void Start()
    {
        StartCoroutine(FadeToClear(5f));
    }

    private void Update()
    { 
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameStateManager.instance.survival)
                Survival();
            else
                Peaceful();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscapeToMenu();
        }

    }

    public void Peaceful()
    {
        StartCoroutine(SceneTransition(false, "SampleScene"));
    }

    public void Survival()
    {
        StartCoroutine(SceneTransition(true, "SampleScene"));
    }

    public void EscapeToMenu()
    {
        StartCoroutine(SceneTransition(true, "MenuScene"));
    }

    IEnumerator FadeToClear(float time)
    {
        float t = 0;

        while (canvasGroup.alpha > 0.01f)
        {
            t += Time.deltaTime / time;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, t);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    IEnumerator FadeToBlack(float time)
    {
        float t = 0;

        while (canvasGroup.alpha < 0.99f)
        {
            t += Time.deltaTime / time;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, t);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    IEnumerator SceneTransition(bool isSurvival, string sceneName)
    {
        GameStateManager.instance.SetSurvival(isSurvival);
        yield return FadeToBlack(2f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Quit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
