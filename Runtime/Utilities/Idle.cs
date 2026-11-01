using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace DigitalArtsAFAD
{
    public class Idle : MonoBehaviour
    {
        public float timer = 30;

        public UnityEvent OnIdleBegin;
        public UnityEvent OnIdleEnd;


        float counter = 0;
        bool isIdle = false;

        // Update is called once per frame
        void Update()
        {
            if (!isIdle)
            {
                counter += Time.deltaTime;
                if (counter > timer)
                {
                    OnIdleBegin.Invoke();
                    isIdle = true;
                }                
            }
        }

        private void OnEnable()
        {
            AttachHandlers();
        }

        private void OnDisable()
        {
            DetachHandlers();
        }

        private void OnDestroy()
        {
            DetachHandlers();
        }

        void OnEvent(InputEventPtr eventPtr, InputDevice device)
        {
            counter = 0;

            if (isIdle)
            {
                OnIdleEnd.Invoke();
                isIdle = false;
            }
        }

        void AttachHandlers()
        {
            InputSystem.onEvent += OnEvent;
        }
        void DetachHandlers()
        {
            InputSystem.onEvent -= OnEvent;
        }
    }
}