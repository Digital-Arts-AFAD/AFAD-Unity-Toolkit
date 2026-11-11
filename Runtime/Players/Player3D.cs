using UnityEngine;
using UnityEngine.UI;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Player class implementation for 3D first-person interaction.
    /// It manages a crosshair in center of screen and looks for interactables by raycasting through it.
    /// </summary>
    public class Player3D : PlayerBase
    {
        [Tooltip("GUI image displaying the interaction crosshair")]
        public Image crosshair;
        [Tooltip("Sprite asset while no interactable object is in focus")]
        public Sprite crosshairPassive;
        [Tooltip("Sprite asset when there is an interactable object in focus")]
        public Sprite crosshairActive;

        [Header("Interaction in 3D")]

        [Tooltip("Component which handles player movement.\nWill be disabled when menu is open")]
        public MonoBehaviour motionController;        
        [Tooltip("Maximum distance for objects to be interactable")]
        public float interactionDistance = 4;
        [Tooltip("Physical strength of player, e.g. when throwing objects")]
        public float strength = 10;
        [Tooltip("True if the objects hidden behind another collider (e.g. wall) should be interactable too.")]
        public bool interactWithOccluded = false;
        [Tooltip("True if the colliders marked as triggers should be considered for interaction.")]
        public bool interactWithTriggers = false;

        [Tooltip("Define the layer for interactable objects. Default is everything.")]
        public LayerMask interactableLayerMask = ~0;



        
        void Start()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            BaseUpdate();

            if (crosshair)
            {
                if (target != null)
                {
                    Sprite s = target.GetCursor();
                    if (s == null)
                        s = crosshairActive;
                    crosshair.sprite = s;
                }
                else
                    crosshair.sprite = crosshairPassive;
            }
        }

        public override void Disable()
        {
            if (motionController)
            {
                motionController.enabled = false;
                events.OnPlayerDisabled.Invoke();
                crosshair.gameObject.SetActive(false);
            }
        }

        public override void Enable()
        {
            if (motionController)
            {
                motionController.enabled = true;
                events.OnPlayerEnabled.Invoke();
                crosshair.gameObject.SetActive(true);
            }
        }

        protected override IInteractable FindInteractable()
        {
            IInteractable result = null;
            QueryTriggerInteraction queryTriggerInteraction = interactWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            if ((Camera.main != null))
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                if (interactWithOccluded)
                {
                    RaycastHit[] hits = Physics.RaycastAll(ray, 2 * interactionDistance, interactableLayerMask, queryTriggerInteraction);

                    float minDistance = float.PositiveInfinity;
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hit = hits[i];

                        float distance = (Camera.main.transform.position - hit.point).magnitude;
                        IInteractable interactable = hit.transform.gameObject.GetComponentInParent<IInteractable>();
                        if (InteractableIsValid(interactable))
                        {
                            float maxDistance = interactionDistance;
                            
                            if (interactionDistance == 0 || distance < maxDistance)
                            {
                                if (distance < minDistance)
                                {
                                    result = interactable;
                                    minDistance = hit.distance;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayerMask, queryTriggerInteraction))
                    {
                        IInteractable interactable = hit.transform.gameObject.GetComponentInParent<IInteractable>();
                        if (interactable != null)
                        {
                            float maxDistance = interactionDistance;
                            float distance = (Camera.main.transform.position - hit.point).magnitude;
                            if (interactable.GetOverrideDistance() != 0)
                            {
                                maxDistance = interactable.GetOverrideDistance();
                            }
                            if (InteractableIsValid(interactable) && distance < maxDistance)
                            {
                                result = interactable;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private void OnDisable()
        {
            // This used to be a dirty fix for Unity Input System v1.4 bug which throws errors on game exit. Not required anymore, hopefully.
            //GetComponent<PlayerInput>().actions = null;
        }
    }
}