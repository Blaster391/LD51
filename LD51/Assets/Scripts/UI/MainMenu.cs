using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_nameEntryPanel = null;
    [SerializeField]
    private GameObject m_levelSelectPanel = null;


    [SerializeField]
    TMP_InputField m_playerName = null;

    [SerializeField]
    TMP_Text m_levelSelect = null;

    [SerializeField]
    private List<string> m_levels = new List<string>();

    bool m_nameEntry = true;
    int m_selectedLevelIndex = 0;
    int m_highestCompletedLevel = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("levelCompleted"))
        {
            m_highestCompletedLevel = PlayerPrefs.GetInt("levelCompleted");
        }
        if (PlayerPrefs.HasKey("name"))
        {
            m_playerName.text = PlayerPrefs.GetString("name");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_nameEntry)
        {
            m_nameEntryPanel.SetActive(true);
            m_levelSelectPanel.SetActive(false);

            m_playerName.text = m_playerName.text.Trim();
            if (m_playerName.text.Length > 20)
            {
                m_playerName.text = m_playerName.text.Substring(0, 19);
            }
        }
        else
        {
            m_nameEntryPanel.SetActive(false);
            m_levelSelectPanel.SetActive(true);

            m_levelSelect.text = m_levels[m_selectedLevelIndex];
        }
    }

    public void ConfirmNameEntry()
    {
        m_nameEntry = false;
        PlayerPrefs.SetString("name", m_playerName.text);
    }

    public void ReturnToNameEntry()
    {
        m_nameEntry = true;
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(m_selectedLevelIndex + 1);
    }

    public void NextLevel()
    {
        m_selectedLevelIndex++;
        m_selectedLevelIndex = Mathf.Clamp(m_selectedLevelIndex, 0, m_highestCompletedLevel);
    }

    public void PreviousLevel()
    {
        m_selectedLevelIndex--;
        m_selectedLevelIndex = Mathf.Clamp(m_selectedLevelIndex, 0, m_highestCompletedLevel);
    }

    public void Quit()
    {
        // TODO fix html 5
        Application.Quit();
    }
}
