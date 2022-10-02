﻿using Scoreboard.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private string m_levelName = "test";

    GameStateManager m_stateManager = null;
    ScoreboardComponent m_scoreboardComponent;

    bool m_scoreSubmitting = false;
    bool m_highscoresAcquired = false;
    private int m_score = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_stateManager = GameHelper.GetManager<GameStateManager>();
        m_scoreboardComponent = GetComponent<Scoreboard.Unity.ScoreboardComponent>();
    }

    public int GetScore()
    {
        if(m_score > 0)
        {
            return m_score;
        }

        float totalScore = (m_stateManager.Score * 100.0f) / m_stateManager.TimeInLevel;
        m_score = Mathf.RoundToInt(totalScore);
        return m_score;
    }

    public void TrySubmitScore()
    {
        if(m_scoreSubmitting)
        {
            return;
        }

        string playerName = PlayerPrefs.GetString("name");
        if(string.IsNullOrEmpty(playerName))
        {
            LoadScoreboard();
            return;
        }

        ScoreboardCore.Data.Score score = new ScoreboardCore.Data.Score();
        score.User = playerName;
        score.Level = m_levelName;


        score.ScoreValue = GetScore();
        score.SubmittedDateTime = DateTime.UtcNow;
        score.ExtraData.Add("time", $"{m_stateManager.TimeInLevel:.00}");



        Func<bool, string, bool> callback = (success, result) =>
        {
            m_scoreSubmitting = false;
            LoadScoreboard();
            

            return true;
        };



        m_scoreSubmitting = true;
        m_scoreboardComponent.SubmitResult(score, callback);
    }

    public void LoadScoreboard()
    {
        Func<List<ScoreboardCore.Data.ScoreResult>, bool, bool> highscoresCallback = (results, success) =>
        {

            if (success)
            {
                m_highscoresAcquired = true;

                GameHelper.GetManager<UIManager>().ShowScoreboard(results);
            }

            return true;
        };

        m_scoreboardComponent.GetHighscores(highscoresCallback, m_levelName);

    }

}
