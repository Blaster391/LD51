using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> m_killAudioClips = new List<AudioClip>();

    [SerializeField]
    private AudioClip m_musicClip;
    [SerializeField]
    private AudioClip m_winClip;
    [SerializeField]
    private AudioClip m_loseClip;
    [SerializeField]
    private AudioClip m_playerDamage;

    [SerializeField]
    private AudioSource m_ostSource;
    [SerializeField]
    private AudioSource m_playerDamageSource;
    [SerializeField]
    private AudioSource m_shotSource;
    [SerializeField]
    private AudioSource m_gameOverSource;
    [SerializeField]
    private AudioSource m_killSource;

    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = 0.5f;

        m_ostSource.loop = true;
        m_ostSource.clip = m_musicClip;
        m_ostSource.Play();

        m_playerDamageSource.clip = m_playerDamage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerDamage()
    {
        m_playerDamageSource.Stop();
        m_playerDamageSource.Play();
    }

    public void Shoot(AudioClip clip)
    {
        m_shotSource.Stop();
        m_shotSource.clip = clip;
        m_shotSource.Play();
    }

    public void LoseGame()
    {
        StopAudio();

        m_gameOverSource.clip = m_loseClip;
        m_gameOverSource.Play();
    }

    public void WinGame()
    {
        StopAudio();

        m_gameOverSource.clip = m_winClip;
        m_gameOverSource.Play();
    }

    public void Kill()
    {
        int index = Random.Range(0, m_killAudioClips.Count);

        m_killSource.Stop();
        m_killSource.clip = m_killAudioClips[index];
        m_killSource.Play();
    }

    private void StopAudio()
    {
        m_ostSource.Stop();
        m_gameOverSource.Stop();
    }

}
