using UnityEngine;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// When attached to canvas or UI element it disables player, stops time and shows cursor.
    /// Requires Application Manager in scene.
    /// </summary>
    public class ModalWindow : MonoBehaviour
    {
        ApplicationManager appManager;
        PlayerBase player;

        private void Awake()
        {
            appManager = FindAnyObjectByType<ApplicationManager>();
            if (!appManager)
                Debug.LogWarning("ModalWindow requires ApplicationManager in scene.");

            player = FindAnyObjectByType<PlayerBase>();
            if (!player)
                Debug.LogWarning("ModalWindow requires Player in scene.");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            player.Disable();
            appManager.ShowCursor();
        }

        private void OnDisable()
        {
            player.Enable();
            appManager.HideCursor();
        }

    }
}