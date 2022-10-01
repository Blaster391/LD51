using Newtonsoft.Json;
using ScoreboardCore.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

namespace Scoreboard
{
    namespace Unity
    {
        public class ScoreboardConnection
        {
            [SerializeField]
            public string DatabaseAddress { get; set; }

            [SerializeField]
            public string GameName { get; set; }

            [SerializeField]
            public string GameKey { get; set; }
        }

        public class ScoreboardComponent : MonoBehaviour
        {
            [SerializeField]
            private string m_scoreboardConnectionFile = "";

            private ScoreboardConnection m_connection = null;

            private void Awake()
            {
                TextAsset connectionText = Resources.Load<TextAsset>(m_scoreboardConnectionFile);

                if(connectionText == null)
                {
                    return;
                }

                m_connection = JsonConvert.DeserializeObject<ScoreboardConnection>(connectionText.text);

                // For testing
                //Score score = new Score();
                //score.Level = "Lap";
                //score.User = "TEST_ID_3";
                //score.ScoreValue = 999;
                //score.ExtraData.Add("Username", "Shane");


                //Func<bool, string, bool> callback = (success, result) =>
                //{
                //    if (success)
                //    {
                //        Debug.Log("SUCCESS");
                //    }
                //    else
                //    {
                //        Debug.Log("FAIL");
                //    }

                //    return true;
                //};

                //SubmitResult(score, callback);
            }

            public void GetHighscores(Func<List<ScoreboardCore.Data.ScoreResult>, bool, bool> _onRequestComplete, string level, bool _highscoresOnly = true, int _resultsSize = 10)
            {
                if(m_connection == null)
                {
                    _onRequestComplete(null, false);
                    return;
                }

                StartCoroutine(GetHighscoresCoroutine(_onRequestComplete, level, _highscoresOnly, _resultsSize));
            }

            public void GetTotalForLevel(Func<int, bool, bool> _onRequestComplete, string level)
            {
                if (m_connection == null)
                {
                    _onRequestComplete(-1, false);
                    return;
                }

                StartCoroutine(GetTotalForLevelCoroutine(_onRequestComplete, level));
            }

            private IEnumerator GetTotalForLevelCoroutine(Func<int, bool, bool> _onRequestComplete, string level)
            {

                string getUrl = "/api/scoreboard/" + m_connection.GameName + "/total?level=" + level;
                var request = UnityWebRequest.Get(m_connection.DatabaseAddress + getUrl);
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);

                    _onRequestComplete(-1, false);
                }
                else
                {
                    Debug.Log("Get request complete!");

                    string resultsString = request.downloadHandler.text;
                    int resultValue = -1;
                    if(int.TryParse(resultsString, out resultValue))
                    {
                        _onRequestComplete(resultValue, false);
                    }
                    else
                    {
                        _onRequestComplete(-1, false);
                    }

                   
                }
            }

            private IEnumerator GetHighscoresCoroutine(Func<List<ScoreboardCore.Data.ScoreResult>, bool, bool> _onRequestComplete, string level, bool _highscoresOnly = true, int _resultsSize = 10)
            {

                string getUrl = "/api/scoreboard/" + m_connection.GameName + "?level=" + level;
                getUrl += $"&amount={_resultsSize}";
                if (_highscoresOnly)
                {
                    getUrl += "&highscoreOnly=true";
                }

                var request = UnityWebRequest.Get(m_connection.DatabaseAddress + getUrl);
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);

                    _onRequestComplete(null, false);
                }
                else
                {
                    Debug.Log("Get request complete!");

                    string resultsString = request.downloadHandler.text;
                    List<ScoreboardCore.Data.ScoreResult> results = JsonConvert.DeserializeObject<List<ScoreboardCore.Data.ScoreResult>>(resultsString);

                    _onRequestComplete(results, true);
                }
            }

            public void SubmitResult(ScoreboardCore.Data.Score score, Func<bool, string, bool> _onRequestComplete)
            {
                if (m_connection == null)
                {
                    _onRequestComplete(false, "NO CONNECTION FILE");
                    return;
                }

                StartCoroutine(SubmitResultCoroutine(score, _onRequestComplete));
            }

            private IEnumerator SubmitResultCoroutine(ScoreboardCore.Data.Score score, Func<bool, string, bool> _onRequestComplete)
            {

                string submissionUrl = "/api/scoreboard/" + m_connection.GameName + "/submit";

                ScoreSubmissionRequest scoreSubmission = ScoreboardCore.Encrypt.ScoreEncrypt.CreateRequestForScore(score, m_connection.GameKey);
                string scoreText = JsonConvert.SerializeObject(scoreSubmission);
                Debug.Log(scoreText);
                var request = UnityWebRequest.Put(m_connection.DatabaseAddress + submissionUrl, scoreText);
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("content-Type", "application/json");
                request.SetRequestHeader("accept", "text/plain");
  
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);
                    _onRequestComplete(false, request.error);
                }
                else
                {
                    Debug.Log("Post request complete!");

                    string resultsString = request.downloadHandler.text;
                    Debug.Log(resultsString);
                    _onRequestComplete(true, resultsString);
                }
            }
        }
    }
}
