﻿/* 
 Filename: GameManager.cs
 Author: Catt Symonds
 Student Number: 101209214
 Date Last Modified: 17/10/2020
 Description: All base game management
 Revision History: 
 14/10/2020: File created, timer function implemented
 16/10/2020: Created time up and game over function
 16/10/2020: Created AddTime function
 17/10/2020: Debugged timer visual so that if you get extra time it recalculates rather than just looking full
 17/10/2020: Saving/loading of time left
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float totalSeconds;

    [SerializeField]
    private float timeSecondsLeft;

    [SerializeField]
    private float speedModifier = 1.0f;

    private float totalTime;
    private float timeLeft;

    private Animator timerAnim;

    public Image timerFill;
    public TextMeshProUGUI timeAddedLabel;

    public TextMeshProUGUI timeUpLabel;
    public SceneMgmtButtons sceneManagement;
    public PlayerController player;

    private ScoreKeeper scoreKeeper;

    private void Start()
    {
        timerAnim = timerFill.GetComponent<Animator>();
        scoreKeeper = GameObject.Find("ScoreKeeper").GetComponent<ScoreKeeper>();
        totalTime = totalSeconds * 1000;
        timeLeft = timeSecondsLeft * 1000;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft <= 0)
        {
            TimeUp();
        }
        else
        {
            ApplySpeedModifiers();
            UpdateTimer();
        }


    }

    // Speeds up timer if the player is being attacked
    void ApplySpeedModifiers()
    {
        if (player.BeingAttacked)
        {
            speedModifier = 2.5f;
            timerAnim.SetBool("spedUp", true);
        }
        else
        {
            speedModifier = 1.0f;
            timerAnim.SetBool("spedUp", false);
        }
    }

    // Regular update to timer visual
    void UpdateTimer()
    {
        timeLeft -= Time.time * speedModifier;
 

        timerFill.fillAmount = timeLeft / totalTime;
    }

    //  Handles starting game over
    void TimeUp()
    {
        timeUpLabel.gameObject.SetActive(true);
        StartCoroutine(TimeUpCoro());
        player.Kill();
    }

    // Waits before playing gameover so that the player has time to see the timeup message!
    IEnumerator TimeUpCoro()
    {
        yield return new WaitForSeconds(1.5f);
        GameOver();
    }

    // Changes scene
    void GameOver()
    { 
        scoreKeeper.SaveTimeLeft(timeLeft);
        sceneManagement.OnNextButtonPressed();
    }

    // Changes scene to win scene
    public void Win()
    {
        //scoreKeeper.SaveTimeLeft(timeLeft);
        timeUpLabel.GetComponent<TextMeshProUGUI>().SetText("You win!");
        sceneManagement.OnWin();
    }

    // Adds time to timer (when player picks up treats or kills enemy)

    public void AddTime(int secondsToAdd)
    {
        timeLeft += secondsToAdd * 1000;
        //totalTime+= secondsToAdd * 1000;
        if (timeLeft > totalTime)
        {
            totalTime = timeLeft;
        }
        timeAddedLabel.SetText("+" + secondsToAdd.ToString());
       StartCoroutine(TurnOnTimeLabelForSeconds(1.5f));
    }


    // Turns on the label showing time added for the second amount given 
    IEnumerator TurnOnTimeLabelForSeconds(float seconds)
    {
        timeAddedLabel.gameObject.SetActive(true);
        yield return new WaitForSeconds(seconds);
        timeAddedLabel.gameObject.SetActive(false);
    }


}
