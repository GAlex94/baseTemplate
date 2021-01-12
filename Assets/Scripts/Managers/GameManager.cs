using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace baseTemplate
{
    public class GameManager : Singleton<GameManager>
    {
        private bool _isDebug = true;
        private bool _isFakeShop = true;

        [field: SerializeField] public StateGameEnum CurrentStateGame { get; private set; }

        public bool IsDebug => _isDebug;
        public bool IsFakeShop => _isFakeShop;
        public static bool IsFirstStart { get; set; }

        private int _idMusic;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _idMusic = -1;
        }

        void Start()
        {
            StartCoroutine(DefferGameStart());
        }
        
        IEnumerator DefferGameStart()
        {
            yield return new WaitForEndOfFrame();

            IGame mainGameObject = FindObjectsOfType<MonoBehaviour>().OfType<IGame>().FirstOrDefault();
            if (mainGameObject != null)
                mainGameObject.StartGame();
            else
            {
                Debug.LogError("IGame object not found in scene! Game didn't launch..");
            }

            yield return new WaitForEndOfFrame();
        }

        public void Init(bool isDebug, bool isFakeShop,  StateGameEnum debugGameState, AudioClip musicAudioClip, float musicValue)
        {
            this._isDebug = isDebug;
            this._isFakeShop = isFakeShop;
            SetStateGame(debugGameState);

            CurrentStateGame = debugGameState;
            _idMusic = -1;

            if (!Debug.isDebugBuild)
            {
                SetStateGame(StateGameEnum.Menu);
                this._isDebug = false;
                this._isFakeShop = false;
            }

            if (musicAudioClip != null)
            {
                _idMusic = SoundManager.PlayMusic(musicAudioClip, musicValue, true, true, 0.0f, 0.0f);
            }
        }
        
        #region Activators from loading scene

        public void RestoreMenu()
        {
            SetStateGame(StateGameEnum.Menu);
            SceneManager.sceneLoaded += OnSceneLoadedMenu;
        }
        
        void OnSceneLoadedMenu(Scene curScene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedMenu;
            StartCoroutine(DefferRestoreMenu());
        }

        IEnumerator DefferRestoreMenu()
        {
            yield return new WaitForEndOfFrame();
            IGame mainGameObject = FindObjectsOfType<MonoBehaviour>().OfType<IGame>().FirstOrDefault();
            if (mainGameObject != null)
                mainGameObject.StartGame();
            else
            {
                Debug.LogError("IGame object not found in scene! Game didn't launch..");
            }
        }

        public void ActivateSimpleGameOnScene()
        {
            SceneManager.sceneLoaded += OnSceneLoadedGame;
        }

        void OnSceneLoadedGame(Scene curScene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedGame;
            StartCoroutine(DefferActivateGameOnSceneGame());
        }

        IEnumerator DefferActivateGameOnSceneGame()
        {
            yield return new WaitForEndOfFrame();
            IGame mainGameObject = FindObjectsOfType<MonoBehaviour>().OfType<IGame>().FirstOrDefault();
            if (mainGameObject != null)
                mainGameObject.StartGame();
            else
            {
                Debug.LogError("IGame object not found in scene! Game didn't launch..");
            }
        }

        #endregion

        public void SetStateGame(StateGameEnum stateGame)
        {
            CurrentStateGame = stateGame;
        }
    }

    public enum StateGameEnum
    {
        Menu,
        Pause,
        Game,
    }
}

