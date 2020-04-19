using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text text;
    bool gameOver;


    [Header("Game Over Menu")]
    public TMP_Text totalFish;
    public TMP_Text totalTime;
    public RectTransform gameOverOverlay;
    public Vector3 TargetPosition;
    public MainMenu mainMenu;

    private void Awake()
    {
        gameOverOverlay.gameObject.SetActive(false);
        if (GameStateManager.instance != null)
        {
            if (!GameStateManager.instance.survival)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        GameStateManager.instance.StartTimer();

        //gameOverOverlay.gameObject.SetActive(false);
    }

    private void Update()
    {


        if (gameOver || GameStateManager.instance.timer == 0)
        {
            if (!gameOver)
            {
                StartCoroutine(OpenGameOverScreen());
            }

            gameOver = true;
            text.text = "0:00s";
        }



        if (!gameOver)
        {
            string mins = Mathf.Floor(GameStateManager.instance.timer / 60).ToString("0");
            string seconds = (GameStateManager.instance.timer % 60).ToString("00");

            text.text = mins + "." + seconds + "s";
        }

    }


    IEnumerator OpenGameOverScreen()
    {
        yield return null;
        totalFish.text = GameStateManager.instance.totalFish.ToString();

        string mins = Mathf.Floor(GameStateManager.instance.totalTime / 60).ToString("0");
        string seconds = (GameStateManager.instance.totalTime % 60).ToString("00");

        totalTime.text = mins + "." + seconds + "s";


        float t = 0;

        gameOverOverlay.gameObject.SetActive(true);

        while (gameOverOverlay.transform.localPosition != TargetPosition)
        {
            t += Time.deltaTime / 5f;

            gameOverOverlay.transform.localPosition = Vector3.Lerp(gameOverOverlay.transform.localPosition, TargetPosition, t);

            yield return null;

        }

    }
}
