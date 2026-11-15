using UnityEngine;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// When attached to canvas or UI element it disables player, stops time and shows cursor.
    /// Requires Application Manager in scene.
    /// </summary>
    public class Modal : MonoBehaviour
    {
        ApplicationManager appManager;
        
        private void Awake()
        {
            appManager = FindAnyObjectByType<ApplicationManager>();
            if (!appManager)
                Debug.LogWarning("ModalWindow requires ApplicationManager in scene.");
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
            appManager.ShowModal(this);
        }

        private void OnDisable()
        {
            appManager.HideModal(this);
        }

    }
}