using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    public float startTime = 20f;
    public bool survival;
    public float timer = 20;
    public float totalTime;
    public float totalFish;
    public bool gameOver;
    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

        GameObject.DontDestroyOnLoad(gameObject);
    }


    public void StartTimer() 
    {
        totalTime = 20;
        totalFish = 0;
        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        timer = Mathf.Clamp(timer, 0, float.MaxValue);
        gameOver = true;
        Debug.LogError("GAME OVER!");
    }

    public void SetSurvival(bool isSurvival)
    {
        gameOver = false;
        survival = isSurvival;
        timer = startTime; 
    }

    public void AddToTime(float time)
    {
        timer += time;
        totalTime += time;
        totalFish += 1;

        Debug.Log("AYE");
    }
}
