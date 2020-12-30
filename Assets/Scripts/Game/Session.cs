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

        [SerializeField]
        private string gameEndingSceneName;

        public IState CurrentState { get => m_currentState; set => m_currentState = value; }

        private AsyncOperation m_openScene;
        private bool m_gameEnding = false;
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
                StartCoroutine(LoadScene(gameOverSceneName));
            }

            CurrentState = CurrentState.Update();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return null;

            m_openScene = SceneManager.LoadSceneAsync(gameOverSceneName);
            m_openScene.allowSceneActivation = false;

            while (!m_openScene.isDone)
            {
                //Output the current progress
                Debug.Log("Loading progress: " + (m_openScene.progress * 100).ToString() + "%");

                // Check if the load has finished
                if (m_openScene.progress >= 0.9f)
                {
                    m_openScene.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        public void LaunchGameEnding()
        {
            if(!m_gameEnding)
            {
                m_gameEnding = true;
                StartCoroutine(LoadScene(gameEndingSceneName));
            }
        }
    }
}
