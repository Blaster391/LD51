using Scoreboard.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private string m_levelName = "test";

    List<ScoreboardCore.Data.ScoreResult> m_highscores;

    GameStateManager m_stateManager = null;
    ScoreboardComponent m_scoreboardComponent;

    bool m_scoreSubmitting = false;
    bool m_highscoresAcquired = false;

    // Start is called before the first frame update
    void Start()
    {
        m_stateManager = GameHelper.GetManager<GameStateManager>();
        m_scoreboardComponent = GetComponent<Scoreboard.Unity.ScoreboardComponent>();
    }

    public int GetScore()
    {
        float totalScore = (m_stateManager.Score * 100.0f) / m_stateManager.TimeInLevel;
        return Mathf.RoundToInt(totalScore);
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
            return;
        }

        ScoreboardCore.Data.Score score = new ScoreboardCore.Data.Score();
        score.User = playerName;
        score.Level = m_levelName;


        score.ScoreValue = GetScore();
        score.SubmittedDateTime = DateTime.UtcNow;
        score.ExtraData.Add("time", $"{m_stateManager.TimeInLevel:.00}");

        Func<List<ScoreboardCore.Data.ScoreResult>, bool, bool> highscoresCallback = (results, success) =>
        {

            if(success)
            {
                m_highscoresAcquired = true;
                m_highscores = results;
            }

            return true;
        };

        Func<bool, string, bool> callback = (success, result) =>
        {
            m_scoreSubmitting = false;
            m_scoreboardComponent.GetHighscores(highscoresCallback, m_levelName);

            return true;
        };



        m_scoreSubmitting = true;
        m_scoreboardComponent.SubmitResult(score, callback);
    }

}
