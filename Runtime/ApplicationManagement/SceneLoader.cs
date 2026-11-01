using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Component for loading scenes. 
    /// If ApplicationManager is present, it loads scene through it, taking advantage of additive loading (If set so in Application Manager) and screen fading
    /// If ApplicationManager is not present, SceneLoader loads scenes directly.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public string sceneName = "";

        ApplicationManager manager;

        private void Start()
        {
            manager = FindAnyObjectByType<ApplicationManager>();
        }

        public void LoadScene()
        {
            if (sceneName != "")
            {
                LoadScene(sceneName);
            }
        }

        public void LoadScene(string name)
        {
            if (manager != null)
                manager.LoadScene(name);
            else
                SceneManager.LoadScene(name);
        }

        public void Reload()
        {
            if (manager != null)
                manager.Reload();
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

    }
}