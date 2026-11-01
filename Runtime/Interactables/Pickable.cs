using UnityEngine;
using UnityEngine.SceneManagement;

namespace DigitalArtsAFAD
{
    public class Pickable : Interactable
    {
        public string dropMessage = "Drop";
        public string throwMessage = "Throw";
        public bool throwable = false;

        bool picked = false;
        bool throwing = false;
        string oldMessage;
        Rigidbody rb;
        Vector3 force = Vector3.zero;
        GameObject cam;

        bool _kinematicChanged = false;
        public bool kinematicChanged { get => _kinematicChanged; }
        bool unsetKinematicChanged = false;


        Scene oldScene;
        Transform oldParent;

        void Update()
        {
            if (picked && cam)
            {
                float angle = Vector3.Angle(Vector3.up, cam.transform.forward);
                
                if (throwable && angle < 90)
                {
                    throwing = true;
                    message = throwMessage;
                }
                else
                {
                    throwing = false;
                    message = dropMessage;
                }
            }
        }

        private void LateUpdate()
        {
        }

        private void Reset()
        {
            message = "Pick up";
        }

        public override void Use(PlayerBase player)
        {
            base.Use(player);

            Player3D player3D = player as Player3D;

            cam = Camera.main.gameObject;

            if (!picked) // Pick up object
            {
                picked = true;
                player3D.SetForcedInteractable(this);

                // remember original hierarchy
                oldScene = gameObject.scene;
                oldParent = gameObject.transform.parent;

                // move to camera hierarchy
                transform.SetParent(cam.transform, true);

                oldMessage = message;
                message = dropMessage;

                if (rb = GetComponent<Rigidbody>())
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    _kinematicChanged = true;
                }                
                
                //StartCoroutine(UpdateCarriedPosition());
            }
            else // Release object
            {
                picked = false;
                player3D.SetForcedInteractable(null);

                // Return to original hierarchy
                transform.SetParent(oldParent, true);
                if (oldParent == null)
                    SceneManager.MoveGameObjectToScene(gameObject, oldScene);

                message = oldMessage;

                if (rb)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    _kinematicChanged = true;
                    if (throwable && throwing)
                    {
                        force = player3D.strength * cam.transform.forward;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (force.magnitude > 0 && rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
                force = Vector3.zero;
            }

            if (unsetKinematicChanged)
            {
                _kinematicChanged = false;
                unsetKinematicChanged = false;
            }
            else
            if (kinematicChanged)
            {
                unsetKinematicChanged = true;
            }

        }
    }
}