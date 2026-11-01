using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class Switch : MonoBehaviour, IInteractable
    {
        public string messageToTurnOn = "Turn on";
        public string messageToTurnOff = "Turn off";
        
        [Tooltip("Inactive interactable can't be used unless changed to active")]
        public bool active = true;

        [Tooltip("Specific interaction distance for this interactable. Overrides distance set in Player2D/Player3D")]
        public float overrideInteractionDistance = 0;

        [Tooltip("Cursor used in point'n'click mode ")]
        public Sprite cursor;

        public bool isOn = false;
        public bool singleUse = false;
        public float delayUntilNextUse = 0;

        [Space(10)]
        public UnityEvent OnTurnedOn;
        public UnityEvent OnTurnedOff;

        float timer = 0;

        void Update()
        {
            if ( timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <=0)
                {
                    Activate();
                }
            }
        }

        public void Activate()
        {
            active = true;
        }

        public void Deactivate()
        {
            active = false;
        }

        public string GetMessage()
        {
            return isOn ? messageToTurnOff : messageToTurnOn;
        }

        public bool IsActive()
        {
            return active;
        }

        public void Use(PlayerBase caller)
        {
            if (isOn)
            {
                isOn = false;
                OnTurnedOff.Invoke();
            }
            else
            {
                isOn = true;
                OnTurnedOn.Invoke();
            }

            if (singleUse)
                Deactivate();

            if (delayUntilNextUse > 0)
            {
                Deactivate();
                timer = delayUntilNextUse;
            }
        }

        public float GetOverrideDistance()
        {
            return overrideInteractionDistance;
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
        public Sprite GetCursor()
        {
            return this.cursor;
        }
    }
}