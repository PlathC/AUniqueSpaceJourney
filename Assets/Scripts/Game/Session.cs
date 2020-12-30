using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AUSJ
{
    [RequireComponent(typeof(AudioSource))]
    public class Session : MonoBehaviour
    {
        private IState m_currentState;
        private Player m_player;

        [SerializeField]
        private bool backgroundMusic = false;

        [SerializeField]
        private string gameOverSceneName;

        public IState CurrentState { get => m_currentState; set => m_currentState = value; }

        private AsyncOperation m_openGameOverScene;
        private bool m_gameover = false;

        // Start is called before the first frame update
        void Start()
        {
            // Find player instance
            m_player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();

            CurrentState = new Tutorial(m_player, this);

            // Set to loop and start background music
            if (backgroundMusic)
            {
                GetComponent<AudioSource>().loop = true;
                GetComponent<AudioSource>().Play();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if ((m_player.CurrentHunger == 0 || m_player.CurrentThirst == 0) && !m_gameover)
            {
                m_gameover = true;
                StartCoroutine(LoadScene());
            }

            CurrentState = CurrentState.Update();
        }

        private IEnumerator LoadScene()
        {
            yield return null;

            m_openGameOverScene = SceneManager.LoadSceneAsync(gameOverSceneName);
            m_openGameOverScene.allowSceneActivation = false;

            while (!m_openGameOverScene.isDone)
            {
                //Output the current progress
                Debug.Log("Loading progress: " + (m_openGameOverScene.progress * 100).ToString() + "%");

                // Check if the load has finished
                if (m_openGameOverScene.progress >= 0.9f)
                {
                    m_openGameOverScene.allowSceneActivation = true;
                }

                yield return null;
            }
        }

    }
}
