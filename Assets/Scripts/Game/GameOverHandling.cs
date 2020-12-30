using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.Extras;

public class GameOverHandling : MonoBehaviour
{
    [SerializeField]
    private SteamVR_LaserPointer laserPointer;

    [SerializeField]
    private string replaySceneName;

    private AsyncOperation m_openReplayScene;

    void Awake()
    {
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.CompareTag("Quit"))
        {
            Debug.Log("Closing Application.");
            Application.Quit();
        }
        else if (e.target.CompareTag("Replay"))
        {
            StartCoroutine(LoadScene());
        }
    }

    private IEnumerator LoadScene()
    {
        yield return null;

        m_openReplayScene = SceneManager.LoadSceneAsync(replaySceneName);
        m_openReplayScene.allowSceneActivation = false;

        while (!m_openReplayScene.isDone)
        {
            //Output the current progress
            Debug.Log("Loading progress: " + (m_openReplayScene.progress * 100).ToString() + "%");

            // Check if the load has finished
            if (m_openReplayScene.progress >= 0.9f)
            {
                m_openReplayScene.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
