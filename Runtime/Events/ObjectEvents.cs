using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class ObjectEvents : MonoBehaviour
    {
        [Space(5)]
        [Tooltip("Triggered only once, when the object first time appears in the scene.")]
        public UnityEvent<GameObject> onStart;
        [Tooltip("Triggered when the object appears in the scene. Even multiple times.")]
        public UnityEvent<GameObject> onEnable;
        [Tooltip("Triggered when the object is destroyed or hidden.")]
        public UnityEvent<GameObject> onDisable;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            onStart?.Invoke(gameObject);
        }

        private void OnEnable()
        {
            onEnable?.Invoke(gameObject);
        }

        private void OnDisable()
        {
            onDisable?.Invoke(gameObject);
        }
    }
}  