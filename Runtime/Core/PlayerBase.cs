using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Base class for Player interaction controller. 
    /// - handles user input not handled by character controller (interaction, menu)
    /// - registers interactable object candidates and triggers related events
    /// - sends interaction messages to interactable objects
    /// 
    /// The class is abstract and should never be instantiated.
    /// Derived classes must implement Disable() and Enable() abstract methods and override virtual method FindInteractable().
    /// These methods should disable/enable player movement and interaction for the time while, e.g., menu is opened.
    /// 
    /// This class is agnostic of camera type (third person, first person, 2D side, 2D isometric, etc...).
    /// Finding nearby interactables need to be performed by overriding virtual method.
    /// </summary>
    public abstract class PlayerBase : MonoBehaviour
    {
        [Header("UI elements")]
        [Tooltip("GUI object displaying the name of the action of the currently focused object. GUI text object can be a child of it.")]
        public GameObject actionMessage;

        /// <summary>
        /// The interactable object that is currently in focus.
        /// </summary>
        protected IInteractable target = null;

        /// <summary>
        /// Used, e.g., when carrying an object. All interaction is forced onto this target. No other objects can be interacted with.
        /// </summary>
        protected IInteractable forced = null;

        protected PlayerEvents events;

        protected ApplicationManager manager;

        private InputAction interactAction;
        private InputAction menuAction;

        /// <summary>
        /// In cases where the script is not on the same object as PlayerInput, input event handlers are not called via messages from PlayerInput, but are attached to events.
        /// </summary>
        private bool eventsAreAttached = false;

        /// <summary>
        /// Disable movement and interaction. E.g. when menu is open.
        /// </summary>
        public abstract void Disable();

        /// <summary>
        /// Enable movement and interaction back after it's been disabled.
        /// </summary>
        public abstract void Enable();

        private void Awake()
        {
            events = GetComponent<PlayerEvents>();
        }

        private void InteractHandler(InputAction.CallbackContext obj)
        {
            OnInteract();
        }
        private void MenuHandler(InputAction.CallbackContext obj)
        {
            OnMenu();
        }

        // Update is called once per frame
        protected void BaseUpdate()
        {
            IInteractable oldInteractable = target;

            // Find interactable. It's either the carried object (forced) or look for interactable nearby
            if (forced != null)
                target = forced; 
            else
                target = FindInteractable();

            // Emit hover events when interactable changes
            if (forced == null && events != null)
            {
                if (oldInteractable != target)
                {
                    events.OnInteractableBlur.Invoke(oldInteractable);
                    events.OnInteractableFocus.Invoke(target);
                }
            }

            if (actionMessage)
            {
                if (target != null)
                {
                    actionMessage.SetActive(true);
                    actionMessage.GetComponentInChildren<TextMeshProUGUI>().text = target.GetMessage();
                }
                else
                {
                    actionMessage.SetActive(false);
                    actionMessage.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }                    
            }
        }

        protected virtual IInteractable FindInteractable()
        {
            return null;
        }


        protected void OnInteract()
        {
            if (target != null)
            {                
                target.Use(this);
            }
        }

        protected void OnMenu()
        {
            // Find application manager on first invoke of menu. Attach menu show/hide handlers
            if (manager == null)
            {
                manager = FindAnyObjectByType<ApplicationManager>();
                if (manager != null)
                {
                    manager.onMenuShow.AddListener(() => { this.Disable(); });
                    manager.onMenuHide.AddListener(() => { this.Enable(); });
                }
            }

            if (manager != null)
            {
                manager.ToggleMenu();
            }
        }

        protected bool InteractableIsValid(IInteractable interactable)
        {
            return (interactable != null && interactable.IsActive());
        }

        public void SetForcedInteractable(IInteractable forced)
        {
            this.forced = forced;
            if (events)
            {
                events.OnInteractableBlur.Invoke(forced);
            }
        }

        /// <summary>
        /// Attaches event handlers to Interact and Menu actions in the Input Actions.
        /// If no such actions are present in Input Actions, it creates them and and binds them to default Keyboard.E and Keyboard.Esc
        /// </summary>
        void AttachHandlers()
        {
            PlayerInput input = FindAnyObjectByType<PlayerInput>();
            
            if (input == null)
            {
                Debug.LogWarning("PlayerInput component not found on Player gameobject.");
                return;
            }

            if (input.gameObject != gameObject)
            {
                eventsAreAttached = true;

                // Attempt to find existing input actions ...
                if (interactAction == null && input != null)
                {
                    interactAction = input.actions.FindAction("Interact"); ;
                }
                if (menuAction == null && input != null)
                {
                    menuAction = input.actions.FindAction("Menu");
                }

                // ... if not found, create own input actions
                if (interactAction == null)
                {
                    interactAction = new InputAction();
                    interactAction.AddBinding("<Keyboard>/e");
                    interactAction.Enable();
                }
                if (menuAction == null)
                {
                    menuAction = new InputAction();
                    menuAction.AddBinding("<Keyboard>/escape");
                    menuAction.Enable();
                }

                menuAction.performed += MenuHandler;
                interactAction.performed += InteractHandler;
            }
        }

        void DetachHandlers()
        {
            if (eventsAreAttached)
            {
                interactAction.performed -= InteractHandler;
                menuAction.performed -= MenuHandler;
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
    }
}