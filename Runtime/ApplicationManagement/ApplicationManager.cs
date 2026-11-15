using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
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
        /// - Modal windows
        /// </summary>

        public GameObject mainMenu;

        [Tooltip("Assign a Fader object to fade in / fade out during scene transitions")]
        public Fader fader;

        public Texture2D cursor;

        public bool stopTimeOnModal = true;

        public UnityEvent<Modal> onModalShow;
        public UnityEvent<Modal> onModalHide;

        [Tooltip("Simple scene mode unloads existing scene when loading new scene.\n\nAdditive scene mode has one primary scene which never unloads and replaces only the secondary scene.")]
        public SceneMode sceneMode = SceneMode.SIMPLE;

        [Tooltip("Name of the default secondary scene to be additively loaded when game starts. Leave blank if scene mode is simple.")]
        public string defaultScene = "";

        public GameObject[] disableObjectsDuringLoading;
        public GameObject[] enableObjectsDuringLoading;

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

        Modal activeModal = null;
        Stack<Modal> modalStack = new Stack<Modal>();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            player = FindAnyObjectByType<PlayerBase>();

            if (sceneMode == SceneMode.ADDITIVE)
            {
                if (defaultScene != "")
                    LoadScene(defaultScene);
            }

            if (cursor != null)
            {
                Vector2 dimensions = new Vector2(cursor.width, cursor.height);
                Vector2 pivot = new Vector2(dimensions.x / 2, dimensions.y / 2);
                Cursor.SetCursor(cursor, pivot, CursorMode.Auto);
                HideCursor();
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
                mainMenu.SetActive(true);
                menuVisible = true; 
            }
        }

        public void HideMenu()
        {
            if (mainMenu)
            {
                menuVisible = false;
                mainMenu.SetActive(false);
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
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(Camera.main.transform.position + Camera.main.transform.forward);
            Mouse.current.WarpCursorPosition(screenPoint);

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

        public void ShowModal(Modal m)
        {
            if (activeModal != null)
            {
                modalStack.Push(activeModal);
                activeModal.gameObject.SetActive(false);
            }
            activeModal = m;
            player.Disable();
            ShowCursor();
            if (stopTimeOnModal)
            {
                oldTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            onModalShow.Invoke(m);
        }


        public void HideModal(Modal m)
        {
            HideCursor();
            player.Enable();
            if (stopTimeOnModal)
            {
                Time.timeScale = oldTimeScale;
            }

            if (modalStack.Contains(m)) return;

            if (activeModal == m)
            {
                activeModal = null;
                onModalHide.Invoke(m);
            }
            else
                Debug.LogError("Modal mismatch");

            if (modalStack.Count > 0)
            {
                Modal old = modalStack.Pop();
                old.gameObject.SetActive(true);
            }
        }
    }
}