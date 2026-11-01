using DigitalArtsAFAD;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Base class for all interactable object. The base class is agnostic of 2D/3D/camera types and only handles logic.
    /// Interaction with interactables happens by Player object sending a "Use" message to the interactable and invoking OnUse event on interactable.
    /// Interactables may implement Use(object player) method in case they need their own specific behaviour. 
    /// E.g. Pickable interactable will set Player's forcedInteractable to the picked object.
    /// </summary>
    public class Interactable : MonoBehaviour, IInteractable
    {
        public string message = "Use";

        [Tooltip("Inactive interactable can't be used unless changed to active")]
        public bool active = true;

        [Tooltip("Specific interaction distance for this interactable. Overrides distance set in Player2D/Player3D")]
        public float overrideInteractionDistance = 0;

        [Tooltip("Cursor icon used for cursor in point'n'click mode ")]
        public Sprite cursor;

        [Space(10)]
        [Tooltip("Called when player interacts with the object")]
        public UnityEvent<PlayerBase> OnUse;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public string GetMessage()
        {
            return message;
        }
        public void ChangeMessage(string newMessage)
        {
            message = newMessage;
        }

        public float GetOverrideDistance()
        {
            return overrideInteractionDistance;
        }

        public void Activate()
        {
            active = true;
        }

        public void Deactivate()
        {
            active = false;
        }

        public bool IsActive()
        {
            return active;
        }



        public virtual void Use(PlayerBase player)
        {
            OnUse.Invoke(player);
        }

        /// <summary>
        /// Editor helper which draws lines to all receivers of an event
        /// </summary>
        /// <param name="b"></param>
        protected void DrawLineGizmo(UnityEventBase b)
        {
            int targetsCount = b.GetPersistentEventCount();
            for (int i = 0; i < targetsCount; i++)
            {
                Object t = b.GetPersistentTarget(i);
                if (t is GameObject go)
                {
                    Gizmos.DrawLine(transform.position, go.transform.position);
                }
                else if (t is Component c)
                {
                    Gizmos.DrawLine(transform.position, c.transform.position);
                }
            }
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