using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class CollisionEvents : MonoBehaviour
    {
        public UnityEvent onCollision;
        
        string allowedTag = "";

        private void OnCollisionEnter(Collision collision)
        {
            if (allowedTag == "" || collision.gameObject.CompareTag(allowedTag))
            {
                CollisionEntered(collision.gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (allowedTag == "" || collision.gameObject.CompareTag(allowedTag))
            {
                CollisionEntered(collision.gameObject);
            }
        }

        void CollisionEntered(GameObject go)
        {
            onCollision.Invoke();
        }

    }
}