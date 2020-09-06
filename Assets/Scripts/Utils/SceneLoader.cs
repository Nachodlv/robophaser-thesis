using System;
using System.Threading.Tasks;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    [RequireComponent(typeof(Fader))]
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        private Fader _fader;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _fader = GetComponent<Fader>();
        }

        public async void LoadSceneAsyncWithError(int sceneIndex, string errorMessage)
        {
            await LoadSceneAsync(sceneIndex);
            var errorDisplayer = FindObjectOfType<ErrorDisplayer>();
            if (errorDisplayer == null) return;
            errorDisplayer.ShowError(errorMessage);
        }

        public async Task LoadSceneAsync(int sceneIndex)
        {
            await _fader.FadeIn();
            PhotonNetwork.LoadLevel(sceneIndex);
            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                await Task.Yield();
            }
            await _fader.FadeOut();
        }
    }
}
