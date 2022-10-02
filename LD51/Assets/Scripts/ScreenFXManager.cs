using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFXManager : MonoBehaviour
{
    Material screenFXMaterial;

    [SerializeField]
    float m_fadeTime = 5.0f;
    [SerializeField]
    float m_maxAlpha = 0.5f;

    RenderTexture m_savedScreen = null;

    float m_alpha = 0.0f;

    bool m_shouldScreenshot = false;

    // Start is called before the first frame update
    void Start()
    {
        screenFXMaterial = new Material(Shader.Find("Hidden/KillScreenShader"));
        m_shouldScreenshot = true;
    }

    public void TriggerScreenshot()
    {
        m_shouldScreenshot = true;
        m_alpha = m_maxAlpha;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        if (m_savedScreen == null || (m_savedScreen.width != source.width || m_savedScreen.height != source.height))
        {
            if(m_savedScreen != null)
            {
                m_savedScreen.Release();
            }

            m_savedScreen = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, m_savedScreen);
        }

        if(m_shouldScreenshot)
        {
            screenFXMaterial.SetFloat("_Alpha", m_alpha);
            screenFXMaterial.SetTexture("_KillScreenTex", m_savedScreen);

            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, temp, screenFXMaterial);

            Graphics.Blit(temp, m_savedScreen);
            m_shouldScreenshot = false;

            temp.Release();
        }

        m_alpha -= Time.deltaTime / m_fadeTime;

        m_alpha = Mathf.Clamp01(m_alpha);

        screenFXMaterial.SetFloat("_Alpha", m_alpha);
        screenFXMaterial.SetTexture("_KillScreenTex", m_savedScreen);

        Graphics.Blit(source, destination, screenFXMaterial);
    }
}
