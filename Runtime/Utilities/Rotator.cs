using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalArtsAFAD
{
    /// <summary>
    /// Helper component for scriptable rotation of objects around any axis.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [Tooltip("Rotation speed around X. In degrees per second")]
        public float speedX = 0;
        [Tooltip("Rotation speed around Y. In degrees per second")]
        public float speedY = 0;
        [Tooltip("Rotation speed around Z. In degrees per second")]
        public float speedZ = 0;

        [Tooltip("True if the object should be randomly rotated on start.")]
        public bool startRandomized = false;

        [Tooltip("True if the object should be rotating automatically. Otherwise rotation needs to be turned on by script.")]
        public bool startRotating = true;

        bool rotating = false;

        // Start is called before the first frame update
        void Start()
        {
            if (startRandomized)
            {
                if (speedX != 0) transform.Rotate(Vector3.right, Random.Range(0, 360));
                if (speedZ != 0) transform.Rotate(Vector3.forward, Random.Range(0, 360));
                if (speedY != 0) transform.Rotate(Vector3.up, Random.Range(0, 360));
            }

            if (startRotating) StartRotating();
        }

        // Update is called once per frame
        void Update()
        {
            if (rotating)
            {
                transform.Rotate(Vector3.right, Time.deltaTime * speedX);
                transform.Rotate(Vector3.forward, Time.deltaTime * speedZ);
                transform.Rotate(Vector3.up, Time.deltaTime * speedY);
            }
        }

        public void StartRotating()
        {
            rotating = true;
        }

        public void StopRotating()
        {
            rotating = false;
        }
    }
}