using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class Timer : MonoBehaviour
    {
        [Space(5)]
        public bool startAutomatically = true;
        [Header("Simple firing")]
        [Space(5)]        

        [Tooltip("Delay before the timer fires.")]
        public float delay = 1f;

        [Header("Repeated firing")]
        [Space(5)]
        public int repeatCount = 0;
        public float endTime = 0;

        [Header("Random delay")]
        [Space(5)]
        public float minDelay = 0f;
        public float maxDelay = 0f;
        

        [Space(10)]
        public UnityEvent onTick;
        public UnityEvent onEnded;

        bool running = false;
        float value = 0;
        int counter = 0;
        float totalTime = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        private void OnEnable()
        {
            if (startAutomatically) StartTimer();
        }

        // Update is called once per frame
        void Update()
        {
            if (running)
            {
                value += Time.deltaTime;
                totalTime += Time.deltaTime;
                if (endTime > 0 && totalTime >= endTime)
                {
                    End();
                }
                else if (value >= delay)
                {
                    Fire();
                }
            }
        }

        void Fire()
        {
            onTick.Invoke();
            counter++;
            if (repeatCount > 0 && counter >= repeatCount)
            {
                End();
            }
            else
            {
                value = 0;
                RandomizeDelay();
            }
        }

        void End()
        {
            running = false;
            onEnded.Invoke();
        }

        void RandomizeDelay()
        {
            if (minDelay > 0 && maxDelay > 0 && minDelay < maxDelay)
                delay = Random.Range(minDelay, maxDelay);
        }

        public void StartTimer()
        {
            if (!running)
            {
                value = 0;
                counter = 0;
                totalTime = 0;
                RandomizeDelay();
                running = true;
            }
        }

        public void StopTimer()
        {
            running = false;
        }
    }
}