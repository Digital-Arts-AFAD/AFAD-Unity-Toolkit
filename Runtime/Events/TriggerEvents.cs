using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class TriggerEvents : MonoBehaviour
    {
        [Tooltip("Check if the trigger should be activated only once")]
        public bool onlyOnce = false;

        [Tooltip("Object tag that triggers action. Leave empty if all objects should trigger action")]
        public string allowedTag = "Player";

        [Space(10)]
        public UnityEvent onEnter;
        public UnityEvent onLeave;

        bool wasEntered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (allowedTag == "" || other.gameObject.CompareTag(allowedTag))
            {
                TriggerEntered(other.gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (allowedTag == "" || collision.gameObject.CompareTag(allowedTag))
            {
                TriggerEntered(collision.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (allowedTag == "" || other.gameObject.CompareTag(allowedTag))
            {
                TriggerExit(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (allowedTag == "" || collision.gameObject.CompareTag(allowedTag))
            {
                TriggerExit(collision.gameObject);
            }
        }


        void TriggerEntered(GameObject go)
        {
            if (!wasEntered || !onlyOnce)
            {
                onEnter.Invoke();
                wasEntered = true;
            }
        }

        void TriggerExit(GameObject go)
        {
            if (!wasEntered || !onlyOnce)
            {
                onLeave.Invoke();
                wasEntered = true;
            }
        }
    }
}