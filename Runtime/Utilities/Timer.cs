using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalArtsAFAD
{
    public class Timer : MonoBehaviour
    {
        [Space(5)]
        public bool startAutomatically = true;
        
        [Tooltip("Delay before the timer fires.")]
        public float interval = 1f;
        public float randomIntervalMin = 0f;
        public float randomIntervalMax = 0f;

        public int tickCount = 1;
        public float endTime = 0;

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
                else if (value >= interval)
                {
                    Tick();
                }
            }
        }

        void Tick()
        {
            onTick.Invoke();
            counter++;
            if (tickCount > 0 && counter >= tickCount)
            {
                End();
            }
            else
            {
                value = 0;
                RandomizeInterval();
            }
        }

        void End()
        {
            running = false;
            onEnded.Invoke();
        }

        void RandomizeInterval()
        {
            if (randomIntervalMin > 0 && randomIntervalMax > 0 && randomIntervalMin < randomIntervalMax)
                interval = Random.Range(randomIntervalMin, randomIntervalMax);
        }

        public void StartTimer()
        {
            if (!running)
            {
                value = 0;
                counter = 0;
                totalTime = 0;
                RandomizeInterval();
                running = true;
            }
        }

        public void StopTimer()
        {
            running = false;
        }
    }
}