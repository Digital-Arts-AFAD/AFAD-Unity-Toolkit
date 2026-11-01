using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace DigitalArtsAFAD
{
    public enum SceneMode { SIMPLE, ADDITIVE };

    public class ApplicationManager : MonoBehaviour
    {
        /// <summary>
        /// Handles application-level commands and states:
        /// - Quitting the application
        /// - Scene loading/unloading
        /// - Main menu
        /// </summary>

        // TODO: Hide defaultScene in Inspector if sceneMoode is SIMPLE
        [Header("Main menu:")]
        [Space(5)]

        public GameObject mainMenu;
        public bool stopTimeWhileMenuOpen = true;

        [Space(10)]
        public UnityEvent onMenuShow;
        public UnityEvent onMenuHide;


        [Header("Scene loading:")]
        [Space(5)]

        [Tooltip("Simple scene mode unloads existing scene when loading new scene.\n\nAdditive scene mode has one primary scene which never unloads and replaces only the secondary scene.")]
        public SceneMode sceneMode = SceneMode.SIMPLE;

        [Tooltip("Name of the default secondary scene to be additively loaded when game starts. Leave blank if scene mode is simple.")]
        public string defaultScene = "";

        [Tooltip("Assign a Fader object to fade in / fade out during scene transitions")]
        public Fader fader;

        [Space(5)]
        public GameObject[] disableObjectsDuringLoading;
        public GameObject[] enableObjectsDuringLoading;

        [Space(10)]
        public UnityEvent onLoadingStarted;
        public UnityEvent onLoadingFinished;

        /// <summary>
        /// Name of the currently loaded secondary scene
        /// </summary>
        protected string currentScene = "";

        /// <summary>
        /// True while asynchronous operation is taking place
        /// </summary>
        bool loading = false;
        public bool isLoading { get { return loading; } }
        

        /// <summary>
        /// Flag is true if menu canvas is currently displayed
        /// </summary>
        bool menuVisible = false;

        /// <summary>
        /// Used when disabling moving and showing cursor
        /// </summary>
        bool cursorWasVisible;
        CursorLockMode cursorLockstate;

        PlayerBase player;

        float oldTimeScale = 1;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindAnyObjectByType<PlayerBase>();

            if (sceneMode == SceneMode.ADDITIVE)
            {
                if (defaultScene != "")
                    LoadScene(defaultScene);
            }
        }

        /// <summary>
        /// Quits the application
        /// </summary>
        public void Quit()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        public void Reload()
        {
            if (sceneMode == SceneMode.SIMPLE)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                StartCoroutine(LoadSceneAsync(currentScene));
        }

        public void FullReload()
        {
            if (loading)
            {
                Debug.LogWarning("Attempting to full reload while another scene is being unloaded/loaded asynchronously");
                return;
            }

            if (sceneMode == SceneMode.SIMPLE)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else
                StartCoroutine(ReloadFromScratch());
        }

        protected IEnumerator ReloadFromScratch()
        {
            yield return UnloadCurrentScene();
            SceneManager.LoadScene(gameObject.scene.buildIndex);
            yield return null;
        }

        public void LoadScene(string sceneName)
        {
            if (loading)
            {
                Debug.LogWarning("Attempting to load scene '" + sceneName + "' while another scene is being unloaded/loaded asynchronously");
                return;
            }

            Scene scene = SceneManager.GetSceneByName(sceneName);

            if (scene == null)
            {
                Debug.LogError("Scene not found: " + sceneName);
            }
            else
            {
                if (scene.isLoaded)
                {
                    currentScene = sceneName;
                    return;
                }
            }

            if (sceneMode == SceneMode.SIMPLE)
                SceneManager.LoadScene(sceneName);
            else
                StartCoroutine(LoadSceneAsync(sceneName));
        }

        protected IEnumerator UnloadCurrentScene()
        {
            if (currentScene != "")
            {
                if (fader)
                {
                    fader.FadeIn();
                    while (fader.phase != FadePhase.None)
                    {
                        yield return null;
                    }
                }

                AsyncOperation unloaded = SceneManager.UnloadSceneAsync(currentScene);
                while (unloaded != null && !unloaded.isDone)
                {
                    yield return null;
                }
            }
            yield return null;
        }

        protected IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loading)
            {
                Debug.LogWarning("Attempting to load " + sceneName + " while another scene is being unloaded/loaded asynchronously");
                yield break;
            }

            loading = true;
            onLoadingStarted.Invoke();
            if (player) player.Disable();

            if (currentScene == "")
            {
                if (fader) fader.Fill();
            }
            yield return null;

            // Show/Hide objects
            foreach (GameObject go in disableObjectsDuringLoading)
            {
                go.SetActive(false);
            }
            foreach (GameObject go in enableObjectsDuringLoading)
            {
                go.SetActive(true);
            }

            // First unload existing child scene
            yield return UnloadCurrentScene();


            // Then load new child scene
            AsyncOperation loaded = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loaded.isDone)
            {
                yield return null;
            }

            currentScene = sceneName;
            loading = false;
            onLoadingFinished.Invoke();
            if (player) player.Enable();

            // Hide/Show objects
            foreach (GameObject go in disableObjectsDuringLoading)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in enableObjectsDuringLoading)
            {
                go.SetActive(false);
            }

            if (fader) fader.FadeOut();

        }

        public void ShowMenu()
        {
            if (mainMenu)
            {
                onMenuShow.Invoke();
                mainMenu.SetActive(true);
                menuVisible = true;
                if (stopTimeWhileMenuOpen)
                {
                    oldTimeScale = Time.timeScale;
                    Time.timeScale = 0;
                }
                ShowCursor();
            }
        }

        public void HideMenu()
        {
            if (mainMenu)
            {
                HideCursor();
                menuVisible = false;
                mainMenu.SetActive(false);
                onMenuHide.Invoke();
                if (stopTimeWhileMenuOpen)
                {
                    Time.timeScale = oldTimeScale;
                }
            }
        }

        /// <returns>Menu visibility after toggle is performed</returns>
        public bool ToggleMenu()
        {
            if (menuVisible)
                HideMenu();
            else
                ShowMenu();

            return menuVisible;
        }


        public void ShowCursor()
        {
            cursorWasVisible = Cursor.visible;
            cursorLockstate = Cursor.lockState;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HideCursor()
        {
            Cursor.lockState = cursorLockstate;
            Cursor.visible = cursorWasVisible;
        }

        public string getScene()
        {
            return currentScene;
        }
    }
}