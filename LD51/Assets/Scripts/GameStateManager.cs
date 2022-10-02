using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public int TimeRemaining 
    { 
        get
        {
            return Mathf.CeilToInt(m_timeRemaining);
        } 
    }
    public int Score
    {
        get; private set;
    } = 0;

    public float TimeInLevel
    {
        get
        {
            return m_timeInLevel;
        }
    }

    private float m_timeRemaining = 10.0f;
    private float m_timeInLevel = 0.0f;

    public bool GameOver { get; private set; } = false;
    public bool GameWon { get; private set; } = false;

    public bool GameActive
    {
        get
        {
            return !GameOver && !GameWon;
        }
    }

    [SerializeField]
    private bool m_isCredits = false;

    public GameObject Player { get; private set; }

    private List<GameObject> m_enemies = new List<GameObject>();

    public CharacterController PlayerController 
    { 
        get
        {
            return Player.GetComponentInChildren<CharacterController>();
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        Player = player;
    }

    public void RegisterEnemy(GameObject enemy)
    {
        m_enemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        m_enemies.Remove(enemy);
    }

    public void AddScore(int score)
    {
        Score += score;
    }

    public void ResetTimer()
    {
        m_timeRemaining = 10.0f;
    }

    public void LoseGame()
    {
        if (m_isCredits)
        {
            return;
        }

        GameOver = true;
        GameHelper.GetManager<ScoreManager>().LoadScoreboard();
    }

    public void WinGame()
    {
        GameWon = true;

        int levelIndex = (SceneManager.GetActiveScene().buildIndex);
        if (!PlayerPrefs.HasKey("levelCompleted") || levelIndex > PlayerPrefs.GetInt("levelCompleted"))
        {
            PlayerPrefs.SetInt("levelCompleted", levelIndex);
        }

        GameHelper.GetManager<ScoreManager>().TrySubmitScore();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if(m_isCredits)
        {
            return;
        }

        if(GameOver || GameWon)
        {
            return;
        }

        if(m_enemies.Count == 0)
        {
            WinGame();
        }

        m_timeRemaining -= Time.deltaTime;
        m_timeInLevel += Time.deltaTime;

        if (m_timeRemaining < 0.0f)
        {
            LoseGame();
            Player.GetComponent<CharacterHealth>().DestroyHead();
        }
    }
}
