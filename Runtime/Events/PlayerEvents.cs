using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Helper component that triggers events related to player or interaction.
    /// The component should be attached to the same object as Player (or any PlayerBase-derived component)
    /// </summary>
    public class PlayerEvents : MonoBehaviour
    {
        [Tooltip("Triggered when player (movement, interaction) is disabled. E.g. when menu is open or a cutscene happens.")]
        public UnityEvent OnPlayerDisabled;

        [Tooltip("Triggered when player (movement, interaction) is re-enabled")]
        public UnityEvent OnPlayerEnabled;

        [Tooltip("Triggered when an interactable object moves into in focus")]
        public UnityEvent<IInteractable> OnInteractableFocus;

        [Tooltip("Triggered when an interactable object moves out of focus")]
        public UnityEvent<IInteractable> OnInteractableBlur;
    }
}